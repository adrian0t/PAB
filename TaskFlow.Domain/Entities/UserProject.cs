using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Entities;

public class UserProject
{
    //public string UserName { get; set; }
    public ApplicationUser User { get; set; }
    //public string ProjectKey { get; set; }
    public Project Project { get; set; }

    // Role in project: "Owner", "Participant", etc.
    public ProjectRole Role { get; set; }
}

public enum ProjectRole
{
    Owner = 0,
    Participant = 1
}