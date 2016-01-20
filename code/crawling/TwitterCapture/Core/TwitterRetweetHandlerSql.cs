using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public class TwitterRetweetHandlerSql : TwitterRetweetHandler
    {
        public override void Download(string screenname)
        {
            using (TwitterDbContext db = new TwitterDbContext())
            {
                foreach (var user in db.Users)
                {
                    var queryResult = (from t in db.Tweets where t.ProfileId == user.Id  select t).OrderByDescending(i => i.Id);

                    foreach (TwitterTweet tweet in queryResult)
                    {
                        //int numberOfObjectsPerPage = 10;
                        //var queryResultPage = queryResult.Skip(numberOfObjectsPerPage * 1).Take(numberOfObjectsPerPage);

                        var cursor = -1;
                        //var count = 100;
                        var api_path = "https://api.twitter.com/1.1/statuses/retweeters/ids.json?id={0}&cursor={1}&stringify_ids=true";

                        while (cursor != 0)
                        {
                            //int page = i / pagesize + 1;
                            //var results = TwitterAccessor.ExecuteCursorGETCursorQueryResult<IIdsCursorQueryResultDTO>(query, 100, page );
                            //var userIds = results.SelectMany(x => x.Ids);
                            //var retweeters = TwitterAccessor.ExecuteCursorGETQuery<long, IIdsCursorQueryResultDTO>(query, 100);
                            string query = string.Format(api_path, tweet.Id, cursor);
                            var jsonObject = TwitterAccessor.GetQueryableJsonObjectFromGETQuery(query);

                            if (jsonObject != null)
                            {
                                cursor = (int)jsonObject["next_cursor"];
                                var ids = jsonObject["ids"];

                                //tweet.RetweetersIds += ids.ToString();
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }
        }

    }
}
