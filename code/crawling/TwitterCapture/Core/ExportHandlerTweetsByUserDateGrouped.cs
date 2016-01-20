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
    public class ExportHandlerTweetsByUserDateGrouped : ExportHandler
    {



        public override void Export(string path, TwitterDbContext db, int interval = 7, string filter = "")
        {
            var query = from t in db.Tweets
                        
                        join p in db.Users on t.ProfileId equals p.Id
                        where t.CreatedBy.Equals("MashiRafael")
                        where t.CreatedAt >= new DateTime(2015, 7, 1)
                        && p.IsLeader && !t.Text.StartsWith("RT @") && t.RetweetCount > 2
                        orderby t.CreatedBy ascending, t.CreatedAt ascending
                        select new
                        {
                            CreatedAt = t.CreatedAt,
                            CreatedBy = t.CreatedBy,
                            Text = t.Text,
                        };


            string previousCreatedBy = "";
            DateTime previousCreatedAt = new DateTime(2015, 7, 1);
            StreamWriter sw = null;
            string tweetText = string.Empty;

            foreach (var tweet in query)
            {
                if (!tweet.CreatedBy.Equals(previousCreatedBy))
                {
                    previousCreatedBy = tweet.CreatedBy;
                    previousCreatedAt = new DateTime(2015, 7, 1);

                    if (sw != null)
                    {
                        sw.Close();
                       

                        tweetText = string.Empty;
                    }

                    string filename = path + tweet.CreatedBy + ".tweets";
                    sw = new StreamWriter(filename);
                }

                //log.Info("processing tweets by {0} from {1}", tweet.CreatedBy, tweet.CreatedAt);
                Console.WriteLine("processing tweets by {0} from {1}", tweet.CreatedBy, tweet.CreatedAt);

                ExportTweet(sw, tweet.Text);
                //tweetText += tweet.Text;

            }

            sw.Close();
        }

    }
}
