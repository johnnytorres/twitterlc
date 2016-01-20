using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public sealed class TwitterDownloader
    {
        public static string ExecuteJsonGETQuery(string query)
        {
            int retryAttempt = 0;

            while (retryAttempt++ < 3)
            {

                RateLimit.RateLimitTrackerOption = Tweetinvi.Core.RateLimitTrackerOptions.TrackAndAwait;
                var jsonObject = TwitterAccessor.ExecuteJsonGETQuery(query);

                //if (!string.IsNullOrEmpty(jsonObject))
                    return jsonObject;

                var limitapi = "https://api.twitter.com/1.1/application/rate_limit_status.json";
                var limitjson = TwitterAccessor.ExecuteJsonGETQuery(limitapi);

                Thread.Sleep(2000);
            }

            return null;
        }
    }
}
