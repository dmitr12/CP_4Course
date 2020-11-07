using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseProjectMusic.Models;
using CourseProjectMusic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CourseProjectMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private DataBaseContext db;
        private readonly IOptions<AuthOptions> authOptions;
        private int UserId => int.Parse(User.Claims.Single(cl => cl.Type == ClaimTypes.NameIdentifier).Value);
        public AuthUserController(DataBaseContext db, IOptions<AuthOptions> authOptions)
        {
            this.db = db;
            this.authOptions = authOptions;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AuthModel model)
        {
            var user = AuthenticateUser(model);
            if (user != null)
            {
                //Generate token
                var token = GenerateJWT(user);
                return Ok(new { access_token = token });
            }
            return Unauthorized();
        }

        private User AuthenticateUser(AuthModel model)
        {
            User user = db.Users.Where(u => u.Mail == model.Email && u.Password == HashClass.GetHash(model.Password)).FirstOrDefault();
            return user;
        }

        private string GenerateJWT(User user)
        {
            var authParams = authOptions.Value;
            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //Заголовки токена генерируются автоматически
            //Pylot токена состоит из claims
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString())
            };
            claims.Add(new Claim("login", user.Login));
            //foreach (var role in user.Roles)
            //{
            //    claims.Add(new Claim("role", role.ToString()));
            //}

            var token = new JwtSecurityToken(issuer: authParams.Issuer, audience: authParams.Audience, claims: claims, notBefore:DateTime.Now, expires: DateTime.Now.AddSeconds(authParams.TokenLifeTime),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
