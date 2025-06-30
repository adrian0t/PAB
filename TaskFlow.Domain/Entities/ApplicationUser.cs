using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public override string UserName { get; set; } = null!;
    public ICollection<UserProject>? UserProjects { get; set; } = null!;
    public ICollection<TaskItem>? AssignedTasks { get; set; } = null!;
    public ICollection<Comment>? Comments { get; set; } = null!;
    public ICollection<UserUnreadComment> UnreadComments { get; set; } = new List<UserUnreadComment>();

}