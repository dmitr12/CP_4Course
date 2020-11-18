using System;
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
        public async Task</*bool*/IActionResult> AddMusic([FromForm]IFormFile asset,[FromQuery] string musicName, [FromQuery] int musicGenreId)
        {
            User user = await db.Users.FindAsync(UserId);
            return Ok(new {name=asset.Name+"+"+musicName+"+"+musicGenreId+"+"+UserId});
            //try
            //{
            //    if (CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
            //    {
            //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //        CloudBlobContainer container = blobClient.GetContainerReference($"{user.Login}_" + storageConfig.Value.ContainerName);
            //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(model.asset.FileName);
            //        await blockBlob.UploadFromStreamAsync(model.asset.OpenReadStream());
            //        return true;
            //    }
            //    return false;
            //}
            //catch
            //{
            //    return false;
            //}
        }
    }
}
