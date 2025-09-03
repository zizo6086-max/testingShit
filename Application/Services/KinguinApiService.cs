using System.Text.Json;
using Application.Common.Interfaces;
using Application.DTOs.store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class KinguinApiService : IKinguinApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KinguinApiService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public KinguinApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<KinguinApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Kinguin:ApiKey"] ?? 
            throw new InvalidOperationException("Kinguin API key is not configured");
        _baseUrl = configuration["Kinguin:BaseUrl"] ?? 
            "https://gateway.kinguin.net/esa/api/v1";

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        // Configure HTTP client
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "UserZone-Platform/1.0");
    }

    public async Task<KinguinProductSearchResponseDto> GetProductsForSyncAsync(
        int page = 1,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_baseUrl}/products?page={page}&limit={Math.Min(limit, 100)}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<KinguinProductSearchResponseDto>(jsonContent, _jsonOptions);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize Kinguin API response");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products from Kinguin API");
            throw;
        }
    }
}
