using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture.Core
{
    public class ExportHandlerProfiles : ExportHandler
    {
        public override void Export(string path, TwitterDbContext db, int interval = 7, string filter = "")
        {
            //throw new NotImplementedException();
        }

        public void Export()
        {
            string namesfile = @"h:\dev\twitter\datanames.tsv";
            string datafile = @"h:\dev\twitter\data.tsv";
            //string yfile = @"h:\dev\twitter\datay.tsv";

            using (SqlConnection conn = new SqlConnection(@"Data Source=JOHNNY-PC\SQLEXPRESS;Initial Catalog=twitterdb;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GetTwitterUsers";
                cmd.Connection = conn;


                StreamWriter swNames = new StreamWriter(namesfile);
                StreamWriter swData = new StreamWriter(datafile);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        MapProfile(swNames, swData, dr);

                    }

                }

                swNames.Close();
                swData.Close();
            }
            
        }

        private void MapProfile(StreamWriter swNames, StreamWriter swData, SqlDataReader dr)
        {
            long id = (long)dr["Id"];
            string nick = (string)dr["ScreenName"];
            string name = (string)dr["Name"];
            string desc = (string)dr["Description"];
            long tweets = (long)dr["StatusesCount"];
            long followers = (long)dr["FollowersCount"];
            long favourites = (long)dr["FavouritesCount"];
            long friends = (long)dr["FriendsCount"];
            long lists = (long)dr["ListedCount"];
            bool protectedAcc = (bool)dr["Protected"];
            bool geoenabled = (bool)dr["GeoEnabled"];
            bool verified = (bool)dr["Verified"];
            string lang = (string)dr["Lang"];
            string loc = (string)dr["Location"];
            bool contributors = (bool)dr["ContributorsEnabled"];
            int yType = (int)dr["Ytype"];
            int yLeader = (int)dr["Yleader"];
            string yCategory = (string)dr["Ycategory"];
            long favoritesReceived = dr["FavouriteCount"]==DBNull.Value ? 0 : (long)dr["FavouriteCount"];
            long retweetReceived = dr["FavouriteCount"] == DBNull.Value ? 0 : (long)dr["RetweetCount"];
            long numWordsName = (long)dr["NumWordsForName"];
            long numWordsDesc = (long)dr["NumWordsForDesc"];
            long numCharsName = (long)dr["NumCharsForName"];
            long numCharsDesc = (long)dr["NumCharsForDesc"];



             

            StringBuilder sbdata = new StringBuilder();
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(tweets));
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(followers));
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(favourites) );
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(friends));
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(lists));
            sbdata.AppendFormat("{0}\t", protectedAcc == false ? 0 : 1);
            sbdata.AppendFormat("{0}\t", geoenabled == false ? 0 : 1);
            sbdata.AppendFormat("{0}\t", verified == false ? 0 : 1);
            sbdata.AppendFormat("{0}\t", contributors == false ? 0 : 1);
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(favoritesReceived));
            sbdata.AppendFormat("{0}\t", GetWhitenedValue(retweetReceived));
            sbdata.AppendFormat("{0}\t", yLeader);
            sbdata.AppendFormat("{0}", yType);

            swData.WriteLine(sbdata.ToString());

            name = CleanTweet(name);
            desc = CleanTweet(desc);
            loc = CleanTweet(loc);

            StringBuilder sbnames = new StringBuilder();
            sbnames.AppendFormat("{0}\t", name);
            sbnames.AppendFormat("{0}\t", desc);
            sbnames.AppendFormat("{0}\t", lang);
            sbnames.AppendFormat("{0}\t", loc);
            //sbnames.AppendFormat("{0}\t", name);

            swNames.WriteLine(sbnames.ToString());
        }

        private static double GetWhitenedValue(long val)
        {
            //return val;
            return val == 0 ? 0 : Math.Log10(val);
        }
    }
}
