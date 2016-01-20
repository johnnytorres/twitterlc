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
        public override void Download(List<string> users)
        {
            // get user profiles

            var api_path = "https://api.twitter.com/1.1/users/show.json?screen_name={0}&include_entities={1}";

            IMongoClient mclient;
            IMongoDatabase mdb;

            mclient = new MongoClient();
            mdb = mclient.GetDatabase("twitter");
            var collection = mdb.GetCollection<BsonDocument>("users");

            foreach (string u in users)
            {
                try
                {
                    _log.Info("Getting profile info for user:{0} ...", u);
                    string query = string.Format(api_path, u, true);
                    var jsonObject = TwitterAccessor.ExecuteJsonGETQuery(query);
                    var filter = Builders<BsonDocument>.Filter.Eq("screen_name", u);
                    BsonDocument doc = BsonDocument.Parse(jsonObject);
                    collection.FindOneAndReplaceAsync(filter, doc).Wait();
                }
                catch(Exception ex)
                {
                    string msg = "Error trying to download profile for user:{0}";
                    msg=string.Format(msg, u);
                    throw new Exception(msg, ex);
                }
          
        }
    }
}
