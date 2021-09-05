using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    interface UIOwner
    {
        void UpdateUIAccordMsg(int msgType, object msgObject);
    }
}
