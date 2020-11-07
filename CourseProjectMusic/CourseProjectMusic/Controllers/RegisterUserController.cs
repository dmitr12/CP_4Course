using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProjectMusic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseProjectMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        DataBaseContext db;

        public RegisterUserController(DataBaseContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User us = await db.Users.Where(u => u.Mail == model.Mail).FirstOrDefaultAsync();
                if (us != null)
                    return BadRequest(new {msg= $"Пользователь с {model.Mail} уже зарегистрирован" });
                us= await db.Users.Where(u => u.Login == model.Login).FirstOrDefaultAsync();
                if(us!=null)
                    return BadRequest(new { msg = $"Пользователь с {model.Login} уже зарегистрирован" });
                User user = new User { Mail = model.Mail, Login = model.Login, Password = model.Password };
                try
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
                return Ok(user);
            }
            return BadRequest();
        }
    }
}
