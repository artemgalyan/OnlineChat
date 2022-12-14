namespace OnlineChat;

public static class Constants
{
    public static readonly TimeSpan AntispamMinTime = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan TimeoutSanitizerRefreshTime = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan TimeoutSanitizerMaxEmptyTime = TimeSpan.FromDays(1);
}