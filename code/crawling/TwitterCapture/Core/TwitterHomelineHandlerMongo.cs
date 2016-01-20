using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class TwitterHomelineHandlerMongo : TwitterHomelineHandler
    {
        string api_path = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&count=200";
        public int MaxTweets { get; set; }



        public override void Download(string user)
        {
            DownloadTimelineAsync(user).Wait();

        }

        private async Task DownloadTimelineAsync(string user)
        {   
            IMongoClient mclient;
            IMongoDatabase mdb;

            mclient = new MongoClient();
            mdb = mclient.GetDatabase("twitter");
            var tweets = mdb.GetCollection<BsonDocument>("tweets");
            await GetHistoricalTweets(user, tweets);
            //await GetFutureTweets(user, tweets);
        }

        private async Task GetFutureTweets(string user, IMongoCollection<BsonDocument> tweets)
        {
            int tweetsCount = 0, tweetsProcessed = 0, tweetsMax = MaxTweets;
            var filter = GetFilter(user);
            var aggdef = tweets.Aggregate()
                .Match(filter)
                .Group(new BsonDocument { { "_id", "$user.screen_name" }, { "max", new BsonDocument("$max", "$id") } });
            var aggresult = await aggdef.FirstOrDefaultAsync();
            var maxid = aggresult["max"].AsNullableInt64;

          


            do
            {
                var query = string.Format(api_path, user);

                if (maxid.HasValue)
                {
                    query = query + "&since_id={0}";
                    query = string.Format(query, maxid + 1);
                }

                var jsonObject = TwitterDownloader.ExecuteJsonGETQuery(query);
                List<object> otweets = (List<object>)JsonConvert.DeserializeObject<List<object>>(jsonObject);
                tweetsCount = otweets.Count;
                tweetsProcessed += tweetsCount;
                _log.Info("Getting {0} new tweets for user:{1}", otweets.Count, user);

                foreach (object oTweet in otweets)
                {
                    var bson = BsonDocument.Parse(oTweet.ToString());
                    tweets.InsertOneAsync(bson).Wait();
                    long? nmax = bson["id"].AsNullableInt64;
                    if (nmax > maxid)
                        maxid = nmax;
                }
            }
            while (tweetsCount > 0 && tweetsProcessed < tweetsMax);
        }



        private async Task GetHistoricalTweets(string user, IMongoCollection<BsonDocument> tweets)
        {
            int tweetsCount = 0, tweetsProcessed = 0, tweetsMax = MaxTweets;
            //var filter = GetFilter(user);
            //var aggdef = tweets.Aggregate()
            //    .Match(filter)
            //    .Group(new BsonDocument { { "_id", "$user.screen_name" }, { "min", new BsonDocument("$min", "$id") } });
            //var aggresult = await aggdef.FirstOrDefaultAsync();
            //var minid = aggresult != null ? aggresult["min"].AsNullableInt64 : null;

           
            //do
            //{
                var query = string.Format(api_path, user);

                //if (minid.HasValue)
                //{
                //    query = query + "&max_id={0}";
                //    query = string.Format(query, minid - 1);
                //}

                var jsonObject = TwitterDownloader.ExecuteJsonGETQuery(query);

                if (jsonObject == null)
                    return;

                List<object> otweets = (List<object>)JsonConvert.DeserializeObject<List<object>>(jsonObject);
                tweetsCount = otweets.Count;
                tweetsProcessed += tweetsCount;
                Console.WriteLine("Getting {0} old tweets for user:{1}", otweets.Count, user);

                foreach (object oTweet in otweets)
                {
                    var bson = BsonDocument.Parse(oTweet.ToString());
                    tweets.InsertOneAsync(bson).Wait();
                    //minid = bson["id"].AsNullableInt64;
                }
            //}
            //while (tweetsCount > 0 && tweetsProcessed < tweetsMax);
        }


        private static FilterDefinition<BsonDocument> GetFilter(string user)
        {
            var filter = Builders<BsonDocument>.Filter.Regex("user.screen_name", "/^" + user + "$/i");
            return filter;
        }
    }
}
