namespace CskMasala.Receipt.Contracts;

public interface IReceiptService
{
    Task<byte[]> GeneratePdfAsync(Guid orderId, CancellationToken ct = default);
}
