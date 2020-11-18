﻿using System;
using System.Collections.Generic;
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

        [HttpGet("list/{userid}")]
        public async Task<List<MusicInfo>> GetMusicListByUserId(int userid)
        {
            List<MusicInfo> res = new List<MusicInfo>();
            await db.Musics.Where(m => m.UserId == userid).ForEachAsync(m => res.Add(new MusicInfo { Name = m.MusicName, Url = config.GetSection("ContainerURL").Value + m.MusicFileName }));
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
            if (await db.Musics.Where(m => m.UserId == user.UserId && m.MusicName == model.MusicName).FirstOrDefaultAsync() != null)
                return Ok(new { msg = $"У вас уже есть запись с названием {model.MusicName}" });
            try
            {
                if (CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
                {
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.Value.ContainerName);
                    CloudBlockBlob musicBlockBlob = container.GetBlockBlobReference($"{user.Login}_" + model.MusicFile.FileName);
                    if (await musicBlockBlob.ExistsAsync())
                        return Ok(new { msg = $"В вашем хранилище уже есть файл {model.MusicFile.FileName}" });
                    CloudBlockBlob imageBlockBlob = container.GetBlockBlobReference($"{user.Login}_" + model.MusicImageFile.FileName);
                    if (await imageBlockBlob.ExistsAsync())
                        return Ok(new { msg = $"В вашем хранилище уже есть файл {model.MusicImageFile.FileName}" });
                    await musicBlockBlob.UploadFromStreamAsync(model.MusicFile.OpenReadStream());
                    await imageBlockBlob.UploadFromStreamAsync(model.MusicImageFile.OpenReadStream());
                    db.Musics.Add(new Music
                    {
                        MusicName = model.MusicName,
                        MusicFileName = $"{user.Login}_"+model.MusicFile.FileName,
                        MusicImageName = $"{user.Login}_"+model.MusicImageFile.FileName,
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
    }
}
