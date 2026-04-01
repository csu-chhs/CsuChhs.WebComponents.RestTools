using CsuChhs.Utils.Utilities;
using Newtonsoft.Json;

namespace CsuChhs.WebComponents.RestTools.Extensions;

public static class RestExtensions
{
    /// <summary>
    /// Given response.Data it attempts to deserialize into
    /// the ProblemDetails object.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static CustomProblemDetails? ToProblemDetails(this string content)
    {
        return JsonConvert
            .DeserializeObject<CustomProblemDetails>(content);
    }
}