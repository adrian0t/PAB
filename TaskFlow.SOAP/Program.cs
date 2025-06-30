using SoapCore;
using SoapCore.ServiceModel;
using SoapCore.Serializer;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TaskFlow.Application.Services;
using TaskFlow.SOAP.Services;
using TaskFlow.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSoapCore();
builder.Services.AddScoped<ICommentsSoapService, CommentsSoapService>();

builder.Services.AddPolicies();

var app = builder.Build();

//custom
await app.SeedDataAsync();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<ICommentsSoapService>(
        "/CommentsSoapService.asmx",
        new[] {
            new SoapEncoderOptions
            {
                MessageVersion = MessageVersion.Soap11,
                WriteEncoding = System.Text.Encoding.UTF8
            }
        },
        SoapSerializer.DataContractSerializer);
});

app.Run();