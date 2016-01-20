using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterCapture.Core
{
    public class TwitterProfileHandlerSql : TwitterProfileHandler
    {
        public override void Download(string user)
        {
            // get user profiles

            var api_path = "https://api.twitter.com/1.1/users/show.json?screen_name={0}&include_entities={1}";

            using (TwitterDbContext db = new TwitterDbContext())
            {


                string query = string.Format(api_path, user, true);
                var jsonObject = TwitterAccessor.GetQueryableJsonObjectFromGETQuery(query);
                var twitterUser = db.Users.Where(tu => tu.ScreenName.Equals(user)).SingleOrDefault();

                if (twitterUser == null)
                {
                    twitterUser = new TwitterProfile();
                    db.Users.Add(twitterUser);
                }

                twitterUser.Id = (long)jsonObject["id"];
                twitterUser.Name = (string)jsonObject["name"];
                twitterUser.ScreenName = (string)jsonObject["screen_name"];
                twitterUser.CreatedAt = DateTime.ParseExact((string)jsonObject["created_at"], "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.InvariantCulture);
                twitterUser.StatusesCount = (long)jsonObject["statuses_count"];
                twitterUser.FavouritesCount = (long)jsonObject["favourites_count"];
                twitterUser.FollowersCount = (long)jsonObject["followers_count"];
                twitterUser.FriendsCount = (long)jsonObject["friends_count"];
                twitterUser.ListedCount = (long)jsonObject["listed_count"];
                //twitterUser.Json = jsonObject.ToString();


                db.SaveChanges();
            }
        }

        public override string Download(long userid)
        {
            throw new NotImplementedException();
        }
    }
}
