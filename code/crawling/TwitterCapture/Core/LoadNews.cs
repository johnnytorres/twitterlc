using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture.Core
{
    public class NewsHandler
    {
        public static void LoadNews(string filename)
        {

            using (TwitterDbContext db = new TwitterDbContext())
            {
                long? minId = (from t in db.Tweets where t.ProfileId == -1 select (long?)t.Id).Min();

                if (!minId.HasValue)
                    minId = 0;

                using (StreamReader sr = new StreamReader(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        string title = sr.ReadLine();
                        string[] tokens = title.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length != 3)
                            continue;

                        TwitterTweet tweet = new TwitterTweet { Id = (--minId).Value, ProfileId = -2, CreatedBy = tokens[0], CreatedAt = DateTime.Parse(tokens[1]), Text = tokens[2] };
                        db.Entry<TwitterTweet>(tweet).State = System.Data.Entity.EntityState.Added;
                    }


                }

                db.SaveChanges();
            }


        }
    }
}
