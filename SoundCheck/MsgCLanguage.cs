using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    static class MsgCLanguage
    {
        public const int CMD_RECORD_STARTED = 0; //must same with C languae define
        public const int CMD_RECORD_DATA_AVALIABLE = 1;
        public const int CMD_RECORD_CLOSED = 2;
    }
}
