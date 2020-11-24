using ExchangeRateSelector;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels;
using System;
using System.Linq;

namespace ExchangeRateWebView.Controllers
{
    [ApiController]
    [Route("currency")]
    public class SingleCursController : ControllerBase
    {
        private readonly ILogger<PaginateCursController> _logger;
        private readonly ExchangeRatesDdContext _db;

        public SingleCursController(ILogger<PaginateCursController> logger, ExchangeRatesDdContext db)
        {
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} is null");
            _db = db ?? throw new NullReferenceException($"{nameof(db)} is null");
        }

        [HttpGet("{CharCode}")]
        public IActionResult Get([FromRoute] string CharCode)
        {
            if (CharCode is null)
                throw new NullReferenceException($"{nameof(CharCode)} is null");

            ExchangeRates valute = _db.ExchangeRates.FirstOrDefault(valute => valute.CharCode == CharCode);

            if (valute is null)
                return NotFound();

            return new ObjectResult(valute);
        }
    }
}
