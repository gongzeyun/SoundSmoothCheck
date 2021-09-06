using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    class DataBindingCollection : System.Collections.IEnumerable
    {
        public List<double> mDataList = new List<double>();

        public void addData(double value)
        {
            mDataList.Add(value);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var s in mDataList)
            {
                yield return s;
            }
        }
    }
    
}
