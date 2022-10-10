using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PowerServiceException : Exception
    {
        public PowerServiceException(string message)
            : base(message)
        {
        }
    }
}
