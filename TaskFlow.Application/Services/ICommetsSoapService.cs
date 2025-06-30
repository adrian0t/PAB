using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using System.ServiceModel;
using System.Runtime.Serialization;
using TaskFlow.Application.DTOs;


namespace TaskFlow.Application.Services;

[ServiceContract]
public interface ICommentsSoapService

{
    [OperationContract]
    public Task<List<CommentDTO>> DumpCommentsAsync();
}


