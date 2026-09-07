using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ChessApi.Models
{
    public partial class ModelChessDB : DbContext
    {
        public ModelChessDB()
            : base("name=ModelChessDB1")
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Move> Moves { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Side> Sides { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(e => e.FEN)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Move>()
                .Property(e => e.FEN)
                .IsUnicode(false);

            modelBuilder.Entity<Move>()
                .Property(e => e.MoveNext)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Side>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<Side>()
                .Property(e => e.Points)
                .HasPrecision(2, 1);
        }
    }
}
