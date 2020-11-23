using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseProjectMusic.Models;
using CourseProjectMusic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CourseProjectMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly DataBaseContext db;
        private readonly IConfiguration config;
        private readonly IOptions<StorageConfiguration> storageConfig;
        private int UserId => int.Parse(User.Claims.Single(cl => cl.Type == ClaimTypes.NameIdentifier).Value);
        public MusicController(DataBaseContext db, IConfiguration config, IOptions<StorageConfiguration> sc)
        {
            this.db = db;
            this.config = config;
            storageConfig = sc;
        }


        [HttpGet("DownloadFile/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            MemoryStream ms = new MemoryStream();
            if (CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = BlobClient.GetContainerReference(storageConfig.Value.ContainerName);

                if (await container.ExistsAsync())
                {
                    CloudBlob file = container.GetBlobReference(fileName);

                    if (await file.ExistsAsync())
                    {
                        await file.DownloadToStreamAsync(ms);
                        Stream blobStream = file.OpenReadAsync().Result;

                        Response.Headers.Add("Accept-Ranges", "bytes");

                        return new FileStreamResult(blobStream, "audio/mp3");
                    }
                    else
                    {
                        return Content("File does not exist");
                    }
                }
                else
                {
                    return Content("Container does not exist");
                }
            }
            else
            {
                return Content("Error opening storage");
            }
        }

        [HttpGet("{id}")]
        public async Task<MusicInfo> GetMusicInfoById(int id)
        {
            Music music = await db.Musics.FindAsync(id);
            return new MusicInfo
            {
                Id = music.MusicId,
                Name = music.MusicName,
                Url = config.GetSection("ContainerURL").Value + music.MusicFileName,
                FileName = music.MusicFileName,
                Img = config.GetSection("ContainerURL").Value + music.MusicImageName,
                GenreId = music.MusicGenreId
            };
        }


        [HttpGet("list/{userid}")]
        public async Task<List<MusicInfo>> GetMusicListByUserId(int userid)
        {
            List<MusicInfo> res = new List<MusicInfo>();
            await db.Musics.Where(m => m.UserId == userid).ForEachAsync(m => res.Add(new MusicInfo {Id=m.MusicId, Name = m.MusicName, Url = config.GetSection("ContainerURL").Value + m.MusicFileName, 
                FileName=m.MusicFileName, Img=config.GetSection("ContainerURL").Value+m.MusicImageName, GenreId=m.MusicGenreId }));
            return res;
        }

        [HttpGet("listMusicGenres")]
        public async Task<List<MusicGenreInfo>> GetMusicGenresList()
        {
            List<MusicGenreInfo> res = new List<MusicGenreInfo>();
            await db.MusicGenres.ForEachAsync(g => res.Add(new MusicGenreInfo { Id = g.MusicGenreId, Name = g.GenreName, Description = g.GenreDescription }));
            return res;
        }

        [HttpPost("AddMusic")]
        [Authorize]
        public async Task<IActionResult> AddMusic([FromForm]AddMusicModel model)
        {
            User user = await db.Users.FindAsync(UserId);
            string dateTimeNow = DateTime.Now.ToString();
            if (await db.Musics.Where(m => m.UserId == user.UserId && m.MusicName == model.MusicName).FirstOrDefaultAsync() != null)
                return Ok(new { msg = $"У вас уже есть запись с названием {model.MusicName}" });
            try
            {
                if (CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
                {
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.Value.ContainerName);
                    CloudBlockBlob musicBlockBlob = container.GetBlockBlobReference($"{user.Login}_{dateTimeNow}_" + model.MusicFile.FileName);
                    if (await musicBlockBlob.ExistsAsync())
                        return Ok(new { msg = $"В вашем хранилище уже есть файл {model.MusicFile.FileName}" });
                    if (model.MusicImageFile!=null)
                    {
                        CloudBlockBlob imageBlockBlob = container.GetBlockBlobReference($"{user.Login}_music_{dateTimeNow}_" + model.MusicImageFile.FileName);
                        if (await imageBlockBlob.ExistsAsync())
                            return Ok(new { msg = $"В вашем хранилище уже есть файл {model.MusicImageFile.FileName}" });
                        await imageBlockBlob.UploadFromStreamAsync(model.MusicImageFile.OpenReadStream());
                    }
                    await musicBlockBlob.UploadFromStreamAsync(model.MusicFile.OpenReadStream());
                    db.Musics.Add(new Music
                    {
                        MusicName = model.MusicName,
                        MusicFileName = $"{user.Login}_{dateTimeNow}_"+ model.MusicFile.FileName,
                        MusicImageName =model.MusicImageFile==null?"default.png":$"{user.Login}_music_{dateTimeNow}_" + model.MusicImageFile.FileName,
                        UserId = user.UserId,
                        DateOfPublication = DateTime.Now.Date,
                        MusicGenreId = model.MusicGenreId
                    });
                    await db.SaveChangesAsync();
                    return Ok(new {msg=""});
                }
                return StatusCode(500);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //[HttpPut("EditMusic")]
        //[Authorize]
        //public async Task<IActionResult> EditMusic([FromForm] AddMusicModel model)
        //{

        //}

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<List<MusicInfo>> DeleteMusic(int id)
        {
            User user = await db.Users.FindAsync(UserId);
            Music music = await db.Musics.FindAsync(id);
            List<MusicInfo> res = new List<MusicInfo>();
            if (music != null)
            {
                try
                {
                    if(CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
                    {
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.Value.ContainerName);
                        if(await container.ExistsAsync())
                        {
                            CloudBlob blob = container.GetBlobReference(music.MusicFileName);
                            if (await blob.ExistsAsync())
                                await blob.DeleteAsync();
                            if (music.MusicImageName != "default.png")
                            {
                                blob = container.GetBlobReference(music.MusicImageName);
                                if (await blob.ExistsAsync())
                                    await blob.DeleteAsync();
                            }  
                            db.Musics.Remove(music);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch
                {
                }
            }
            return await GetMusicListByUserId(UserId);
        }
    }
}
