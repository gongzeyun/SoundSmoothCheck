using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCheck
{
    interface ErrorReportListener
    {
        void onErrorReport(ErrorContainer errorContainer);
    }
}
