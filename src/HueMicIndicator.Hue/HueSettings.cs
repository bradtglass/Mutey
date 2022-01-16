namespace HueMicIndicator.Hue;

public record HueSettings
{
    internal const string Sub = "hue";
    
    public string? AppKey { get; init; }
}