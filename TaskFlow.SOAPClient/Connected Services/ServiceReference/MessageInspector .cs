using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;

public class JwtHeaderInspector : IClientMessageInspector, IEndpointBehavior
{
    private readonly string _jwtToken;

    public JwtHeaderInspector(string jwtToken)
    {
        _jwtToken = jwtToken;
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        var httpRequestMessageProperty = request.Properties.ContainsKey(HttpRequestMessageProperty.Name)
            ? (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name]
            : new HttpRequestMessageProperty();

        httpRequestMessageProperty.Headers["Authorization"] = $"Bearer {_jwtToken}";

        request.Properties[HttpRequestMessageProperty.Name] = httpRequestMessageProperty;

        return null;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        // No-op
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(this);
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

    public void Validate(ServiceEndpoint endpoint) { }
}
