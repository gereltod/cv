using System.Text.Json.Serialization;

public class Currency
{
    public string? code { get; set; }
    public string? symbol { get; set; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal rate { get; set; }
    public string? description { get; set; }
}
public class BTC
{
    public Dictionary<string, string>? time { get; set; }
    public string? disclaimer { get; set; }

    public Dictionary<string, Currency>? bpi { get; set; }
}
