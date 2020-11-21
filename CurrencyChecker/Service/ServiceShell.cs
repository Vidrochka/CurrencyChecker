using CurrencyChecker.DTO;
using CurrencyChecker.Exceptions;
using CurrencyChecker.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyChecker.Service
{
    public class ServiceShell : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceShell> _logger;

        private readonly DbWorker _db;
        private readonly WebWorker _web;
        private readonly ResponseParser _parser;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ManualResetEventSlim _workSignal;

        private readonly Task _worker;

        public long LoadPeriodInSeconds => Convert.ToInt64(TimeSpan.Parse("24:00:00", CultureInfo.InvariantCulture).TotalSeconds);

        public ServiceShell(IConfiguration configuration, ILogger<ServiceShell> logger, DbWorker db, WebWorker web, ResponseParser parser)
        {
            _configuration = configuration;
            _logger = logger;
            _db = db;
            _web = web;
            _parser = parser;

            _cancellationTokenSource = new CancellationTokenSource();
            _workSignal = new ManualResetEventSlim(false);

            _worker = new Task(() => Worker(_cancellationTokenSource.Token));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                long checkTimeInSeconds = _configuration.GetCurrencyLoadTime();
                long firstLoadInMilliseconds = 0;


                if (DateTime.Now > DateTime.Today.AddSeconds(checkTimeInSeconds))
                {
                    TimeSpan timeBeforeFirstWork =
                        DateTime.Today.AddDays(1).AddSeconds(checkTimeInSeconds) - DateTime.Now;
                    firstLoadInMilliseconds = Convert.ToInt64(timeBeforeFirstWork.TotalMilliseconds);

                    _logger.LogInformation($"Now {DateTime.Now:g} more than " +
                                           $"{DateTime.Today.AddSeconds(checkTimeInSeconds):g}. " +
                                           $"Work after {timeBeforeFirstWork:g}");
                }
                else
                {
                    TimeSpan timeBeforeFirstWork = DateTime.Today.AddSeconds(checkTimeInSeconds) - DateTime.Now;
                    firstLoadInMilliseconds = Convert.ToInt64(timeBeforeFirstWork.TotalMilliseconds);

                    _logger.LogInformation($"Now {DateTime.Now:g} less than " +
                                           $"{DateTime.Today.AddSeconds(checkTimeInSeconds):g}. " +
                                           $"Work after {timeBeforeFirstWork:g}");
                }

                _timer = new Timer(_ => _workSignal.Set(), null, firstLoadInMilliseconds, LoadPeriodInSeconds);

                _logger.LogInformation($"Next load after {firstLoadInMilliseconds}ms");

                if (_configuration.GetNeedImmediateLoad())
                    _workSignal.Set();

                _worker.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Service not started: {ex}");
            }

            return Task.CompletedTask;
        }

        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                _workSignal.Wait(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    ValCurs curs = await RequestCursAsync(cancellationToken).ConfigureAwait(false);

                    if (curs is null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        _logger.LogError($"Service cant get exchange rates");
                        throw new ExchangeRateRequestException("Service cant get exchange rates");
                    }

                    bool dBWriteState = await SaveCursAsync(curs, cancellationToken);

                    if (!dBWriteState)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        _logger.LogError($"Service cant store exchange rates");
                        throw new ExchangeRateRequestException("Service cant store exchange rates");
                    }

                    _workSignal.Reset();
                    _workSignal.Wait(cancellationToken);
                }

                _logger.LogInformation("Work thread stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something wrong in service work thread: {ex}");
            }
        }

        public async Task<ValCurs> RequestCursAsync(CancellationToken cancellationToken)
        {
            ValCurs curs = null;
            foreach (int i in Enumerable.Range(0, _configuration.GetRetryRequestCount()))
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                await using Stream responseStream
                    = await _web.RequestNewCurrencyAsync(DateTime.Now).ConfigureAwait(false);

                if (responseStream is null)
                {
                    try
                    {
                        _logger.LogInformation("Wait request timeout");
                        await Task.Delay(_configuration.GetRetryRequestWaitTime(),
                            _cancellationTokenSource.Token);
                        _logger.LogInformation("Request timeout end");

                        continue;
                    }
                    catch (TaskCanceledException)
                    {
                        return null;
                    }
                }

                curs = await _parser.ParseCursAsync(responseStream).ConfigureAwait(false);

                if (curs is null)
                {
                    try
                    {
                        _logger.LogInformation("Wait parse timeout");
                        await Task.Delay(_configuration.GetRetryRequestWaitTime(),
                            _cancellationTokenSource.Token);
                        _logger.LogInformation("Parse timeout end");

                        continue;

                    }
                    catch (TaskCanceledException)
                    {
                        return null;
                    }
                }

                break;
            }

            return curs;
        }

        public async Task<bool> SaveCursAsync(ValCurs curs, CancellationToken cancellationToken)
        {
            bool dBWriteState = false;
            foreach (int i in Enumerable.Range(0, _configuration.GetRetryStoreCurrencyCount()))
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                dBWriteState = await _db.StoreCurrencyAsync(curs).ConfigureAwait(false);

                if (!dBWriteState)
                {
                    try
                    {
                        _logger.LogInformation("Wait db save timeout");
                        await Task.Delay(_configuration.GetRetryStoreCurrencyWaitTime(),
                            _cancellationTokenSource.Token);
                        _logger.LogInformation("DB timeout end");

                        continue;
                    }
                    catch (TaskCanceledException)
                    {
                        return false;
                    }

                }

                break;
            }

            return dBWriteState;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _logger.LogInformation($"Stop requested");

            _workSignal.Set();

            Task.WaitAll(_worker);
            _logger.LogInformation($"Service stopped");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}