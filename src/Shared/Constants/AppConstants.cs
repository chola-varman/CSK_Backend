namespace CskMasala.Shared.Constants;

public enum UserRole
{
    Guest,
    Customer,
    Admin
}

public static class AppConstants
{
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string ServiceName = "CskMasala";
    public const string ServiceVersion = "1.0.0";
}
