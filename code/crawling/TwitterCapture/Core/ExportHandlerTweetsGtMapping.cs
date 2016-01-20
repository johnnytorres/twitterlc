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
    public class ExportHandlerTweetsGtMappings : ExportHandler
    {
        public string OutputFile { get; set; }
        

        public override void Export(string path, TwitterDbContext db, int interval = 7, string gtNews="")
        {
            var query = from t in db.Users
                        //where t.ScreenName.Equals("MashiRafael") 
                        where t.Id > 0
                        orderby t.ScreenName ascending
                        select new
                        {
                            // CreatedAt = t.CreatedAt,
                            CreatedBy = t.ScreenName,
                        };


            string previousCreatedBy = "";
            DateTime previousCreatedAt = new DateTime(2015, 6, 1);
            DateTime targetDate = new DateTime(2015, 8, 23);
            //StreamWriter sw = null;
            //StreamWriter swWegiths = null;
            StreamWriter swList = new StreamWriter(OutputFile);
            

            foreach (var tweet in query)
            {
                if (!tweet.CreatedBy.Equals(previousCreatedBy))
                {
                    previousCreatedBy = tweet.CreatedBy;
                    previousCreatedAt = new DateTime(2015, 6, 1);
                }

                while (previousCreatedAt <= targetDate)
                {
                    DateTime dfrom = previousCreatedAt;
                    DateTime dto = previousCreatedAt.AddDays(interval - 1);
                    string template = "{0}{1}.D" + interval.ToString("00")
                        + dfrom.ToString(".yyyyMMdd") + dto.ToString(".yyyyMMdd") + ".tweets";
                    string filename = string.Format(template, path, tweet.CreatedBy);
                    string luserfile = string.Format(template, string.Empty, tweet.CreatedBy);
                    string nfile = string.Format(template, string.Empty, gtNews).Replace(".tweets", ".news");
                    string line = "{0},{1},{2},{3},{4},{5}";
                    //swList.WriteLine()
                    line = string.Format(line, luserfile, nfile, dfrom.ToShortDateString(), dto.ToShortDateString(), tweet.CreatedBy, gtNews);
                    swList.WriteLine(line);


                    previousCreatedAt = previousCreatedAt.AddDays(interval);
                }
               
            }

           
            swList.Close();
        }

   

        

    }
}
