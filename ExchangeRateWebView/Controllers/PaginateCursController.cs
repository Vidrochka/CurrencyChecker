using ExchangeRateSelector;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRateWebView.Controllers
{
    [ApiController]
    [Route("currencies")]
    public class PaginateCursController : ControllerBase
    {
        private readonly ILogger<PaginateCursController> _logger;
        private readonly ExchangeRatesDdContext _db;

        public PaginateCursController(ILogger<PaginateCursController> logger, ExchangeRatesDdContext db)
        {
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} is null");
            _db = db ?? throw new NullReferenceException($"{nameof(db)} is null");
        }

        [HttpGet("{pageLength?}/{pageNumber?}")]
        public IEnumerable<ExchangeRates> Get(int? pageLength, int? pageNumber)
        {
            if (!pageLength.HasValue || _db.ExchangeRates.Count() <= pageLength)
                return _db.ExchangeRates;

            if (!pageNumber.HasValue)
                return _db.ExchangeRates.Take(pageLength.Value);

            if (pageLength.Value * (pageNumber.Value - 1) >= _db.ExchangeRates.Count())
                return _db.ExchangeRates.TakeLast(pageLength.Value);

            return _db.ExchangeRates.Skip(pageLength.Value * (pageNumber.Value - 1)).Take(pageLength.Value);
        }
    }
}
