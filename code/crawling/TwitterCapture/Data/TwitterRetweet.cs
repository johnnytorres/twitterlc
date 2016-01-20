using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture
{
    public class TwitterRetweet
    {
        [Key]
        public int RetweeterId { get; set; }
        [ForeignKey("RetweeterId")]
        public TwitterProfile Retweeter { get; set; }
        [Key]
        public int TweetId { get; set; }
        [ForeignKey("TweetId")]
        public TwitterTweet Tweet { get; set; }
    }
}
