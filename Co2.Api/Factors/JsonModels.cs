namespace Co2.Api.Factors;

public sealed class FactorRecord
{
    public string Id { get; set; } = "";
    public string Category { get; set; } = "";
    public string Region { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Value { get; set; }
    public string Source { get; set; } = "";
}

public sealed class FactorFile
{
    public string Version { get; set; } = "";
    public List<FactorRecord> Factors { get; set; } = new();
}
