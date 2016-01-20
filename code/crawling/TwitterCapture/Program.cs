using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tweetinvi;
using Tweetinvi.Core;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces.DTO.QueryDTO;
using Tweetinvi.Core.Parameters;
using TwitterCapture.Core;

namespace TwitterCapture
{
    class Program
    {
        static Logger log = NLog.LogManager.GetCurrentClassLogger();
        static ConsoleKey key = ConsoleKey.A;
       
        static void Main(string[] args)
        {
            try
            {
                //string cmd = args[0];

                //if (cmd.Equals("-export"))
                //{
                   //Exporter.ExportData(args);
                //}

                //else if (cmd.Equals("-loadNews"))
                //{
                   // NewsHandler.LoadNews(@"H:\dev\twitter\eltelegrafo2.txt");
                //}

                //else if (cmd.StartsWith("-capture"))
                //{
                //    log.Trace("initializing twitter capture...[ok]");
                //    PortableLog plog = new PortableLog();
                //    plog.Log = log;
                //    Tweetinvi.Credentials.RateLimit.RateLimitAwaiter.Logger = plog;
                //    Thread.CurrentThread.Name = "main";
                //    GetTwitterData(cmd).Wait();
                //}
                //else if (cmd.Equals("-import"))
                //{
                //    //LoadFromMongo().Wait();
                //    LoadFromMongoTweets().Wait();
                //}
                //else
                //{
                //    throw new ApplicationException("Unknown command");
                //}
                //ExportHandlerProfiles export = new ExportHandlerProfiles();
                //export.Export();
                
            }
            catch (Exception ex)
            
            {
                log.Error(ex, ex.Message);
            }

        }

        private static async Task LoadFromMongo()
        {
            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var filter = new BsonDocument();
            var users = mdb.GetCollection<BsonDocument>("users");


            using (var cursor = await users.FindAsync(filter))
            {
                using (TwitterDbContext db = new TwitterDbContext())
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;
                        foreach (var u in batch)
                        {
   

                            long id = long.Parse(u["id_str"].AsString);

                            //TwitterProfile tp = db.Users.Find(id);
                            //log.Info("loading user id:{0}", id);
                            Console.WriteLine("loading user id:{0}"+id);
                            //if (tp == null)
                            //{
                                TwitterProfile tp = new TwitterProfile();
                                tp.Id = id;
                                db.Users.Add(tp);
                            //}

                           
                            tp.ContributorsEnabled = u["contributors_enabled"].AsBoolean;
                            tp.CreatedAt = DateTime.ParseExact((string)u["created_at"].AsString, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
                            tp.Description = u["description"].AsString;
                            tp.FavouritesCount = u["favourites_count"].AsInt32;
                            tp.FollowersCount = u["followers_count"].AsInt32;
                            tp.FriendsCount = u["friends_count"].AsInt32;
                            tp.GeoEnabled = u["geo_enabled"].AsBoolean;
                            tp.Lang = u["lang"].AsString;
                            tp.ListedCount = u["listed_count"].AsInt32;
                            tp.Location = u["location"].AsString;
                            tp.Name = u["name"].AsString;
                            tp.Protected = u["protected"].AsBoolean;
                            tp.ScreenName = u["screen_name"].AsString;
                            tp.StatusesCount = u["statuses_count"].AsInt32;
                            tp.Verified = u["verified"].AsBoolean;

                            db.SaveChanges();

                        }
                    }
                }
            }
        }

        private static async Task LoadFromMongoTweets()
        {
            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var filter = new BsonDocument();
            var users = mdb.GetCollection<BsonDocument>("tweets");

            DataTable dt = CreateTable();
            int c = 0;
                //using (TwitterDbContext db = new TwitterDbContext())
            using (SqlConnection connection =  new SqlConnection(@"Data Source=JOHNNY-PC\SQLEXPRESS;Initial Catalog=twitterdb;Integrated Security=True"))
                
            {
                    connection.Open();
                    using (var cursor = await users.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;

                        

                        
                            foreach (var u in batch)
                            {


                                ImportTweet(dt, u);

                            }

                            if (c++ < 10)
                                continue;

                            c = 0;

                        try
                        {

                            // make sure to enable triggers
                            // more on triggers in next post
                            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);

                            // set the destination table name
                            bulkCopy.DestinationTableName = "TwitterTweets";


                            // write the data in the "dataTable"
                            bulkCopy.WriteToServer(dt);
                            //connection.Close();
                            Console.WriteLine("loading {0} tweets...", dt.Rows.Count);
                            dt = CreateTable();
                            // reset}
                        }catch(Exception ex)
                        {
                            Console.WriteLine("error");
                        }



                    }
                }
            }
        }

        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("ProfileId", typeof(long));
            dt.Columns.Add("CreatedBy", typeof(string));
            dt.Columns.Add("CreatedAt", typeof(DateTime));


            dt.Columns.Add("FavouriteCount", typeof(long));
            dt.Columns.Add("RetweetCount", typeof(long));
            dt.Columns.Add("Text", typeof(string));
            return dt;
        }

        private static void ImportTweet(DataTable dt, BsonDocument u)
        {
            try
            {
                long id = long.Parse(u["id_str"].AsString);
                long profileid = long.Parse(u["user"]["id_str"].AsString);
                DateTime createdat = DateTime.ParseExact((string)u["created_at"].AsString, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
                string createdby = u["user"]["screen_name"].AsString;
                long fc = u["favorite_count"].AsInt32;
                long rc = u["retweet_count"].AsInt32;
                string text = u["text"].AsString;

                DataRow dr = dt.NewRow();
                dr["Id"] = id;
                dr["ProfileId"] = profileid;
                dr["CreatedAt"] = createdat;
                dr["CreatedBy"] = createdby;
                dr["FavouriteCount"] = fc;
                dr["RetweetCount"] = rc;
                dr["Text"] = text;

                dt.Rows.Add(dr);


                //TwitterProfile tp = db.Users.Find(id);
                //log.Info("loading user id:{0}", id);
                // Console.WriteLine("loading user id:{0}" + id);
                //if (tp == null)
                //{
               // TwitterTweet tp = new TwitterTweet();
               // tp.Id = id;
               //// db.Tweets.Add(tp);
               // //}


               // tp.ProfileId = long.Parse(u["user"]["id_str"].AsString);
               // tp.CreatedAt = DateTime.ParseExact((string)u["created_at"].AsString, "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
               // tp.CreatedBy = u["user"]["screen_name"].AsString;
               // tp.FavouriteCount = u["favorite_count"].AsInt32;
               // tp.RetweetCount = u["retweet_count"].AsInt32;
               // tp.Text = u["text"].AsString;

                //db.SaveChanges();
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                Console.WriteLine("error loading tweet.. already loaded probably...");
            }
        }

        


        private static async Task GetTwitterData(string cmd)
        {
            //do
            //{
            try
            {
                if (cmd.Equals("-captureleaders"))
                    await DownloadLeaders2();
                if (cmd.Equals("-captureretweeters"))
                    await DownloadRetweeters();


            }
            catch(Exception ex) {
                string msg = "CRITICAL error downloading retweeters";

                log.Error(ex, msg);
            }
                //ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadLeaders));
            //}
            //while (!(key = Console.ReadKey().Key).Equals(ConsoleKey.Escape));
               // while (true) ;
            await dummy();
        }

        private static async Task dummy()
        {
            log.Trace("finishing twitter capture...[ok]");
        }

        private static async Task DownloadRetweeters()
        {
            //Thread.CurrentThread.Name = "retweeters";
           
            log.Info("Initializing download retweeters info...");
            RateLimit.RateLimitTrackerOption = RateLimitTrackerOptions.TrackAndAwait;
            Auth.SetUserCredentials("QGVxnRdRgXejpz1vGZ2zeZbIF", "xsYiPSimriSTg5Tj3EK62otj76GMcJDAGzsD0yBThqYhzi0zPT", "1143544813-ESKB02tQga4myKWlCsZAPEPOTJon7IMhX15TxOP", "rTe6QHPaX3KdtZRxllHcYsdrhVrio0wIlHMlGgZ723vhD");
            var user = User.GetLoggedUser();
            log.Info("Logged as " + user.ScreenName);

            log.Trace("loading db objects...");
            IMongoClient mclient = new MongoClient();
            //mclient.Settings.ConnectTimeout = new TimeSpan(1, 0, 0);
            

            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var leaders = mdb.GetCollection<BsonDocument>("retweeters");
            var filter = new BsonDocument();
            var users = mdb.GetCollection<BsonDocument>("users");
            var leadersbak = mdb.GetCollection<BsonDocument>("retweetersbak");
            
            while (key != ConsoleKey.Escape)
            {
                try
                {
                    await Dr(leaders, filter, users, leadersbak);
                }
                catch (Exception ex)
                {
                    string msg = "General error downloading retweeters";
                    
                    log.Error(ex, msg);
                }
            }
        }

        private static async Task Dr(IMongoCollection<BsonDocument> leaders, BsonDocument filter, IMongoCollection<BsonDocument> users, IMongoCollection<BsonDocument> leadersbak)
        {
            using (var cursor = await leaders.FindAsync(filter))
            {

                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var rt in batch)
                    {
                        BsonArray bRetweeters = rt["retweeters_id"].AsBsonArray;

                        //if (bRetweeters.Count == 0)
                        //    continue;

                        foreach (var bUser in bRetweeters)
                        {
                            var lUser = bUser.ToInt64();
                            try
                            {

                                log.Info("readling retweeter {0}", lUser);
                                var usersFilter = Builders<BsonDocument>.Filter.Eq("id", lUser);
                                var result = await users.Find(usersFilter).ToListAsync();
                                if (result.Count > 0)
                                    continue;

                                log.Info("downloading retweeter {0}", lUser);
                                TwitterProfileHandler profileHandler = new TwitterProfileHandlerMongo();
                                string screenName = profileHandler.Download(lUser);
                                //TwitterHomelineHandler homeHandler = new TwitterHomelineHandlerMongo();
                                //homeHandler.Download(screenName);
                            }
                            catch (Exception ex)
                            {
                                string msg = "Error trying to download profile for user:{0}";
                                msg = string.Format(msg, lUser);
                                log.Error(ex, msg);
                            }
                        }

                        //rt["done"] = true;
                        //var filterupdate = Builders<BsonDocument>.Filter.Eq("_id", rt["_id"]);
                        //var update = Builders<BsonDocument>.Update.Push("done", true);
                        //await leaders.UpdateOneAsync(filterupdate, rt);
                        await leadersbak.InsertOneAsync(rt);
                        var filterbak = Builders<BsonDocument>.Filter.Eq("_id", rt["_id"]);
                        await leaders.DeleteOneAsync(filterbak);
                    }
                }
            }

            Thread.Sleep(5000);
        }

     
        private static async Task DownloadLeaders()
        {
            //Thread.CurrentThread.Name = "leaders";
            log.Info("Initializing download leaders info...");
            RateLimit.RateLimitTrackerOption = RateLimitTrackerOptions.TrackAndAwait;
            Auth.SetUserCredentials("W628b4dGSYkBy3oxaBou0Dp7W", "4DSRpc2uPKkw2yTn0Q5AdaO3zkHNJK7p2mA2f8M05alTQl8ulX", "1020526939-g4betm5uhETG7Eh9b4JipGjyUsaQtSvTJDZuDyS", "QIjKTi5NkqAbSwJwwtc9HcpzTnNpGrRxJKcns0CbTMXJA");
            var user = User.GetLoggedUser();
            log.Info("Logged as " + user.ScreenName);
            

            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var leaders = mdb.GetCollection<BsonDocument>("leaders");
            var filter = new BsonDocument();

            while (key != ConsoleKey.Escape)
            {
                using (var cursor = await leaders.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;
                        foreach (var leader in batch)
                        {

                            try
                            {
                                string u = leader["user"].AsString;
                                log.Trace("procesing leader {0}", u);
                                TwitterProfileHandler profileHandler = new TwitterProfileHandlerMongo();
                                profileHandler.Download(u);
                                TwitterHomelineHandlerMongo homeHandler = new TwitterHomelineHandlerMongo();
                                homeHandler.MaxTweets = 200;
                                homeHandler.Download(u);
                                //TwitterRetweetHandler retweeterHandler = new TwitterRetweetHandlerMongo();
                                //retweeterHandler.Download(u);
                            }catch(Exception ex)
                            {
                                log.Error(ex.Message, ex);
                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }


        private static async Task DownloadLeaders2()
        {
            //Thread.CurrentThread.Name = "leaders";
            log.Info("Initializing download leaders info...");
            RateLimit.RateLimitTrackerOption = RateLimitTrackerOptions.TrackAndAwait;
            Auth.SetUserCredentials("W628b4dGSYkBy3oxaBou0Dp7W", "4DSRpc2uPKkw2yTn0Q5AdaO3zkHNJK7p2mA2f8M05alTQl8ulX", "1020526939-g4betm5uhETG7Eh9b4JipGjyUsaQtSvTJDZuDyS", "QIjKTi5NkqAbSwJwwtc9HcpzTnNpGrRxJKcns0CbTMXJA");
            var user = User.GetLoggedUser();
            log.Info("Logged as " + user.ScreenName);


            IMongoClient mclient = new MongoClient();
            IMongoDatabase mdb = mclient.GetDatabase("twitter");
            var leaders = mdb.GetCollection<BsonDocument>("users");
            var proc = mdb.GetCollection<BsonDocument>("usersproc");
            var filter = new BsonDocument();



            while (true)
            {
                try
                {
                    using (var cursor = await leaders.FindAsync(filter))
                    {
                        while (await cursor.MoveNextAsync())
                        {
                            var batch = cursor.Current;
                            foreach (var leader in batch)
                            {

                                try
                                {
                                    long id = long.Parse(leader["id_str"].AsString);
                                    var filterproc = Builders<BsonDocument>.Filter.Eq("id", id);
                                    var res = await proc.Find(filterproc).ToListAsync();
                                    //log.Info("procesing leader {0}", id);
                                    Console.WriteLine(string.Format("procesing leader {0}", id));
                                    if (res.Count > 0)
                                        continue;



                                    string u = leader["screen_name"].AsString;
                                    
                                    //TwitterProfileHandler profileHandler = new TwitterProfileHandlerMongo();
                                    //profileHandler.Download(u);
                                    TwitterHomelineHandlerMongo homeHandler = new TwitterHomelineHandlerMongo();
                                    homeHandler.MaxTweets = 200;
                                    homeHandler.Download(u);
                                    //TwitterRetweetHandler retweeterHandler = new TwitterRetweetHandlerMongo();
                                    //retweeterHandler.Download(u);

                                    BsonDocument d = new BsonDocument();
                                    d["id"] = id;
                                    await proc.InsertOneAsync(d);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.Message, ex);
                                }
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
                catch (Exception exx)
                {
                    log.Error(exx.Message);
                }
            }
           
        
        }
    }
}
