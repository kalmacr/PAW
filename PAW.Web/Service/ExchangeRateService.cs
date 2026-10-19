using PAW.Architecture.Providers;
using System.Text.Json.Serialization;

namespace PAW.Web.Services;

public interface IExchangeRateService
{
    Task<TipoCambioResponse> GetExchangeRateAsync(CancellationToken ct = default);
}

public class ExchangeRateService : IExchangeRateService
{
    public async Task<TipoCambioResponse> GetExchangeRateAsync(CancellationToken ct = default)
    {
        var client = new TipoCambioClient();
        return await client.GetTipoCambioAsync(ct);
    }
}

// DTO Data transfer Objects for the API response
public class TipoCambioResponse
{
    [JsonPropertyName("dolar")]
    public Dolar Dolar { get; set; } = new();

    [JsonPropertyName("euro")]
    public Euro Euro { get; set; } = new();
}

public class Dolar
{
    [JsonPropertyName("venta")]
    public ValorFecha Venta { get; set; } = new();

    [JsonPropertyName("compra")]
    public ValorFecha Compra { get; set; } = new();
}

public class ValorFecha
{
    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }
}

public class Euro
{
    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }

    [JsonPropertyName("dolares")]
    public decimal Dolares { get; set; }

    [JsonPropertyName("colones")]
    public decimal Colones { get; set; }
}

public class TipoCambioClient(HttpClient? http = null)
{
    private const string Url = "https://api.hacienda.go.cr/indicadores/tc";

    private readonly HttpClient _http = http ?? new HttpClient();

    public async Task<TipoCambioResponse> GetTipoCambioAsync(CancellationToken ct = default)
    {
        /*var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.hacienda.go.cr/indicadores/tc");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();*/

        using var http = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Get, "https://api.hacienda.go.cr/indicadores/tc")
        {
            Version = System.Net.HttpVersion.Version11,
            VersionPolicy = HttpVersionPolicy.RequestVersionExact
        };
        req.Headers.UserAgent.ParseAdd("curl/8.7.1");
        req.Headers.Accept.ParseAdd("*/*");

        var res = await http.SendAsync(req);
        var content = await res.Content.ReadAsStringAsync();
        //var result = "{\r\n  \"dolar\": {\r\n    \"venta\": {\r\n      \"fecha\": \"2026-10-04\",\r\n      \"valor\": 462.08\r\n    },\r\n    \"compra\": {\r\n      \"fecha\": \"2026-10-04\",\r\n      \"valor\": 455.71\r\n    }\r\n  },\r\n  \"euro\": {\r\n    \"fecha\": \"2026-10-02\",\r\n    \"dolares\": 1.1266,\r\n    \"colones\": 520.58\r\n  }\r\n}";
        var result = await JsonProvider.DeserializeAsync<TipoCambioResponse>(content) 
            ?? throw new InvalidOperationException("Empty response from tipo de cambio API.");
        return result;
        //var result = await _http.GetFromJsonAsync<TipoCambioResponse>(Url, ct);
        //return result ?? throw new InvalidOperationException("Empty response from tipo de cambio API.");
    }
}
