using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCapture
{
    public class TwitterDbContext : DbContext
    {
        public DbSet<TwitterProfile> Users { get; set; }
        public DbSet<TwitterTweet> Tweets { get; set; }

        public TwitterDbContext() : base("twitterdb") { }
    }
}
