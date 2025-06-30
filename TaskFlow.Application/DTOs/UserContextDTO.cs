using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs;

public class UserContextDTO
{
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsUser { get; set; }
    public bool IsOwner { get; set; }
    public bool IsParticipant { get; set; }
}
