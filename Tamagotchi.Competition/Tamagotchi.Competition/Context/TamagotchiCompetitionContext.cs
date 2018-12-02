using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Tamagotchi.Competition.Context
{
    public partial class TamagotchiCompetitionContext : DbContext
    {
        public TamagotchiCompetitionContext()
        {
        }

        public TamagotchiCompetitionContext(DbContextOptions<TamagotchiCompetitionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Score> Score { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=desktop-279l8sc\\sqlexpress;Database=tamagotchi-competition;User ID=skipp;Password=skipper5811465;Trusted_Connection=True;App=EFCore&quot;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Events>(entity =>
            {
                entity.ToTable("EVENTS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActionCode)
                    .IsRequired()
                    .HasColumnName("ACTION_CODE")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DeseaseCode)
                    .HasColumnName("DESEASE_CODE")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Finish)
                    .HasColumnName("FINISH")
                    .HasColumnType("date");

                entity.Property(e => e.RoomCode)
                    .IsRequired()
                    .HasColumnName("ROOM_CODE")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Start)
                    .HasColumnName("START")
                    .HasColumnType("date");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });

            modelBuilder.Entity<Score>(entity =>
            {
                entity.ToTable("SCORE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserId).HasColumnName("USER_ID");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });
        }
    }
}
