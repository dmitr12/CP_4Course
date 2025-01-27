﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }
        [Required]
        [StringLength(50, MinimumLength =3)]
        public string Login { get; set; }
        [Required]
        [StringLength(30, MinimumLength =6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage ="Не удалось подтвердить пароль")]
        public string ConfirmPassword { get; set; }
    }
}
