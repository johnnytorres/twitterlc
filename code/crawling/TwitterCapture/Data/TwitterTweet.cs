using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture
{
    public class TwitterTweet
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None), Key] 
        public long Id { get; set; }

    
        public long ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public TwitterProfile Profile { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public long FavouriteCount { get; set; }

        public long RetweetCount { get; set; }

        public string Text { get; set; }


    }
}
