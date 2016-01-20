using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitterCapture.Core
{
    public class ExportHandlerNewsByUserDate : ExportHandler
    {
        
        

        public override void Export(string path, TwitterDbContext db, int interval = 7, string filter="")
        {
            var query = from t in db.Tweets
                        where t.CreatedBy.Equals(filter) && t.CreatedAt >= new DateTime(2015, 6, 1) && t.ProfileId < 0
                        orderby t.CreatedBy ascending, t.CreatedAt ascending
                        select new
                        {
                            CreatedAt = t.CreatedAt,
                            CreatedBy = t.CreatedBy,
                            Text = t.Text,
                            Retweets = t.RetweetCount
                        };


            string previousCreatedBy = "";
            DateTime previousCreatedAt = new DateTime(2015, 6, 1);
            StreamWriter sw = null;
            

            foreach (var tweet in query)
            {
                if (!tweet.CreatedBy.Equals(previousCreatedBy))
                {
                    previousCreatedBy = tweet.CreatedBy;
                    previousCreatedAt = new DateTime(2015, 6, 1);
                }

                if (tweet.CreatedAt.Date >= previousCreatedAt)
                {
                    log.Info("processing tweets by {0} from {1}", tweet.CreatedBy, tweet.CreatedAt);
                    string filename = path + tweet.CreatedBy + ".D" +interval.ToString("00")
                        + previousCreatedAt.ToString(".yyyyMMdd") + previousCreatedAt.AddDays(interval-1).ToString(".yyyyMMdd") + ".news";


                    if (sw != null)
                        sw.Close();

                    sw = new StreamWriter(filename);
                    previousCreatedAt = previousCreatedAt.AddDays(interval);


                }

                string tweetText = tweet.Text;
                ExportTweet(sw, tweetText);
            }

            sw.Close();
        }

   

        

    }
}
