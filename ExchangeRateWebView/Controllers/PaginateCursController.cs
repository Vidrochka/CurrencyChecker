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
            IQueryable<ExchangeRates> exchangeRates =
                _db.ExchangeRates.Where(valute => valute.ExchangeRateDate == DateTime.Today);

            if (!pageLength.HasValue || exchangeRates.Count() <= pageLength)
                return exchangeRates;

            if (!pageNumber.HasValue)
                return exchangeRates.Take(pageLength.Value);

            if (pageLength.Value * (pageNumber.Value - 1) >= exchangeRates.Count())
                return exchangeRates.TakeLast(pageLength.Value);

            return exchangeRates.Skip(pageLength.Value * (pageNumber.Value - 1)).Take(pageLength.Value);
        }
    }
}
