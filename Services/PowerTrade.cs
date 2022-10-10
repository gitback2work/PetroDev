using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PowerTrade
    {
        public DateTime Date { get; internal set; }

        public PowerPeriod[] Periods { get; internal set; }

        public static PowerTrade Create(DateTime date, int numberOfPeriods)
        {
            return new PowerTrade
            {
                Date = date,
                Periods = (from period in Enumerable.Range(1, numberOfPeriods)
                           select new PowerPeriod
                           {
                               Period = period
                           }).ToArray()
            };
        }
    }
}
