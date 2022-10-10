using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroLib
{
    public class DailyPowerPosition
    {
        public List<PowerPosition> ListPowerPositions { get; set; }

        public DailyPowerPosition()
        {
            ListPowerPositions = new List<PowerPosition>();

            var localTime = DateTime.Today.AddHours(-1); //Doesn't need to be today but today works fine

            for (int i = 0; i< 24; i++)
            {
                var powerPosition = new PowerPosition { LocalTime = localTime };
                ListPowerPositions.Add(powerPosition);
                localTime = localTime.AddHours(1);
            }
        }
    }
}
