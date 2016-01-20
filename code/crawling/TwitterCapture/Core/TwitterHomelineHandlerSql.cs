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
    public class TwitterHomelineHandlerSql : TwitterHomelineHandler
    {
        public override void Download(string screename)
        {
            //var api_path = "https://api.twitter.com/1.1/statuses/user_timeline.json?user_id={0}&count=200";

            //using (TwitterDbContext db = new TwitterDbContext())
            //{
            //    foreach (var user in db.Users)
            //    {
            //        _log.Info("Getting historical (MoreHist:{1}) tweets for user:{0}...", user.ScreenName, user.HasMoreHistoricalTweets);

            //        if (!user.HasMoreHistoricalTweets)
            //            continue;

            //        var minTweetId = (from t in db.Tweets where t.ProfileId == user.Id select t.Id).Min(i => (long?)i);
            //        _log.Info("Getting min tweet ID:{0} for user:{1}", minTweetId, user.ScreenName);
            //        var maxTweetId = (from t in db.Tweets where t.ProfileId == user.Id select t.Id).Max(i => (long?)i);
            //        _log.Info("Getting max tweet ID:{0} for user:{1}", maxTweetId, user.ScreenName);

            //        var query = string.Format(api_path, user.Id);

            //        if (minTweetId.HasValue)
            //        {
            //            var api_path2 = api_path + "&max_id={1}";
            //            query = string.Format(api_path2, user.Id, minTweetId - 1);
            //        }


            //        var jsonObject = TwitterAccessor.ExecuteJsonGETQuery(query);

            //        if (jsonObject == null)
            //        {
            //            _log.Info("Probably rate limit error getting tweets for user:{0} ", user.ScreenName);
            //            continue;
            //        }

            //        List<object> otweets = (List<object>)JsonConvert.DeserializeObject<List<object>>(jsonObject);
            //        _log.Info("Getting {0} old tweets for user:{1}", otweets.Count, user.ScreenName);

            //        if (otweets.Count == 0)
            //        {
            //            user.HasMoreHistoricalTweets = false;
            //        }

            //        foreach (object oTweet in otweets)
            //        {
            //            JObject jTweet = JObject.Parse(oTweet.ToString());

            //            TwitterTweet tweet = new TwitterTweet();
            //            tweet.Id = (long)jTweet["id"];
            //            tweet.ProfileId = (long)jTweet["user"]["id"];
            //            tweet.CreatedBy = (string)jTweet["user"]["screen_name"];
            //            tweet.CreatedAt = DateTime.ParseExact((string)jTweet["created_at"], "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
            //            //tweet.IsRetweet = (bool)jTweet["retweeted"];
            //            tweet.FavouriteCount = (long)jTweet["favorite_count"];
            //            tweet.RetweetCount = (long)jTweet["retweet_count"];
            //            tweet.Text = (string)jTweet["text"];
            //            //tweet.Json = jTweet.ToString();

            //            db.Tweets.Add(tweet);
            //        }

            //        if (maxTweetId.HasValue)
            //        {
            //            // TODO
            //        }

            //        db.SaveChanges();
            //    }


            //}
        }
    }
}
