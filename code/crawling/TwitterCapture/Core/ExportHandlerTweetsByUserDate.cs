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
    public class ExportHandlerTweetsByUserDate : ExportHandler
    {



        public override void Export(string path, TwitterDbContext db, int interval = 7, string filter = "")
        {
            var query = from t in db.Tweets
                        join p in db.Users on t.ProfileId equals p.Id
                         //where t.CreatedBy.Equals(filter) 
                        where t.CreatedAt >= new DateTime(2015, 6, 1) 
                        && t.CreatedAt <= new DateTime(2015, 8,23)
                        && t.ProfileId > 0
                        && p.IsLeader  
                        && !t.Text.StartsWith("RT @") //&& t.RetweetCount > 2 
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
            StreamWriter swWegiths = null;
            StreamWriter swStats = new StreamWriter(path+"stats.txt");
            long numRetweets = 0;
            //StreamWriter swList = new StreamWriter("FileList.txt");
            

            foreach (var tweet in query)
            {
                if (!tweet.CreatedBy.Equals(previousCreatedBy))
                {
                    previousCreatedBy = tweet.CreatedBy;

                    
                    previousCreatedAt = new DateTime(2015, 6, 1);

                    while (tweet.CreatedAt.AddDays(-interval) > previousCreatedAt)
                        previousCreatedAt=previousCreatedAt.AddDays(interval);
                    
                    numRetweets = 0;
                    if (sw != null)
                    {
                        sw.Close();
                        swWegiths.Close();
                        sw = null;
                        swWegiths = null;
                    }
                }

                if (tweet.CreatedAt.Date >= previousCreatedAt)
                {
                    Console.WriteLine("processing tweets by {0} from {1}", tweet.CreatedBy, tweet.CreatedAt);
                    DateTime dfrom = previousCreatedAt;
                    DateTime dto = previousCreatedAt.AddDays(interval - 1);
                    string template = "{0}{1}.D" +interval.ToString("00")
                        + dfrom.ToString(".yyyyMMdd") + dto.ToString(".yyyyMMdd") + ".tweets";
                    string filename = string.Format(template, path, tweet.CreatedBy);
                    string luserfile = string.Format(template, string.Empty, tweet.CreatedBy);
                    string nfile = string.Format(template, string.Empty, filter).Replace(".tweets", ".news");
                    //string line = "{0},{1},{2},{3},{4},{5}";
                    //swList.WriteLine()
                    //line = string.Format(line, luserfile, nfile, dfrom.ToShortDateString(), dto.ToShortDateString(), tweet.CreatedBy, gtNews);
                    //swList.WriteLine(line);

                    if (sw != null)
                    {
                        string stat = previousCreatedBy + "," + previousCreatedAt.AddDays(-7).ToShortDateString() + "," + numRetweets;
                        swStats.WriteLine(stat);
                        numRetweets = 0;
                        sw.Close();
                        swWegiths.Close();
                    }

                    sw = new StreamWriter(filename);
                    swWegiths = new StreamWriter(filename + ".weights");
                    

                    previousCreatedAt = previousCreatedAt.AddDays(interval);


                }

                numRetweets += tweet.Retweets;
                string tweetText = tweet.Text;
                ExportTweet(sw, tweetText, swWegiths, tweet.Retweets);
            }

            sw.Close();
            swWegiths.Close();
            swStats.Close();
        }

   

        

    }
}
