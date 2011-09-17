using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace redditMetro.Models
{
    [DataContract]
    public class ListingBaseResponse
    {
        [DataMember]
        public ListingResponse data { get; set; }
        [DataMember]
        public String kind { get; set; }
    }


    public class ListingResponse
    {
        /// Next Page Name
        public String after { get; set; }

        // previous page name
        public String before { get; set; }

        public List<ListingItem> children { get; set; }

        // hash, unsure what this is for yet.
        public string modhash { get; set; }
    }

    public class ListingItem
    {
        public ListingData data { get; set; }
        public string kind { get; set; }
    }

    public class ListingData
    {
        public String author { get; set; }
        public String author_flair_css_class { get; set; }
        public String author_flair_text { get; set; }
        public bool clicked { get; set; }
        public long created { get; set; }
        public long created_utc { get; set; }
        public String domain { get; set; }

        // down votes
        public long downs { get; set; }
        
        // up votes
        public long ups { get; set; }

        public bool hidden { get; set; }

        public string id { get; set; }
        public bool is_self { get; set; }
        // always null
        public string levenshtein { get; set; }

        // null if not logged in, otherwise indicates if user has voted the story {true for upvote, false for downvote, null for no vote}
        public bool? likes { get; set; }

        // usually null it seems
        //public String media { get; set; }

        // seems to always be an empty object
        //public MediaData media_embed { get; set; }
        public string name { get; set; }
        public long num_comments { get; set; }
        public bool over_18 { get; set; }
        public string permalink { get; set; }
        public bool saved { get; set; }
        public long score { get; set; }
        public String selftext { get; set; }
        public String selftext_html { get; set; }
        public string subreddit { get; set; }
        public string subreddit_id { get; set; }
        public string thumbnail { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }

    public class MediaData
    {

    }
}
