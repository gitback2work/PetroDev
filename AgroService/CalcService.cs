using PetroLib;
using Services;
using System.Text;

namespace AgroService
{
    public interface ICalcService
    {
        Task DoWorkAsync();
    }
    public class CalcService:ICalcService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CalcService> _logger;

        public CalcService(ILogger<CalcService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        async Task ICalcService.DoWorkAsync()
        {            
            //Besides running tests, it was helpful in development to be able to set the environment variable here
            //In production, we can omit the "ServiceMode" setting
            var serviceMode = _config.GetValue<string>("PowerService:ServiceMode");
            if(!string.IsNullOrEmpty(serviceMode))
            {
                Environment.SetEnvironmentVariable("SERVICE_MODE", serviceMode);
            }            

            var folder = _config.GetValue<string>("PowerService:CSVLocation");
            var timeStamp = DateTime.Now;
            var filename = $"{folder}\\PowerPosition_{timeStamp:yyyyMMdd_HHmm}.csv";


            bool success = false;

            //If at first you don't succeed, try, try again
            while(!success)
            {
                try
                {
                    var service = new PowerService();
                    var actualLocalTime = DateTime.Now.AddHours(-1); 
                    
                    var lstPowerTrades = service.GetTrades(actualLocalTime).ToList();

                    
                    var dailyPowerPosition = new DailyPowerPosition();

                    int i = 0;
                    foreach (var powerPosition in dailyPowerPosition.ListPowerPositions)
                    {
                        foreach(var powerTrade in lstPowerTrades)
                        {
                            powerPosition.Volume += powerTrade.Periods[i].Volume;
                        }                        
                        i ++;
                    }


                    var sb = new StringBuilder();
                    sb.AppendLine($"\"Local Time\",\"Volume\"");
                    foreach(var powerPosition in dailyPowerPosition.ListPowerPositions)
                    {
                        var record = $"\"{powerPosition.LocalTime:HH:mm}\",\"{powerPosition.Volume}\"";
                        sb.AppendLine(record);
                    }

                    Directory.CreateDirectory(folder);
                    File.WriteAllText(filename, sb.ToString());

                    _logger.LogInformation($"{lstPowerTrades.Count()} power trades retrieved successfully");
                    success = true;
                }
                catch (PowerServiceException pse)
                {
                    _logger.LogWarning(pse, "PowerService unavailable. Trying again in 5 seconds");
                    success = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unknown exception getting power trades");
                    throw;
                }

                if (success) continue;

                //wait 5 seconds before trying again
                await Task.Delay(TimeSpan.FromSeconds(5));

            }


            return;


        }

    }
}
