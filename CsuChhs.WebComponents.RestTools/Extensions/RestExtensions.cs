using Newtonsoft.Json;
using CsuChhs.WebComponents.RestTools.ResourceModels;

namespace CsuChhs.WebComponents.RestTools.Extensions;

public static class RestExtensions
{
    /// <summary>
    /// Given response.Data it attempts to deserialize into
    /// the ProblemDetails object.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static ProblemDetails? ToProbleDetails(this string content)
    {
        return JsonConvert
            .DeserializeObject<ProblemDetails>(content);
    }
}