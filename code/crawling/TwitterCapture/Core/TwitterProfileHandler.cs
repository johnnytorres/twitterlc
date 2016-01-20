using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public abstract class TwitterProfileHandler
    {
        protected Logger _log = NLog.LogManager.GetCurrentClassLogger();


        public abstract void Download(string user);


        public abstract string Download(long userid);
       
    }
}
