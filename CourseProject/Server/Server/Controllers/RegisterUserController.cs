using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server.Controllers
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

                User user = new User { Login = model.Login, Mail = model.Mail, Password = model.Password };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Ok(user);
            }
            return BadRequest();
        }
    }
}
