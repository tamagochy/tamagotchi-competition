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

            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 1,
                ActionCode = "brush",
                RoomCode = "playRoom",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 5
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 2,
                ActionCode = "takePicture",
                RoomCode = "playRoom",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 10
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 3,
                ActionCode = "playBall",
                RoomCode = "playRoom",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 15
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 4,
                ActionCode = "playTicTacToe",
                RoomCode = "playRoom",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 15
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 5,
                ActionCode = "giveAntiviral",
                RoomCode = "hospital",
                DeseaseCode = "cold",
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 100
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 6,
                ActionCode = "pshikIngalipt",
                RoomCode = "hospital",
                DeseaseCode = "angina",
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 100
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 7,
                ActionCode = "stripCandy",
                RoomCode = "hospital",
                DeseaseCode = "inflammationTricks",
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 100
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 8,
                ActionCode = "giveAntihistamine",
                RoomCode = "hospital",
                DeseaseCode = "allergy",
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 100
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 9,
                ActionCode = "dumplings",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 50
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 10,
                ActionCode = "pizza",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 45
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 11,
                ActionCode = "paste",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 30
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 12,
                ActionCode = "soup",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 25
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 13,
                ActionCode = "porridge",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 20
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 14,
                ActionCode = "yogurt",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 15
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 15,
                ActionCode = "oatCookies",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 2
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 16,
                ActionCode = "apple",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 5
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 17,
                ActionCode = "water",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 3
            });
            modelBuilder.Entity<Events>().HasData(new Events()
            {
                Id = 18,
                ActionCode = "tea",
                RoomCode = "kitchen",
                DeseaseCode = null,
                Start = new System.TimeSpan(0, 0, 0),
                Finish = new System.TimeSpan(23, 0, 0),
                Value = 3
            });
        }
    }
}
