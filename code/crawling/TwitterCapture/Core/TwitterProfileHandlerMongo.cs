using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public class TwitterProfileHandlerMongo : TwitterProfileHandler
    {
        public override void Download(string user)
        {
            // get user profiles

            var api_path = "https://api.twitter.com/1.1/users/show.json?screen_name={0}&include_entities={1}";
            string query = string.Format(api_path, user, true);

            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var collection = mdb.GetCollection<BsonDocument>("users");

            try
            {
                _log.Info("Getting profile info for user:{0} ...", user);     
                var jsonObject = TwitterDownloader.ExecuteJsonGETQuery(query);
                var filter = Builders<BsonDocument>.Filter.Regex("screen_name", "/^" + user + "$/i");
                var option = new FindOneAndReplaceOptions<BsonDocument>();
                option.IsUpsert = true;
                BsonDocument doc = BsonDocument.Parse(jsonObject);
                collection.FindOneAndReplaceAsync(filter, doc, option).Wait();
            }
            catch (Exception ex)
            {
                string msg = "Error trying to download profile for user:{0}";
                msg = string.Format(msg, user);
                throw new Exception(msg, ex);
            }

        }

        public override string Download(long userid)
        {
            var api_path = "https://api.twitter.com/1.1/users/show.json?user_id={0}&include_entities={1}";
            string query = string.Format(api_path, userid, true);

            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var collection = mdb.GetCollection<BsonDocument>("users");

            try
            {
                _log.Info("Getting profile info for user:{0} ...", userid);
                var jsonObject = TwitterDownloader.ExecuteJsonGETQuery(query);
                var filter = Builders<BsonDocument>.Filter.Eq("id", userid);
                var option = new FindOneAndReplaceOptions<BsonDocument>();
                option.IsUpsert = true;
                BsonDocument doc = BsonDocument.Parse(jsonObject);
                collection.FindOneAndReplaceAsync(filter, doc, option).Wait();
                return doc["screen_name"].AsString;
            }
            catch (Exception ex)
            {
                string msg = "Error trying to download profile for user:{0}";
                msg = string.Format(msg, userid);
                throw new Exception(msg, ex);
            }
        }
    }
}
