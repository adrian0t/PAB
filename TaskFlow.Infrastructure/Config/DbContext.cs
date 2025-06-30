using System.Collections.Generic;
using TaskFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.Design;

namespace TaskFlow.Infrastructure.Config;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Project> Projects { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserUnreadComment> UnreadComments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProject>()
            .Property<string>("UserName");
        modelBuilder.Entity<UserProject>()
            .Property<string>("ProjectKey");

        modelBuilder.Entity<TaskItem>()
            .Property<string>("ProjectKey");

        modelBuilder.Entity<Comment>()
           .Property<int>("CommentId");
        modelBuilder.Entity<Comment>()
           .Property<string>("TaskItemKey");
        modelBuilder.Entity<Comment>()
           .Property<string>("UserName");

        modelBuilder.Entity<Project>()
            .HasKey(p => p.Key);

        modelBuilder.Entity<UserProject>()
            .HasKey("UserName", "ProjectKey");

        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserProjects)
            .HasForeignKey("UserName")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserProject>()
            .HasOne(up => up.Project)
            .WithMany(p => p.UserProjects)
            .HasForeignKey("ProjectKey")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasKey(t => t.TaskKey);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey("ProjectKey")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasKey("CommentId");

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey("TaskItemKey")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey("UserName")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserUnreadComment>(entity =>
        {
            entity.HasKey("UserId", "CommentId");

            entity.HasOne(uc => uc.User)
                  .WithMany(u => u.UnreadComments)
                  .HasForeignKey("UserId");

            entity.HasOne(uc => uc.Comment)
                  .WithMany(c => c.UnreadByUsers)
                  .HasForeignKey("CommentId");
        });
    }
}
