using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ExamProject_updated_
{
    internal class QuizDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionBox> QuestionBoxes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<UserTestHistory> UserTestHistories { get; set; }
        public DbSet<LeaderBoard> Leaderboards { get; set; }
        public DbSet<GlobalLeaderboard> GlobalLeaderboards { get; set; }

        public QuizDbContext() {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=QuizDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
    new Role { Id = 1, Name = "Admin" },
    new Role { Id = 2, Name = "User" }
);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "General Knowledge" },
                new Category { Id = 2, Name = "Science" },
                new Category { Id = 3, Name = "Math" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "admin123", Birthday = new DateTime(2000, 1, 1), RoleId = 1 }
            );

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserTestHistory>()
                .HasOne(uth => uth.User)
                .WithMany()
                .HasForeignKey(uth => uth.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTestHistory>()
                .HasOne(uth => uth.Test)
                .WithMany()
                .HasForeignKey(uth => uth.TestId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TestQuestion>()
                .HasOne(tq => tq.Test)
                .WithMany()
                .HasForeignKey(tq => tq.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestQuestion>()
                .HasOne(tq => tq.Question)
                .WithMany()
                .HasForeignKey(tq => tq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserTestHistory>()
                .HasIndex(uth => new { uth.UserId, uth.TestId })
                .IsUnique();

            modelBuilder.Entity<TestQuestion>()
                .HasOne(tq => tq.Test)
                .WithMany(t => t.TestQuestions)
                .HasForeignKey(tq => tq.TestId);

        }
    }
}
