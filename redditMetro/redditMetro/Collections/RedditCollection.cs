using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using System.Net;
using redditMetro.Models;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Foundation.Collections;
using Databinding;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace redditMetro.Collections
{
    public class RedditCollectionResult : IAsyncOperation<LoadMoreItemsResult>
    {
        public LoadMoreItemsResult Result;
        AsyncStatus status = AsyncStatus.Created;
        uint id = 0;
        Exception error;

        public RedditCollectionResult()
        {
            Result = new LoadMoreItemsResult();
            id = (uint)new Random().Next();
            error = null;
        }

        public AsyncOperationCompletedHandler<LoadMoreItemsResult> Completed
        {
            get;
            set;
        }
        
        public LoadMoreItemsResult GetResults()
        {
            return Result;
        }

        public void Cancel()
        {
            this.status = AsyncStatus.Canceled;
            Result.Count = 0;
        }

        public void Close()
        {
            this.status = AsyncStatus.Completed;
            if (Completed != null)
                Completed(this);
        }

        public Exception ErrorCode
        {
            get { return this.error; }
            set { this.error = value; }
        }

        public uint Id
        {
            get { return this.id; }
        }

        public void Start()
        {
            this.status = AsyncStatus.Started;
        }

        public AsyncStatus Status
        {
            get { return this.status; }
            set { this.status = value; }
        }
    }

    public class RedditCollection : IObservableVector<object>, IIncrementalLoadingVector
    {
        bool hasMoreItems = false;
        string nextItems = "";
        string username = "";
        string password = "";
        private IList<object> list;
        private ReadOnlyCollection<object> readOnlyList;
                
        public bool LoggedIn { get; set; }

        public IList<object> Items
        {
            get { return this.list; }
            set { this.list = value; }
        }

        public RedditCollection(INotifyCollectionChanged list)
        {
            LoggedIn = false;
            var ob = list.ToObservableVector<object>();
            this.list = ob.ToList<object>();
            readOnlyList = new ReadOnlyCollection<object>((IList<object>)list);
            hasMoreItems = true;
            //Task.Run(() => { LoadMoreItemsAsync(0); });
        }

        public RedditCollection(INotifyCollectionChanged list, bool isLoggedIn)
        {
            LoggedIn = isLoggedIn;
            var ob = list.ToObservableVector<object>();
            this.list = ob.ToList<object>();
            readOnlyList = new ReadOnlyCollection<object>((IList<object>)list);
            hasMoreItems = true;
            //Task.Run(() => { LoadMoreItemsAsync(0); });
        }

        public string UserName
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public bool HasMoreItems
        {
            get { return this.hasMoreItems; }
        }

        public event AsyncOperationCompletedHandler<LoadMoreItemsResult> Completed;

        private RedditCollectionResult result;

        /// <summary>
        /// requires us to take in count, not sure that we'll use it, or if we can ignore it, 
        /// since reddit always operates on collections of 25 at a time...could complicate things...
        /// </summary>
        /// <param name="count">number of items to load...we'll always be loading 25 for now</param>
        /// <returns></returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            result = new RedditCollectionResult();
            result.Start();
            Task.Run(() =>
                {
                    if (this.LoggedIn)
                    {
                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json");
                            request.CookieContainer = new CookieContainer();

                            Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
                            request.CookieContainer.Add(new Uri("http://www.reddit.com"), c);

                            RequestState rs = new RequestState();
                            rs.Request = request;

                            var response = request.BeginGetResponse(new AsyncCallback(RespCallback), rs);
                            //LoadCollection(response);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        var client = new HttpClient();
                        var response = client.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
                        LoadCollection(response);
                    }
                });
            return result;
        }

        private async void LoadCollection(HttpContent response)
        {
            await Task.Run(() => { LoadCollection(response.ContentReadStream); });
        }

        private async void LoadCollection(Stream responseStream)
        {
            SubredditResponse data = new SubredditResponse();
            await Task.Run(() =>
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(SubredditResponse));
                    data = (SubredditResponse)deserializer.ReadObject(responseStream);
                });
            foreach (Subreddit r in data.data.children)
            {
                if (r.data.url.ToLower().Contains("/r/pics"))
                    r.data.image = "/Images/r.pics.png";
                else if (r.data.url.ToLower().Contains("/r/gaming"))
                    r.data.image = "/Images/r.gaming.png";
                else if (r.data.url.ToLower().Contains("/r/askreddit"))
                    r.data.image = "/Images/r.askreddit.png";
                else if (r.data.url.ToLower().Contains("/r/atheism"))
                    r.data.image = "/Images/r.atheism.png";
                else if (r.data.url.ToLower().Contains("/r/circlejerk"))
                    r.data.image = "/Images/r.circlejerk.png";
                else if (r.data.url.ToLower().Contains("/r/comics"))
                    r.data.image = "/Images/r.comics.png";
                else if (r.data.url.ToLower().Contains("/r/fffffffuuuuuuuuuuuu"))
                    r.data.image = "/Images/r.fu.png";
                else if (r.data.url.ToLower().Contains("/r/iama"))
                    r.data.image = "/Images/r.iama.png";
                else if (r.data.url.ToLower().Contains("/r/minecraft"))
                    r.data.image = "/Images/r.minecraft.png";
                else if (r.data.url.ToLower().Contains("/r/music"))
                    r.data.image = "/Images/r.music.png";
                else if (r.data.url.ToLower().Contains("/r/science"))
                    r.data.image = "/Images/r.science.png";
                else if (r.data.url.ToLower().Contains("/r/technology"))
                    r.data.image = "/Images/r.technology.png";
                else if (r.data.url.ToLower().Contains("/r/tf2"))
                    r.data.image = "/Images/r.tf2.png";
                else if (r.data.url.ToLower().Contains("/r/todayilearned"))
                    r.data.image = "/Images/r.todayilearned.png";
                else if (r.data.url.ToLower().Contains("/r/trees"))
                    r.data.image = "/Images/r.trees.png";
                else if (r.data.url.ToLower().Contains("/r/twoxchromosomes"))
                    r.data.image = "/Images/r.twoxchromosomes.png";
                else if (r.data.url.ToLower().Contains("/r/videos"))
                    r.data.image = "/Images/r.videos.png";
                else if (r.data.url.ToLower().Contains("/r/worldnews"))
                    r.data.image = "/Images/r.worldnews.png";
                else if (r.data.url.ToLower().Contains("/r/wtf"))
                    r.data.image = "/Images/r.wtf.png";
                else
                    r.data.image = "/Images/reddit.com.header.png";
            }
            //App.Subreddits = data.data.children;
            foreach (Subreddit subreddit in data.data.children)
            {
                list.Add(subreddit);
            }

            if (data.data.after != null)
            {
                // we have more subreddits!
                this.nextItems = data.data.after;
                this.hasMoreItems = true;
            }
            else
            {
                this.nextItems = "";
                this.hasMoreItems = false;
            }
            result.Result.Count = (uint)list.Count;
            result.Close();
            Completed(result);
            //VectorChanged(this, new VectorChangedEventArgs());
            //VectorChanged(this, null);
        }

        private void RespCallback(IAsyncResult ar)
        {
            RequestState rs = (RequestState)ar.AsyncState;
            WebRequest req = rs.Request;
            WebResponse response = req.EndGetResponse(ar);
            Stream responseStream = response.GetResponseStream();
            rs.ResponseStream = responseStream;
            LoadCollection(responseStream);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public event VectorChangedEventHandler<object> VectorChanged;

        public int IndexOf(object item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Add(object item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(object item)
        {
            return list.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        public bool Remove(object item)
        {
            return list.Remove(item);
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
