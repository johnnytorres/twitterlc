using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Parameters;

namespace TwitterCapture.Core
{
    public class TwitterStreammingHandler
    {
        Logger log = NLog.LogManager.GetCurrentClassLogger();



        public void GetStreamingData()
        {
            log.Info("Initializing twitter capture...");
            Auth.SetUserCredentials("W628b4dGSYkBy3oxaBou0Dp7W", "4DSRpc2uPKkw2yTn0Q5AdaO3zkHNJK7p2mA2f8M05alTQl8ulX", "1020526939-g4betm5uhETG7Eh9b4JipGjyUsaQtSvTJDZuDyS", "QIjKTi5NkqAbSwJwwtc9HcpzTnNpGrRxJKcns0CbTMXJA");
            var user = User.GetLoggedUser();
            Console.WriteLine(user.ScreenName);

            // Publish the Tweet "Hello World" on your Timeline
            //Tweet.PublishTweet("IBM plans to use #BigData to manage diabetes and obesity http://t.co/vt9e8M9FXI http://t.co/ewJHlNAHYL”");

            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddTrack("ecuador");
            stream.AddLocation(new Coordinates(-92.68, -5.01), new Coordinates(-75.19, 2.34));

            stream.JsonObjectReceived += stream_JsonObjectReceived;
            //stream.MatchingTweetAndLocationReceived += stream_MatchingTweetAndLocationReceived;
            //stream.MatchingTweetReceived += (sender, tweet) =>
            //{

            //    Console.WriteLine(tweet.Tweet);
            //};
            log.Info("Initializing twitter capture...[ok]");
            stream.StartStreamMatchingAllConditions();
        }

        void stream_MatchingTweetAndLocationReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetAndLocationReceivedEventArgs e)
        {
            //Console.WriteLine(e.Tweet);
        }

        void stream_JsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
        {
            try
            {
                string filename = DateTime.Now.ToString("yyyyMMdd") + ".log";

                File.AppendAllText(filename, e.Json);
                Console.WriteLine(e.Json);

            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
            }
        }
    }
}
