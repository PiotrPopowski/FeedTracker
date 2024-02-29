namespace FeedTracker.Contracts.Weather
{
    public record HighTemperatureMessage(double TempC, DateTime Date) : IMessage;
}
