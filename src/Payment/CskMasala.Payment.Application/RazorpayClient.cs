using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CskMasala.Payment.Application;

public class RazorpayClient(HttpClient httpClient, IConfiguration config)
{
    private readonly string _keyId = config["RAZORPAY_KEY_ID"] ?? string.Empty;
    private readonly string _keySecret = config["RAZORPAY_KEY_SECRET"] ?? string.Empty;

    public async Task<string> CreateOrderAsync(decimal amount, string receipt, CancellationToken ct)
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_keyId}:{_keySecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var body = new { amount = (int)(amount * 100), currency = "INR", receipt };
        var response = await httpClient.PostAsJsonAsync("https://api.razorpay.com/v1/orders", body, ct);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return content.GetProperty("id").GetString()!;
    }

    public bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string signature)
    {
        var payload = $"{razorpayOrderId}|{razorpayPaymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_keySecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return computed == signature.ToLower();
    }
}
