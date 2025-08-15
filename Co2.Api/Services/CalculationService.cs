using Co2.Api.Contracts;
using Co2Calc.Engine;

namespace Co2.Api.Services;

public sealed class CalculationService(IMethodology methodology, IFactorProvider factors) : ICalculationService
{
    private readonly IMethodology _methodology = methodology;
    private readonly IFactorProvider _factors = factors;

    public PageCalcResponse CalculatePage(PageCalcRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Region))
            throw new ArgumentException("Region required", nameof(req.Region));
        if (req.BytesTransferred < 0)
            throw new ArgumentOutOfRangeException(nameof(req.BytesTransferred));
        if (req.CacheHitRate is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(req.CacheHitRate));

        var input = new CalcInput(req.Region, req.BytesTransferred, req.CacheHitRate);
        var r = _methodology.Calculate(input, _factors);

        return new PageCalcResponse
        {
            RunId = Guid.NewGuid(),
            Co2e_g = r.Co2eGrams,
            Range = new RangeDto { Min = r.MinGrams, Max = r.MaxGrams },
            Factors = r.FactorIds.Select(id => new FactorRefDto { Id = id }).ToList(),
            MethodologyVersion = $"{_methodology.Id}:{_methodology.Version}"
        };
    }
}
