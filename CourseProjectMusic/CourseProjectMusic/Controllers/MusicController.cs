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

        [HttpGet]
        [Authorize]
        public IActionResult S()
        {
            User user = db.Users.Find(UserId);
            return Ok(new { user.Mail});
        }

        [HttpPost("AddMusic")]
        [Authorize]
        public async Task<string> AddMusic(/*IFormFile asset*/)
        {
            User user = await db.Users.FindAsync(UserId);
            return user.Mail;
           
            //try
            //{
            //    if (CloudStorageAccount.TryParse(storageConfig.Value.ConnectionString, out CloudStorageAccount storageAccount))
            //    {
            //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //        CloudBlobContainer container = blobClient.GetContainerReference("SomeUserName_"+storageConfig.Value.ContainerName);
            //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(asset.FileName);
            //        await blockBlob.UploadFromStreamAsync(asset.OpenReadStream());
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
