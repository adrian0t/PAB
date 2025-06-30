using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TaskFlow.Domain.Entities;

public class Comment
{
    public string Text { get; set; } = null!;
    public DateTime CommentedAt { get; set; } = DateTime.Now;
    public ApplicationUser User { get; set; } = null!;
    public TaskItem Task {  get; set; } = null!;
    public ICollection<UserUnreadComment> UnreadByUsers { get; set; } = new List<UserUnreadComment>();

}