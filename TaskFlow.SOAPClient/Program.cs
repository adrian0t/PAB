namespace TaskFlow.SOAPClient;
using ServiceReference;

public class Program
{
    static async Task Main(string[] args)
    {
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRHptaXRyeVphaXRzYXUiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc1MDU0NTMxMiwiaXNzIjoiVGFza0Zsb3dBUEkiLCJhdWQiOiJUYXNrRmxvd0NsaWVudHMifQ.tGov47pPB2MADOtJg-xJX-YYSl4C8CGFA5ECENjGZEI";
        
        var client = new CommentsSoapServiceClient(CommentsSoapServiceClient.EndpointConfiguration.BasicHttpBinding_ICommentsSoapService);
        
        client.Endpoint.EndpointBehaviors.Add(new JwtHeaderInspector(jwt));

        try
        {
            var comments = await client.DumpCommentsAsync();

            foreach (var comment in comments)
            {
                Console.WriteLine($"[{comment.CommentedAt}] {comment.UserName}: {comment.Text}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error calling SOAP service: " + ex.Message);
        }
        Console.ReadKey();
    }
}
