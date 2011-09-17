using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace redditMetro.Models
{
    [DataContract]
    public class SubredditResponse
    {
        [DataMember]
        public String kind { get; set; }

        [DataMember]
        public SubredditData data { get; set; }
    }

    public class SubredditData
    {
        public String modhash { get; set; }
        public List<Subreddit> children { get; set; }
        public String after { get; set; }
        public String before { get; set; }
    }

    public class Subreddit
    {
        public String kind { get; set; }
        public SubredditInfo data { get; set; }
    }

    public class SubredditInfo
    {
        public string display_name { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public long created { get; set; }
        public long created_utc { get; set; }
        public bool over18 { get; set; }
        public long subscribers { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }
}
