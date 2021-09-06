using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    [Serializable]
    class SerializablePoint
    {
       public double mXValue;
       public double mYValue;

       public SerializablePoint(double xValue, double yValue)
       {
            mXValue = xValue;
            mYValue = yValue;
       }
    }
}
