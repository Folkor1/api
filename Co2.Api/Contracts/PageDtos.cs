namespace Co2.Api.Contracts;

public sealed class PageCalcRequest
{
    public string Region { get; set; } = "IE";   // "IE","EU","US"...
    public long BytesTransferred { get; set; }   // bytes
    public double CacheHitRate { get; set; } = 0.3; // 0..1
}

public sealed class RangeDto { public decimal Min { get; set; } public decimal Max { get; set; } }
public sealed class FactorRefDto { public string Id { get; set; } = ""; }

public sealed class PageCalcResponse
{
    public Guid RunId { get; set; }
    public decimal Co2e_g { get; set; }
    public RangeDto Range { get; set; } = new();
    public List<FactorRefDto> Factors { get; set; } = new();
    public string MethodologyVersion { get; set; } = "";
}
