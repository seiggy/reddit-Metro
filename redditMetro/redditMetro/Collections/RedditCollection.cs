using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Databinding;
using redditMetro.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
//using Databinding;

namespace redditMetro.Collections
{
    public class RedditCollection : Databinding.ObservableVector, IIncrementalLoadingVector
    {
        bool hasMoreItems = false;
        string nextItems = "";
        string username = "";
                
        public bool LoggedIn { get; set; }
        
        public RedditCollection()
        {
            LoggedIn = false;
            hasMoreItems = true;
            //Task.Run(() => { LoadMoreItemsAsync(0); });
        }

        void list_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            // dunno if we should do anything here?
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, @event);
            }
        }

        public event VectorChangedEventHandler<object> CollectionChanged;

        public RedditCollection(bool isLoggedIn)
        {
            LoggedIn = isLoggedIn;
            hasMoreItems = true;
            //Task.Run(() => { LoadMoreItemsAsync(0); });
        }

        public string UserName
        {
            get { return this.username; }
            set { this.username = value; }
        }
        
        public bool IIncrementalLoadingVector.HasMoreItems
        {
            get { return this.hasMoreItems; }
        }

        public event AsyncOperationCompletedHandler<LoadMoreItemsResult> Completed;


        
        /// <summary>
        /// requires us to take in count, not sure that we'll use it, or if we can ignore it, 
        /// since reddit always operates on collections of 25 at a time...could complicate things...
        /// </summary>
        /// <param name="count">number of items to load...we'll always be loading 25 for now</param>
        /// <returns></returns>
        //public IAsyncOperation<LoadMoreItemsResult> IIncrementalLoadingVector.LoadMoreItemsAsync(uint count)
        //{
            //Task.Run(() =>
            //    {
            //        if (this.LoggedIn)
            //        {
            //            try
            //            {
            //                HttpClientHandler handler = new HttpClientHandler();
                            
            //                //var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json");
            //                //request.CookieContainer = new CookieContainer();

            //                Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
            //                handler.CookieContainer.Add(new Uri("http://www.reddit.com"), c);

            //                App.JsonClient = new HttpClient(handler);
                                                        
            //                var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits/mine.json").Result.Content;
            //                LoadCollection(response);
            //            }
            //            catch (Exception)
            //            {

            //            }
            //        }
            //        else
            //        {
            //            App.JsonClient = new HttpClient();
            //            var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
            //            LoadCollection(response);
            //        }
            //    });
            //return result;
        //}

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return new AsyncRedditLoader(this, count);
        }

        private void AppendCollections()
        {
            
        }

        public async Task LoadCollection(HttpContent response)
        {
            await Task.Run(() => { LoadCollection(response.ContentReadStream); });
        }

        public async Task LoadCollection(Stream responseStream)
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
                this.Add(subreddit);
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
            //Completed(result);
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
        
        class AsyncRedditLoader : IAsyncOperation<LoadMoreItemsResult>
        {
            private RedditCollection _rl;
            private uint _count;

            public AsyncRedditLoader(RedditCollection rl, uint count)
            {
                _rl = rl;
                _count = count;
            }

            private AsyncOperationCompletedHandler<LoadMoreItemsResult> _completed;

            AsyncOperationCompletedHandler<LoadMoreItemsResult> IAsyncOperation<LoadMoreItemsResult>.Completed
            {
                get { return _completed; }
                set { _completed = value; }
            }

            protected virtual void OnDownloadComplete()
            {
                if (null != _completed)
                {
                    _completed(this);
                }
            }

            LoadMoreItemsResult IAsyncOperation<LoadMoreItemsResult>.GetResults()
            {
                return new LoadMoreItemsResult { Count = _count };
            }

            void IAsyncInfo.Start()
            {
                if (null != _rl)
                {
                    // get reddits
                    if (_rl.LoggedIn)
                    {
                        try
                        {
                            HttpClientHandler handler = new HttpClientHandler();

                            //var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json");
                            //request.CookieContainer = new CookieContainer();

                            Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
                            handler.CookieContainer.Add(new Uri("http://www.reddit.com"), c);

                            App.JsonClient = new HttpClient(handler);

                            var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits/mine.json").Result.Content;
                            _rl.LoadCollection(response);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        App.JsonClient = new HttpClient();
                        var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
                        _rl.LoadCollection(response);
                    }

                    OnDownloadComplete();
                }
            }

            void IAsyncInfo.Cancel()
            {
                throw new NotImplementedException();
            }

            void IAsyncInfo.Close()
            {

            }

            Exception IAsyncInfo.ErrorCode
            {
                get { throw new NotImplementedException(); }
            }

            uint IAsyncInfo.Id
            {
                get { throw new NotImplementedException(); }
            }

            AsyncStatus IAsyncInfo.Status
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
