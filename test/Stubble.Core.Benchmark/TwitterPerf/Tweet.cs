using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Benchmark.TwitterPerf
{

    public class UserMention
    {
        public string name { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
        public int id { get; set; }
        public string screen_name { get; set; }
    }

    public class UserEntities
    {
        public List<string> urls { get; set; }
        public List<string> hashtags { get; set; }
        public List<UserMention> user_mentions { get; set; }
    }

    public class User
    {
        public int statuses_count { get; set; }
        public bool profile_use_background_image { get; set; }
        public string time_zone { get; set; }
        public bool @protected { get; set; }
        public bool default_profile { get; set; }
        public bool notifications { get; set; }
        public string profile_text_color { get; set; }
        public string name { get; set; }
        public string expanded_url { get; set; }
        public bool default_profile_image { get; set; }
        public bool following { get; set; }
        public bool verified { get; set; }
        public bool geo_enabled { get; set; }
        public string profile_background_image_url { get; set; }
        public int favourites_count { get; set; }
        public string id_str { get; set; }
        public int utc_offset { get; set; }
        public string profile_link_color { get; set; }
        public string profile_image_url { get; set; }
        public string description { get; set; }
        public bool is_translator { get; set; }
        public string profile_background_image_url_https { get; set; }
        public string location { get; set; }
        public bool follow_request_sent { get; set; }
        public int friends_count { get; set; }
        public string profile_background_color { get; set; }
        public bool profile_background_tile { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public int followers_count { get; set; }
        public string profile_image_url_https { get; set; }
        public UserEntities entities { get; set; }
        public string lang { get; set; }
        public bool show_all_inline_media { get; set; }
        public int listed_count { get; set; }
        public bool contributors_enabled { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public int id { get; set; }
        public string created_at { get; set; }
        public string screen_name { get; set; }
    }

    public class Small
    {
        public int h { get; set; }
        public int w { get; set; }
        public string resize { get; set; }
    }

    public class Large
    {
        public int h { get; set; }
        public int w { get; set; }
        public string resize { get; set; }
    }

    public class Thumb
    {
        public int h { get; set; }
        public int w { get; set; }
        public string resize { get; set; }
    }

    public class Medium2
    {
        public int h { get; set; }
        public int w { get; set; }
        public string resize { get; set; }
    }

    public class Sizes
    {
        public Small small { get; set; }
        public Large large { get; set; }
        public Thumb thumb { get; set; }
        public Medium2 medium { get; set; }
    }

    public class Medium
    {
        public string type { get; set; }
        public string expanded_url { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
        public string url { get; set; }
        public string media_url { get; set; }
        public string display_url { get; set; }
        public long id { get; set; }
        public string media_url_https { get; set; }
        public Sizes sizes { get; set; }
    }

    public class TweetEntities
    {
        public List<string> urls { get; set; }
        public List<string> hashtags { get; set; }
        public List<Medium> media { get; set; }
        public List<UserMention> user_mentions { get; set; }
    }

    public class Tweet
    {
        public int? in_reply_to_status_id { get; set; }
        public bool possibly_sensitive { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string contributors { get; set; }
        public bool truncated { get; set; }
        public string id_str { get; set; }
        public User user { get; set; }
        public int retweet_count { get; set; }
        public string in_reply_to_user_id { get; set; }
        public bool favorited { get; set; }
        public string geo { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public TweetEntities entities { get; set; }
        public string coordinates { get; set; }
        public string source { get; set; }
        public string place { get; set; }
        public bool retweeted { get; set; }
        public long id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public string annotations { get; set; }
        public string text { get; set; }
        public string created_at { get; set; }
    }
}
