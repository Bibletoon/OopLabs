namespace Backups.RestorePointsLimiters.Configurations;

public class HybridRestorePointsLimiterConfig
{
    public HybridRestorePointsLimiterConfig(IRestorePointsLimiter first, IRestorePointsLimiter second, bool acceptBoth)
    {
        First = first;
        Second = second;
        AcceptBoth = acceptBoth;
    }

    public IRestorePointsLimiter First { get; }
    public IRestorePointsLimiter Second { get; }
    public bool AcceptBoth { get; }
}