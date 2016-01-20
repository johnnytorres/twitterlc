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
    public class Exporter
    {
        static Logger log = NLog.LogManager.GetCurrentClassLogger();



        private static void ExportUsersToMongo()
        {
            IMongoClient mclient;
            IMongoDatabase mdb;

            mclient = new MongoClient();
            mdb = mclient.GetDatabase("twitter");
            var collection = mdb.GetCollection<BsonDocument>("users");

            using (TwitterDbContext db = new TwitterDbContext())
            {
                foreach (var user in db.Users)
                {
                    //if (user.Json == null)
                    //    continue;

                    //var doc = BsonDocument.Parse(user.Json);
                    //collection.InsertOneAsync(doc).Wait();
                }
            }
        }

        private void ExportTweetsToMongo()
        {
            //IMongoClient mclient;
            //IMongoDatabase mdb;

            //mclient = new MongoClient();
            //mdb = mclient.GetDatabase("twitter");
            //var collection = mdb.GetCollection<BsonDocument>("tweets");

            //using (TwitterDbContext db = new TwitterDbContext())
            //{
            //    foreach (var tweet in db.Tweets)
            //    {
            //        if (string.IsNullOrEmpty(tweet.Json))
            //            continue;

            //        var doc = BsonDocument.Parse(tweet.Json);
            //        collection.InsertOneAsync(doc).Wait();

            //    }
            //}
        }

        private static void ExportTweetsToFile()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "Tweets" + Path.DirectorySeparatorChar;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (TwitterDbContext db = new TwitterDbContext())
            {
                foreach (var tweet in db.Tweets)
                {
                    string userdir = dir + tweet.ProfileId + Path.DirectorySeparatorChar;
                    string fname = userdir + tweet.Id + ".txt";
                    //File.AppendAllText(fname, tweet.Json);
                }
            }
        }

        private static void ExportUsers()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "Profiles" + Path.DirectorySeparatorChar;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (TwitterDbContext db = new TwitterDbContext())
            {
                foreach (var user in db.Users)
                {
                    string fname = dir + user.Id + ".txt";
                    //File.AppendAllText(fname, user.Json);
                }
            }
        }

        public static void ExportData(string[] args)
        {
            string path = args[1] + Path.DirectorySeparatorChar;

            using (TwitterDbContext db = new TwitterDbContext())
            {
                //for (int i = 7; i <= 7; i++)
                //{
                //ExportHandler newsExporter = new ExportHandlerNewsByUserDate();
                //newsExporter.Export(path, db, 7, "Universo");
                //newsExporter.Export(path, db, 7, "Telegrafo");
                ExportHandler tweetsExporter = new ExportHandlerTweetsByUserDate();
                tweetsExporter.Export(path, db, 7, "Telegrafo");
                //ExportHandlerTweetsByUserDateGrouped exporter = new ExportHandlerTweetsByUserDateGrouped();
                //exporter.Export(path, db, 90);


                //ExportHandlerTweetsGtMappings mappings = new ExportHandlerTweetsGtMappings();
                //mappings.OutputFile = @"H:\dev\twitter\Universo.mappings";
                //mappings.Export(path, db, 7, "Universo");
                //mappings.OutputFile = @"H:\dev\twitter\Telegrafo.mappings";
                //mappings.Export(path, db, 7, "Telegrafo");
            }
        }

  

    }
}
