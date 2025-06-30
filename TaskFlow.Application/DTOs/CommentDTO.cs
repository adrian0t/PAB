using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs;
  
[DataContract]
public class CommentDTO
{
    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public DateTime CommentedAt { get; set; }

    [DataMember]
    public string UserName { get; set; }

    [DataMember]
    public string TaskKey { get; set; }
}