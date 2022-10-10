using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PowerService : IPowerService
    {
        private static readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        private readonly Random _random = new Random();

        private readonly string _mode;

        public PowerService()
        {
            _mode = Environment.GetEnvironmentVariable("SERVICE_MODE") ?? "Normal";
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            CheckThrowError();
            Thread.Sleep(GetDelay());
            return GetTradesImpl(date);
        }

        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            CheckThrowError();
            await Task.Delay(GetDelay());
            return GetTradesImpl(date);
        }

        private void CheckThrowError()
        {
            if (_mode == "Error" || _random.Next(10) == 9)
            {
                throw new PowerServiceException("Error retrieving power volumes");
            }
        }

        private TimeSpan GetDelay()
        {
            return TimeSpan.FromSeconds(_random.NextDouble() * 5.0);
        }

        private IEnumerable<PowerTrade> GetTradesImpl(DateTime date)
        {
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified).Date.AddHours(-1.0);
            DateTime dateTime2 = dateTime.AddDays(1.0);
            DateTime dateTime3 = TimeZoneInfo.ConvertTimeToUtc(dateTime, GmtTimeZoneInfo);
            DateTime dateTime4 = TimeZoneInfo.ConvertTimeToUtc(dateTime2, GmtTimeZoneInfo);
            int numberOfPeriods = (int)dateTime4.Subtract(dateTime3).TotalHours;
            int count = ((_mode == "Test") ? 2 : _random.Next(1, 20));
            PowerTrade[] array = (from _ in Enumerable.Range(0, count)
                                  select PowerTrade.Create(date, numberOfPeriods)).ToArray();
            int num = 0;
            DateTime dateTime5 = dateTime3;
            while (dateTime5 < dateTime4)
            {
                PowerTrade[] array2 = array;
                foreach (PowerTrade obj in array2)
                {
                    double volume = ((_mode == "Test") ? ((double)(num + 1)) : (_random.NextDouble() * 1000.0));
                    obj.Periods[num].Volume = volume;
                }

                num++;
                dateTime5 = dateTime5.AddHours(1.0);
            }

            return array;
        }
    }
}
