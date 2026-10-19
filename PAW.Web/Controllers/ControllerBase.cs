using Microsoft.AspNetCore.Mvc;
using PAW.Web.Services;

namespace PAW.Web.Controllers;

public class ControllerBase : Controller
{
    private readonly IExchangeRateService _exchangeRateService;

    private TipoCambioResponse _tipoCambio = null;
    protected TipoCambioResponse TipoCambio 
    { 
        get
        {
            return _tipoCambio ?? _exchangeRateService.GetExchangeRateAsync().Result;
        } 
    } 

    public ControllerBase(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }
}
