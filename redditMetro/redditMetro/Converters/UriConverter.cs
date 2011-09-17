using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace redditMetro.Converters
{
    public class UriConverter : IValueConverter
    {
        public object Convert(object value, string typeName, object parameter, string language)
        {
            string uri = value as string;
            if (uri != null)
            {
                if(uri.Contains("http://"))
                {
                    return uri;
                }
                else
                {
                    return "http://www.reddit.com" + uri;
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, string typeName, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
