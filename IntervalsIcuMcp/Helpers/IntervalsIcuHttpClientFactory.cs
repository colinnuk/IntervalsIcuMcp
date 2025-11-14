using System.Text;
namespace LlmFitnessCoach.WebAPI.Helpers;

public static class IntervalsIcuHttpClientFactory
{
    /// <summary>
    /// Configures the provided HttpClient for Intervals.icu API with the correct base address and Basic Auth header.
    /// </summary>
    public static void ConfigureForIntervalsApi(this HttpClient client, string apiKey)
    {
        client.BaseAddress = new Uri("https://intervals.icu/api/v1/");
        var credentials = $"API_KEY:{apiKey}";
        var bytes = Encoding.UTF8.GetBytes(credentials);
        var authHeader = $"Basic {Convert.ToBase64String(bytes)}";
        client.DefaultRequestHeaders.Add("Authorization", authHeader);
    }
}