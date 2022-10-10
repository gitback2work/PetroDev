namespace AgroService
{
    public class TimedWorker : IHostedService, IDisposable
    {
        private readonly ILogger<TimedWorker> _logger;
        private readonly ICalcService _calcService;
        private Timer _timer;
        private int _counter = 0;
        private static object _lock = new object();
        private readonly IConfiguration _config;


        public TimedWorker(ILogger<TimedWorker> logger, IConfiguration config, ICalcService calcService)
        {
            _logger = logger;
            _config = config;
            _calcService = calcService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int refreshInterval = _config.GetValue<int>("PowerService:RefreshInterval");
            bool useSeconds = _config.GetValue<bool>("PowerService:UseSeconds");

            if(useSeconds)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(refreshInterval));
            }
            else
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(refreshInterval));
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    _calcService.DoWorkAsync().Wait();
                }
                finally
                {
                    _counter++;
                    Monitor.Exit(_lock);
                }
            }
        }


    }
}