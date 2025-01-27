﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(maximumLength:50, MinimumLength =3)]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public byte[] Avatar { get; set; }

        [StringLength(100, MinimumLength =3)]
        public string Name { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string Surname { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

    }
}
