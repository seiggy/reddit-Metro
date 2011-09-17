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

    [DataContract]
    public class SubredditInfo
    {
        [DataMember]
        public string display_name { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public long created { get; set; }
        [DataMember]
        public long created_utc { get; set; }
        [DataMember]
        public bool over18 { get; set; }
        [DataMember]
        public long subscribers { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string description { get; set; }

        [IgnoreDataMember]
        public string image { get; set; }
    }
}
