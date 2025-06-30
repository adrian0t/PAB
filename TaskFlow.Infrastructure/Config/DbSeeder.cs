using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Config;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.EnsureCreatedAsync();

        if (context.Users.Count() > 0 ||
            context.UserProjects.Count() > 0 ||
            context.Projects.Count() > 0 ||
            context.TaskItems.Count() > 0)
            return;

        var globalRoles = new[] { "Admin", "User" };

        foreach (var role in globalRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var me = await CreateUserIfNotExists(userManager, "Adrian", "Password");
        var admin = await CreateUserIfNotExists(userManager, "Admin", "Password");
        var owner = await CreateUserIfNotExists(userManager, "Owner", "Password");
        var user = await CreateUserIfNotExists(userManager, "User", "Password");

        await userManager.AddToRoleAsync(me, "Admin");
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.AddToRoleAsync(owner, "User");
        await userManager.AddToRoleAsync(user, "User");

        if (!context.Projects.Any())
        {
            var project = new Project("DEMO", "Demonstration Project");

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            context.UserProjects.AddRange(new[]
            {
                new UserProject
                {
                    User = owner,
                    Project = project,
                    Role = ProjectRole.Owner
                },
                new UserProject
                {
                    User = user,
                    Project = project,
                    Role = ProjectRole.Participant
                }
            });

            context.TaskItems.Add(new TaskItem
            {
                TaskKey = project.GenerateNextTaskKey(),
                Title = $"First Test Task",
                Description = "do this goddamn project!",
                Project = project,
                Assignee = me,
                Status = Domain.Entities.TaskStatus.New
            });
            await context.SaveChangesAsync();

            var task = context.TaskItems.First();
            var comment = new Comment
            {
                Text = "Please check this task again.",
                Task = task,
                User = me 
            };
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            context.UnreadComments.Add(new UserUnreadComment
            {
                User = user,
                Comment = comment
            });

            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> CreateUserIfNotExists(
        UserManager<ApplicationUser> userManager,
        string userName,
        string password)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user != null) return user;

        user = new ApplicationUser
        {
            UserName = userName
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"Failed to create user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }
}