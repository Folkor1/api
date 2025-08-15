using Co2.Api.Contracts;

namespace Co2.Api.Services;

public interface ICalculationService
{
    PageCalcResponse CalculatePage(PageCalcRequest req);
}
