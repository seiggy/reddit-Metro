﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using redditMetro.Collections;
using redditMetro.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Security.Credentials;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace redditMetro
{
    public sealed partial class CollectionPage
    {
        public CollectionPage()
        {
            InitializeComponent();
            Subreddits = new RedditCollection(App.isLoggedIn);
            BackButton.IsEnabled = false;
            ItemListView.ItemsSource = Subreddits;

            GetReddits();
        }

        private async void GetReddits()
        {
            if (Subreddits.LoggedIn)
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
                    await Subreddits.LoadCollection(response);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                App.JsonClient = new HttpClient();
                var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
                await Subreddits.LoadCollection(response);
            }

            ItemListView.ScrollIntoView(ItemListView.Items[0]);
        }

        void ItemView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Construct the appropriate destination page and set its context appropriately
            var selectedItem = (sender as Selector).SelectedItem as IGroupInfo;
            var subreddit = (sender as Selector).SelectedItem as Subreddit;
            App.SelectedSubreddit = subreddit;
            App.ShowSplit(selectedItem);
            
        }

        public RedditCollection Subreddits { get; set; }

        // View state management for switching among Full, Fill, Snapped, and Portrait states

        private DisplayPropertiesEventHandler _displayHandler;
        private TypedEventHandler<ApplicationLayout, ApplicationLayoutChangedEventArgs> _layoutHandler;

        //private void RespCallback(IAsyncResult ar)
        //{
        //    RequestState rs = (RequestState)ar.AsyncState;
        //    WebRequest req = rs.Request;
        //    WebResponse response = req.EndGetResponse(ar);
        //    Stream responseStream = response.GetResponseStream();
        //    rs.ResponseStream = responseStream;
        //    //LoadCollection(responseStream);
        //}
                
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //App.BaseFilePath = this.BaseUri.AbsolutePath;
            //CollectionViewSource.Source = Subreddits.Items;
            //ItemGridView.ItemsSource = Subreddits;
            ItemGridView.IncrementalLoadingTrigger = IncrementalLoadingTrigger.Edge;
            //Subreddits.LoadMoreItemsAsync(25, this.Dispatcher);

            //ItemGridView.DataContext = Subreddits;

            App.SearchPane = SearchPane.GetForCurrentView();
            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged += _layoutHandler;

            if (App.Subreddits == null || App.Subreddits.Count == 0)
            {
                //Task.Run(() =>
                //{
                //    //App.Subreddits = new Collections.RedditCollection(App.isLoggedIn);
                //    //App.Subreddits.CollectionChanged += new Windows.Foundation.Collections.VectorChangedEventHandler<object>(Subreddits_CollectionChanged);
                //    //App.Subreddits.LoadMoreItemsAsync(25);
                    
                //});
                //ItemGridView.ItemsSource = App.Subreddits;
                //if (App.isLoggedIn)
                //{
                //    try
                //    {
                //        var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json");
                //        request.CookieContainer = new CookieContainer();
                        
                //        Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
                //        request.CookieContainer.Add(new Uri("http://www.reddit.com"), c);

                //        RequestState rs = new RequestState();
                //        rs.Request = request;

                //        var response = request.BeginGetResponse(new AsyncCallback(RespCallback), rs);
                //        //LoadCollection(response);
                //    }
                //    catch (Exception)
                //    {

                //    }
                //}
                //else
                //{
                //    var client = new HttpClient();
                //    var response = client.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
                //    LoadCollection(response);
                //}
            }
            else
            {
                //CollectionViewSource.Source = App.Subreddits;
                this.UpdateLayout();
            }

            SettingsPane settingsPane = SettingsPane.GetForCurrentView();
            
            if(settingsPane.ApplicationCommands.Count == 0)
                settingsPane.ApplicationCommands.Add(new SettingsCommand(KnownSettingsCommand.Account, new UICommandInvokedHandler(AccountCommandHandler)));
            
            SetCurrentViewState(this);
        }

        void Subreddits_CollectionChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            Dispatcher.Invoke(CoreDispatcherPriority.Normal, (x, y) =>
                {
                    
                }, this, null);
            //this.ItemGridView.UpdateLayout();
        }

        public void AccountCommandHandler(IUICommand command)
        {
            accountSettings.Margin = ThicknessHelper.FromUniformLength(0);
        }

        //private async void LoadCollection(Stream contentStream)
        //{
        //    SubredditResponse data = new SubredditResponse();
        //    await Task.Run(() =>
        //        {
        //            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(SubredditResponse));
        //            data = (SubredditResponse)deserializer.ReadObject(contentStream);
        //        });

        //    foreach (Subreddit r in data.data.children)
        //    {
        //        if (r.data.url.ToLower().Contains("/r/pics"))
        //            r.data.image = App.BaseFilePath + "/Images/r.pics.png";
        //        else if (r.data.url.ToLower().Contains("/r/gaming"))
        //            r.data.image = App.BaseFilePath + "/Images/r.gaming.png";
        //        else if (r.data.url.ToLower().Contains("/r/askreddit"))
        //            r.data.image = App.BaseFilePath + "/Images/r.askreddit.png";
        //        else if (r.data.url.ToLower().Contains("/r/atheism"))
        //            r.data.image = App.BaseFilePath + "/Images/r.atheism.png";
        //        else if (r.data.url.ToLower().Contains("/r/circlejerk"))
        //            r.data.image = App.BaseFilePath + "/Images/r.circlejerk.png";
        //        else if (r.data.url.ToLower().Contains("/r/comics"))
        //            r.data.image = App.BaseFilePath + "/Images/r.comics.png";
        //        else if (r.data.url.ToLower().Contains("/r/fffffffuuuuuuuuuuuu"))
        //            r.data.image = App.BaseFilePath + "/Images/r.fu.png";
        //        else if (r.data.url.ToLower().Contains("/r/iama"))
        //            r.data.image = App.BaseFilePath + "/Images/r.iama.png";
        //        else if (r.data.url.ToLower().Contains("/r/minecraft"))
        //            r.data.image = App.BaseFilePath + "/Images/r.minecraft.png";
        //        else if (r.data.url.ToLower().Contains("/r/music"))
        //            r.data.image = App.BaseFilePath + "/Images/r.music.png";
        //        else if (r.data.url.ToLower().Contains("/r/science"))
        //            r.data.image = App.BaseFilePath + "/Images/r.science.png";
        //        else if (r.data.url.ToLower().Contains("/r/technology"))
        //            r.data.image = App.BaseFilePath + "/Images/r.technology.png";
        //        else if (r.data.url.ToLower().Contains("/r/tf2"))
        //            r.data.image = App.BaseFilePath + "/Images/r.tf2.png";
        //        else if (r.data.url.ToLower().Contains("/r/todayilearned"))
        //            r.data.image = App.BaseFilePath + "/Images/r.todayilearned.png";
        //        else if (r.data.url.ToLower().Contains("/r/trees"))
        //            r.data.image = App.BaseFilePath + "/Images/r.trees.png";
        //        else if (r.data.url.ToLower().Contains("/r/twoxchromosomes"))
        //            r.data.image = App.BaseFilePath + "/Images/r.twoxchromosomes.png";
        //        else if (r.data.url.ToLower().Contains("/r/videos"))
        //            r.data.image = App.BaseFilePath + "/Images/r.videos.png";
        //        else if (r.data.url.ToLower().Contains("/r/worldnews"))
        //            r.data.image = App.BaseFilePath + "/Images/r.worldnews.png";
        //        else if (r.data.url.ToLower().Contains("/r/wtf"))
        //            r.data.image = App.BaseFilePath + "/Images/r.wtf.png";
        //        else
        //            r.data.image = App.BaseFilePath + "/Images/reddit.com.header.png";
        //    }
        //    //App.Subreddits.AddRange(data.data.children);

        //    if (data.data.after != null && App.isLoggedIn)
        //    {
        //        Task.Run(() =>
        //            {
        //                //var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json?after=" + data.data.after);
        //                //request.CookieContainer = new CookieContainer();
        //                HttpClientHandler handler = new HttpClientHandler();
                        
        //                Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
        //                handler.CookieContainer.Add(new Uri("http://www.reddit.com"), c);
        //                App.JsonClient = new HttpClient(handler);
        //                //RequestState rs = new RequestState();
        //                //rs.Request = request;

        //                var response = App.JsonClient.GetAsync("http://www.reddit.com/reddits/mine.json?after=" + data.data.after).Result.Content;
        //                LoadCollection(response);
        //            });
        //    }
        //    else
        //    {
        //        // don't bind the collection till we have all the items...weird behavior otherwise
        //        // we're off the main UI thread now, so we need to invoke back to the UI thread to modify the CollectionViewSource
        //        Dispatcher.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, (x, y) =>
        //            {
        //                //CollectionViewSource.Source = App.Subreddits;
        //                this.UpdateLayout();
        //            }, this, null);
        //    }
        //    //CollectionViewSource.Source = data.data.children;
        //}

        //private void ExtraReddits(IAsyncResult ar)
        //{
        //    RequestState rs = (RequestState)ar.AsyncState;
        //    WebRequest req = rs.Request;
        //    WebResponse response = req.EndGetResponse(ar);
        //    Stream responseStream = response.GetResponseStream();
        //    rs.ResponseStream = responseStream;
        //    LoadCollection(responseStream);
        //}
        
        //private void LoadCollection(HttpContent messageTask)
        //{
        //    LoadCollection(messageTask.ContentReadStream);            
        //}

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

            DisplayProperties.OrientationChanged -= _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged -= _layoutHandler;
        }

        private void Page_LayoutChanged(object sender, ApplicationLayoutChangedEventArgs e)
        {
            SetCurrentViewState(this);
        }

        private void Page_OrientationChanged(object sender)
        {
            SetCurrentViewState(this);
        }

        private void SetCurrentViewState(Control viewStateAwareControl)
        {
            VisualStateManager.GoToState(viewStateAwareControl, this.GetViewState(), false);
        }

        private String GetViewState()
        {
            var orientation = DisplayProperties.CurrentOrientation;
            if (orientation == DisplayOrientations.Portrait ||
                orientation == DisplayOrientations.PortraitFlipped) return "Portrait";
            var layout = ApplicationLayout.Value;
            if (layout == ApplicationLayoutState.Filled) return "Fill";
            if (layout == ApplicationLayoutState.Snapped) return "Snapped";
            return "Full";
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }

        private async void Grid_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerEventArgs e)
        {
            if (accountSettings.Margin.Right == 0)
            {
                accountSettings.Margin = ThicknessHelper.FromLengths(0, 0, -346, 0);
                App.Settings["UserName"] = accountSettings.UserName;
                
                if (accountSettings.UserName.Length > 0 && accountSettings.Password.Length > 0 && !App.isLoggedIn)
                {
                    await LoginReddit();
                    Dispatcher.Invoke(CoreDispatcherPriority.Normal, (x, y) =>
                    {
                        Subreddits = new RedditCollection(App.isLoggedIn);
                        //Subreddits.LoadMoreItemsAsync(25, this.Dispatcher);
                    }, this, null);
                    //App.ShowCollection();
                    //try
                    //{
                    //    var request = (HttpWebRequest)WebRequest.Create("http://www.reddit.com/reddits/mine.json");
                    //    request.CookieContainer = new CookieContainer();

                    //    Cookie c = new Cookie("reddit_session", App.cookie.Replace(",", "%2C"));
                    //    request.CookieContainer.Add(new Uri("http://www.reddit.com"), c);

                    //    RequestState rs = new RequestState();
                    //    rs.Request = request;

                    //    var response = request.BeginGetResponse(new AsyncCallback(RespCallback), rs);
                    //    //LoadCollection(response);
                    //}
                    //catch (Exception)
                    //{

                    //}
                }
                else if (!accountSettings.SavePassword)
                {
                    App.Settings["SavePassword"] = false;
                    Task.Run(() =>
                        {
                            try
                            {
                                var passwords = App.PasswordVault.FindAllByResource("redditMetro");
                                foreach (var pass in passwords)
                                {
                                    if (pass.UserName == accountSettings.UserName)
                                        App.PasswordVault.Remove(pass);
                                }
                            }
                            catch (Exception)
                            {
                                //user doesn't have a password stored, ignore
                            }
                        });
                }
            }
        }

        private async Task LoginReddit()
        {
            try
            {
                App.JsonClient = new HttpClient();
                App.JsonClient.BaseAddress = new Uri("http://www.reddit.com/api/");

                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("api_type", "json");
                values.Add("user", accountSettings.UserName);
                values.Add("passwd", accountSettings.Password);

                FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                var response = await App.JsonClient.PostAsync("login/" + accountSettings.UserName, content);
                var stream = response.EnsureSuccessStatusCode().Content.ContentReadStream;
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(LoginResponse));
                var data = (LoginResponse)deserializer.ReadObject(stream);
                if (data.json.errors.Count == 0)
                {
                    App.modhash = data.json.data.modhash;
                    App.cookie = data.json.data.cookie;
                    App.isLoggedIn = true;
                    App.Subreddits = new RedditCollection(App.isLoggedIn);
                    App.Settings["UserName"] = accountSettings.UserName;
                    if (accountSettings.SavePassword)
                    {
                        bool passwordSet = false;
                        try
                        {
                            var passwords = App.PasswordVault.FindAllByResource("redditMetro");

                            // check and see if the user's password is already stored, if it is, lets update the password
                            foreach (var pass in passwords)
                            {
                                if (pass.UserName == accountSettings.UserName)
                                {
                                    pass.Password = accountSettings.Password;
                                    passwordSet = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // user hasn't logged in before...ignore exception and store the password
                        }

                        // new user, so we'll add a new password cred to the store
                        if (!passwordSet)
                        {
                            App.PasswordVault.Add(new PasswordCredential("redditMetro", accountSettings.UserName, accountSettings.Password));
                        }
                        App.Settings["SavePassword"] = accountSettings.SavePassword;
                    }
                    else
                    {
                        // user no longer wants us to store their password, so we remove it from the password vault
                        try
                        {
                            var passwords = App.PasswordVault.FindAllByResource("redditMetro");
                            foreach (var pass in passwords)
                            {
                                if (pass.UserName == accountSettings.UserName)
                                    App.PasswordVault.Remove(pass);
                            }
                        }
                        catch (Exception)
                        {
                            //user doesn't have a password stored, ignore
                        }
                    }
                }
                else
                {
                    foreach (string s in data.json.errors)
                    {
                        accountSettings.ErrorMessage += s + ". ";
                    }
                    accountSettings.Margin = ThicknessHelper.FromUniformLength(0);
                }
            }
            catch (Exception)
            {
                // help!
            }
        }
    }
}
