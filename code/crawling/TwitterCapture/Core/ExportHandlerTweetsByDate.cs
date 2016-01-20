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
    public class ExportHandlerTweetsByDate : ExportHandler
    {




        public override void Export(string path, TwitterDbContext db, int interval = 7, string filter = "")
        {
            var query = from t in db.Tweets
                        where t.CreatedBy.Equals("MashiRafael") 
                        where t.CreatedAt >= new DateTime(2015, 6, 1)
                        orderby t.CreatedBy ascending, t.CreatedAt ascending
                        select new
                        {
                            CreatedAt = t.CreatedAt,
                            CreatedBy = t.CreatedBy,
                            Text = t.Text,
                        };


            string previousFilename = "";
            DateTime previousCreatedAt = new DateTime(2015, 6, 1);
            DateTime refCreatedAt = previousCreatedAt.AddDays(14);
            string filename = path + refCreatedAt.Date.ToString("yyyyMMdd") + ".tweets";
            StreamWriter sw = new StreamWriter(filename);
            

            foreach (var tweet in query)
            {
                if (tweet.CreatedAt.Date >= refCreatedAt)
                {
                    log.Info("processing tweets from {0]", tweet.CreatedAt);
                    refCreatedAt.AddDays(14);
                    previousCreatedAt = tweet.CreatedAt.Date;
                    filename = path + refCreatedAt.Date.ToString("yyyyMMdd") + ".tweets";


                    if (!filename.Equals(previousFilename))
                    {
                        if (sw != null)
                            sw.Close();

                        sw = new StreamWriter(filename);
                        previousFilename = filename;
                    }
                }

                string tweetText = tweet.Text;
                ExportTweet(sw, tweetText);
            }

            sw.Close();
        }

       

    }
}
