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
    public abstract class ExportHandler
    {
        protected Logger log = NLog.LogManager.GetCurrentClassLogger();






        public abstract void Export(string path, TwitterDbContext db, int interval = 7, string filter = "");


        protected void ExportTweet(StreamWriter sw, string tweetText, StreamWriter swWeight=null, long tweetWeight=0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CleanTweet(tweetText));

            if (sb.Length > 0)
            {
                string cleanTweet = sb.ToString();

                if (cleanTweet.Split(new char[] { ' ' }).Length > 1)
                {

                    sw.WriteLine(cleanTweet);

                    if (swWeight != null)
                    {
                        swWeight.WriteLine(tweetWeight.ToString());

                    }
                }
            }

           
        }

        protected string CleanTweet(string tweet)
        {
            // remove URLs
            //Regex linkParser = new Regex(@"(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //tweet = linkParser.Replace(tweet, delegate(Match match) { return string.Empty; });
            Regex removeTime = new Regex(@"\d\dh\d\d");
            tweet = removeTime.Replace(tweet, delegate(Match match) { return string.Empty; });


            StringBuilder sbTweet = new StringBuilder();
            string[] tokens = tweet.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int iGarbage = -1;

            foreach (string word in tokens)
            {
                string wordNoGarbage = word;
                if ((iGarbage = word.IndexOf("http")) != -1)
                    wordNoGarbage = word.Remove(iGarbage);

                if ((iGarbage = wordNoGarbage.IndexOf("@")) != -1)
                    wordNoGarbage = wordNoGarbage.Remove(iGarbage);


                if (string.IsNullOrEmpty(wordNoGarbage))
                    continue;

                string[] tokens2 = RemoveDiacritics(wordNoGarbage).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string cleanWord in tokens2)
                    if (cleanWord.Length > 3)
                        sbTweet.Append(cleanWord + " ");
            }

            return sbTweet.ToString();
        }

        protected string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    char ctweet = stFormD[ich];

                    if (char.IsLetterOrDigit(ctweet))
                        sb.Append(ctweet);
                    else
                        sb.Append(' ');
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

    }
}
