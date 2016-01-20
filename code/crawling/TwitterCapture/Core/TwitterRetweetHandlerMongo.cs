using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public class TwitterRetweetHandlerMongo : TwitterRetweetHandler
    {
        public override void Download(string screenname)
        {
            DownloadRetweeters(screenname).Wait();



        }

        private async Task DownloadRetweeters(string screenname)
        {
            IMongoClient mclient;
            IMongoDatabase mdb;

            mclient = new MongoClient();
            mdb = mclient.GetDatabase("twitter");
            var tweets = mdb.GetCollection<BsonDocument>("tweets");
            var retweeters = mdb.GetCollection<BsonDocument>("retweeters");
            var filter = Builders<BsonDocument>.Filter.Regex("user.screen_name", "/^" + screenname + "$/i");
            _log.Info("processing retweeters for user {0}", screenname);

            using (var cursor = await tweets.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var tweet in batch)
                    {
                        var filter2 = Builders<BsonDocument>.Filter.Eq("tweet_id", tweet["id"]);
                        var retweetersDoc = await retweeters.Find(filter2).FirstOrDefaultAsync();
                        retweetersDoc = ProcessRetweeeters(screenname, tweet, retweetersDoc);
                        if (retweetersDoc != null)
                        {
                            retweeters.InsertOneAsync(retweetersDoc).Wait();
                            return;
                        }
                    }
                }
            }

        }

        private BsonDocument ProcessRetweeeters(string username, BsonDocument tweet, BsonDocument retweetersDoc)
        {
            if(retweetersDoc==null)
                retweetersDoc = new BsonDocument { { "tweet_id", tweet["id"] }, { "retweeters_id", new BsonArray { } }, { "last_cursor", -1 } };

            var api_path = "https://api.twitter.com/1.1/statuses/retweeters/ids.json?id={0}&count=100&cursor={1}";
            var idxcursor = retweetersDoc["last_cursor"];

            if (idxcursor == 0)
                return null;

            int numRetweeters = 0;

            while (idxcursor != 0)
            {   
                string query = string.Format(api_path, tweet["id"], idxcursor);
                var jsonObject = TwitterDownloader.ExecuteJsonGETQuery(query);
                var doc = BsonDocument.Parse(jsonObject);
                idxcursor = (int)doc["next_cursor"];

                BsonValue v= retweetersDoc["retweeters_id"].AsBsonValue;
                BsonArray a = v.AsBsonArray;
                a.AddRange(doc["ids"].AsBsonArray);
                retweetersDoc["last_cursor"] = doc["next_cursor"];
                numRetweeters += a.Count;
            }

            _log.Info("Getting {1} retweeters for tweet:{0} of user {2}", tweet["id"], numRetweeters, username);
            return retweetersDoc;
        }

    }
}
