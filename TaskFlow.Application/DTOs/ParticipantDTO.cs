using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs;

public class ParticipantDTO
{
    public string User { get; set; } = null!;
    public string Role { get; set; } = null!;
}
