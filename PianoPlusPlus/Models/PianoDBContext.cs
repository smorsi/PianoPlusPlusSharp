using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PianoPlusPlus.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PianoPlusPlus.Models
{
    public class PianoDBContext : DbContext
    {

        public PianoDBContext() : base("PianoDBContext")
        {
        }
        
        public DbSet<Students> Students { get; set; }
        public DbSet<PianoPlusPlus.Models.WebRTC.Tutors> Tutors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}