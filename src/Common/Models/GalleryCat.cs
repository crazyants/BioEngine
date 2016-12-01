﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Common.Base;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Common.Models
{
    public class GalleryCat : ChildModel
    {
        public const int PicsOnPage = 24;

        [Key]
        public int Id { get; set; }

        public int Pid { get; set; }
        public string GameOld { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Url { get; set; }

        [ForeignKey(nameof(Pid))]
        public GalleryCat ParentCat { get; set; }

        public static void ConfigureDB(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GalleryCat>().ToTable("be_gallery_cats");
            modelBuilder.Entity<GalleryCat>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<GalleryCat>().Property(x => x.GameId).HasColumnName("game_id");
            modelBuilder.Entity<GalleryCat>().Property(x => x.DeveloperId).HasColumnName("developer_id");
            modelBuilder.Entity<GalleryCat>().Property(x => x.GameOld).HasColumnName("game_old");
            modelBuilder.Entity<GalleryCat>().Property(x => x.Pid).HasColumnName("pid");
            modelBuilder.Entity<GalleryCat>().Property(x => x.Title).HasColumnName("title");
            modelBuilder.Entity<GalleryCat>().Property(x => x.Desc).HasColumnName("desc");
            modelBuilder.Entity<GalleryCat>().Property(x => x.Url).HasColumnName("url");
        }
    }
}