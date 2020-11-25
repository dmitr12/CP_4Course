using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProjectMusic.Models
{
    public class DataBaseContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MusicGenre> MusicGenres { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
    }
}
