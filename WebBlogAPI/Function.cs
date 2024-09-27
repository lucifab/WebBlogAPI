using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json.Nodes;
using WebBlogAPI.Class;
using WebBlogAPI.Controllers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WebBlogAPI;

public class Function
{

    /// <summary>
    /// Opening function that handles API events
    /// </summary>
    /// <param name="Class.APIRequest">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>Task</returns>
    public async Task<object> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        MySQLConnector db = new MySQLConnector();

        Console.WriteLine("Received request: " + JsonConvert.SerializeObject(request));

        string action = null;
        APIRequest parsedRequest = new APIRequest();

        if (String.IsNullOrEmpty(request.Body)!=true)
        {
            parsedRequest = JsonConvert.DeserializeObject<APIRequest>(request.Body);
        }

        Console.WriteLine(parsedRequest.Action);
        Console.WriteLine(parsedRequest.Post);

        try
        {
            switch (parsedRequest.Action)
            {
                case "getPosts":
                    var posts = await db.GetPostsAsync();
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = JsonConvert.SerializeObject(posts),
                        Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                    };
                case "pushPost":
                    await db.PushPostAsync(parsedRequest.Post);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                    };
                default:
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        { "message", "Action not supported" }
                    }
                            ),
                        Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                    };
            }
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        { "message", ex.Message }
                    }
                            ),
                Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
            };
        }
        
    }
}
