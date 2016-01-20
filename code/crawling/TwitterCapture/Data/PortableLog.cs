using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Credentials;

namespace TwitterCapture
{
    public class PortableLog //: ILog
    {
        public Logger Log { get; set; }

        public void Debug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }
    }
}
