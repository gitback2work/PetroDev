using PetroLib;
using Services;

namespace ServiceTests
{
    public class TradeTests
    {
        [Fact]
        public void BaseServiceRunningTest()
        {
            Environment.SetEnvironmentVariable("SERVICE_MODE", "Test");

            var service = new PowerService();
            var lstPowerTrades = service.GetTrades(DateTime.Now).ToList();

            Assert.True(lstPowerTrades.Count() == 2);

        }

        [Fact]
        public void ErrorHandlingTest()
        {
            Environment.SetEnvironmentVariable("SERVICE_MODE", "Error");

            var service = new PowerService();
            Assert.Throws<PowerServiceException>(
                    () => service.GetTrades(DateTime.Now)
                );

        }

        [Fact]
        public void AggregationTest()
        {
            //Set mode to test so we get 2 power trades
            Environment.SetEnvironmentVariable("SERVICE_MODE", "Test");

            var service = new PowerService();
            var actualLocalTime = DateTime.Now.AddHours(-1);
            var lstPowerTrades = service.GetTrades(actualLocalTime).ToList();

            //Set power trade #1
            foreach(var period in lstPowerTrades[0].Periods) {
                period.Volume = 100;
            }

            //Set power trade #2
            foreach (var period in lstPowerTrades[1].Periods)
            {
                period.Volume = (period.Period <= 11) ? 50 : -20;
            }



            var dailyPowerPosition = new DailyPowerPosition();
            int i = 0;
            foreach (var powerPosition in dailyPowerPosition.ListPowerPositions)
            {
                foreach (var powerTrade in lstPowerTrades)
                {
                    powerPosition.Volume += powerTrade.Periods[i].Volume;
                }
                i++;
            }


            int j = 0;
            foreach (var powerPosition in dailyPowerPosition.ListPowerPositions)
            {
                if(j <= 10)
                {
                    Assert.True(powerPosition.Volume == 150);
                }
                else
                {
                    Assert.True(powerPosition.Volume == 80);
                }
                j++;
            }

        }

    }
}