using Microsoft.EntityFrameworkCore;

namespace Tamagotchi.Competition.Context
{
    public partial class TamagotchiCompetitionContext : DbContext
    {
        public TamagotchiCompetitionContext()
        { }

        public TamagotchiCompetitionContext(DbContextOptions<TamagotchiCompetitionContext> options)
            : base(options)
        { }

        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Score> Score { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Events>(entity =>
            {
                entity.ToTable("EVENTS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID");

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
                    .HasColumnType("time");

                entity.Property(e => e.RoomCode)
                    .IsRequired()
                    .HasColumnName("ROOM_CODE")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Start)
                    .HasColumnName("START")
                    .HasColumnType("time");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });

            modelBuilder.Entity<Score>(entity =>
            {
                entity.ToTable("SCORE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID");

                entity.Property(e => e.UserId).HasColumnName("USER_ID");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });
        }
    }
}
