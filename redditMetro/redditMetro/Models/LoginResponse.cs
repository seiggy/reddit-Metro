using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redditMetro.Models
{
    public class LoginResponse
    {
        public LoginWrapper json { get; set; }
    }

    public class LoginWrapper
    {
        public List<string> errors { get; set; }
        public LoginData data { get; set; }
    }

    public class LoginData
    {
        public string modhash { get; set; }
        public string cookie { get; set; }
    }
}
