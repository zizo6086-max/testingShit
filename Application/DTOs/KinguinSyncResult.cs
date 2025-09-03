namespace Application.DTOs;

public class KinguinSyncResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public int TotalProductsProcessed { get; set; }
    public int ProductsCreated { get; set; }
    public int ProductsUpdated { get; set; }
    public int ProductsSkipped { get; set; }
    public int ProductsFailed { get; set; }
    public int TotalPagesProcessed { get; set; }
    public List<string> Errors { get; set; } = new();

    public static KinguinSyncResult CreateSuccess(
        DateTime startTime,
        DateTime endTime,
        int totalProcessed,
        int created,
        int updated,
        int skipped,
        int failed,
        int pagesProcessed)
    {
        return new KinguinSyncResult
        {
            IsSuccess = true,
            StartTime = startTime,
            EndTime = endTime,
            TotalProductsProcessed = totalProcessed,
            ProductsCreated = created,
            ProductsUpdated = updated,
            ProductsSkipped = skipped,
            ProductsFailed = failed,
            TotalPagesProcessed = pagesProcessed
        };
    }

    public static KinguinSyncResult CreateFailure(
        DateTime startTime,
        DateTime endTime,
        string errorMessage,
        List<string>? errors = null)
    {
        return new KinguinSyncResult
        {
            IsSuccess = false,
            StartTime = startTime,
            EndTime = endTime,
            ErrorMessage = errorMessage,
            Errors = errors ?? new List<string>()
        };
    }
}
