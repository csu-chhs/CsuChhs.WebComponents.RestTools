using System.Net;
using CsuChhs.WebComponents.Api.Exceptions;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace CsuChhs.WebComponents.RestTools;

public class JwtBaseClient
{
    /// <summary>
    /// Initialize a standard client without
    /// JWT Support.
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public RestClient InitializeRestClient(string baseUrl)
    {
        var options = new RestClientOptions(baseUrl)
        {
        };

        return new RestClient(options,
            configureSerialization: s => s.UseNewtonsoftJson());
    }

    /// <summary>
    /// Initialize a client specifically with JWT support.
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    public RestClient InitializeRestClient(string baseUrl,
        string jwtToken)
    {
        JwtAuthenticator authenticator = new JwtAuthenticator(jwtToken);
        var options = new RestClientOptions(baseUrl)
        {
            Authenticator = authenticator
        };

        return new RestClient(options,
            configureSerialization: s => s.UseNewtonsoftJson());
    }
    
    /// <summary>
    /// Error handler for GET operations that
    /// do not include a specific object.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public Exception ErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        Exception? exception)
    {
        return _RestErrorHandler(statusCode,
            responseContent,
            contentObjects,
            false,
            exception,
            null,
            null);
    }

    /// <summary>
    /// Error handler for GET operations that
    /// do include a reference to a singular object.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="exception"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Exception ErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        Exception? exception,
        int id)
    {
        return _RestErrorHandler(statusCode,
            responseContent,
            contentObjects,
            false,
            exception,
            null,
            id);
    }

    
    /// <summary>
    /// Error handler for common Post (Create) operations
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="exception"></param>
    /// <param name="resourceModel"></param>
    /// <returns></returns>
    public Exception PostErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        Exception? exception,
        object? resourceModel)
    {
        return _RestErrorHandler(statusCode,
            responseContent,
            contentObjects,
            true,
            exception,
            resourceModel,
            null);
    }
    
    /// <summary>
    /// Error handler for common PUT operations.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="exception"></param>
    /// <param name="resourceModel"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Exception PutErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        Exception? exception,
        object? resourceModel,
        int id)
    {
        return _RestErrorHandler(statusCode,
            responseContent,
            contentObjects,
            true,
            exception,
            resourceModel,
            id);
    }

    /// <summary>
    /// Handlers for Delete Calls.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="exception"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Exception DeleteErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        Exception? exception,
        int id)
    {
        return _RestErrorHandler(statusCode,
            responseContent,
            contentObjects,
            true,
            exception,
            null,
            id);
    }
    
    /// <summary>
    /// Common way of handling errors.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="contentObjects"></param>
    /// <param name="createUpdate"></param>
    /// <param name="exception"></param>
    /// <param name="resourceModel"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private Exception _RestErrorHandler(HttpStatusCode statusCode,
        string? responseContent,
        string contentObjects,
        bool createUpdate = false,
        Exception? exception = null,
        object? resourceModel = null,
        int? id = null)
    {
        if (statusCode == HttpStatusCode.NotFound)
        {
            return new ObjectNotFoundException();
        }

        if (statusCode == HttpStatusCode.Unauthorized ||
            statusCode == HttpStatusCode.Found)
        {
            return new UnauthorizedAccessException();
        }

        if (createUpdate)
        {
            var ex = new FailedToSaveToApiException($"Failed to create/update/delete {contentObjects}",
                exception);
            ex.AddWebTrace(responseContent);
            if (resourceModel != null)
            {
                ex.AddResourceModel(resourceModel);
            }

            if (id != null)
            {
                ex.Data.Add("id", id);
            }

            return ex;
        }
        else
        {
            var ex = new FailedToFetchFromApiException($"Failed to fetch {contentObjects}",
                exception);
            ex.AddWebTrace(responseContent);
            if (resourceModel != null)
            {
                ex.AddResourceModel(resourceModel);
            }

            if (id != null)
            {
                ex.Data.Add("id", id);
            }

            return ex;
        }
    }
}