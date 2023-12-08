﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace WebApplication1.Helpers
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();

        }

        public DbSet<Post> Posts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(e => e.Posts)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

            modelBuilder.Entity<User>()
                .Ignore(u => u.AccessFailedCount)
                .Ignore(u => u.LockoutEnabled)
                .Ignore(u => u.LockoutEnd)
                .Ignore(u => u.PhoneNumber)
                .Ignore(u => u.ConcurrencyStamp)
                .Ignore(u => u.SecurityStamp)
                .Ignore(u => u.EmailConfirmed)
                .Ignore(u => u.PhoneNumberConfirmed)
                .Ignore(u => u.TwoFactorEnabled)
                ;


        }

    }

}