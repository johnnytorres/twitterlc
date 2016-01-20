using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture
{
    public class TwitterProfile
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None), Key] 
        public long Id { get; set; }
        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool Protected { get; set; }
        public DateTime CreatedAt { get; set; }
        public long StatusesCount { get; set; }
        public long FavouritesCount { get; set; }
        public long FollowersCount { get; set; }
        public long FriendsCount { get; set; }
        public long ListedCount { get; set; }
        public bool GeoEnabled { get; set; }
        public bool Verified { get; set; }
        public string Lang { get; set; }

        public bool ContributorsEnabled { get; set; }

       
        [Column("Yleader")]    
        public bool IsLeader { get; set; }

        public List<TwitterTweet> Tweets { get; set; }
    }
}
