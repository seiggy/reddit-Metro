using Expression.Blend.SampleData.SampleDataSource;
using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using redditMetro.Models;
using System.Collections;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using Windows.Security.Credentials;
using Windows.System.Threading;

namespace redditMetro
{
    partial class App
    {
        // TODO: Create a data model appropriate for your problem domain to replace the sample data
        private static SampleDataSource _sampleData;

        public static List<Subreddit> Subreddits { get; set; }
        public static ListingResponse Posts { get; set; }
        public static Subreddit SelectedSubreddit { get; set; }
        public static Subreddit PreviousSubreddit { get; set; }
        public static DateTime LastRefresh { get; set; }
        public static TimeSpan RefreshInterval = new TimeSpan(0, 0, 1, 0, 0);
        public static IPropertySet Settings { get; set; }
        public const string CONTAINER_NAME = "redditMetro";
        public static SearchPane SearchPane { get; set; }
        public static string modhash { get; set; }
        public static string cookie { get; set; }
        public static bool isLoggedIn { get; set; }
        public static PasswordVault PasswordVault { get; set; }
        private static string basefilepath = "";
        public static string BaseFilePath { get { return basefilepath; } set { basefilepath = value; } }

        public App()
        {
            InitializeComponent();
        }

        public async static void LoadSettings()
        {
            //PasswordCredential cred = new PasswordCredential("redditMetro", Settings["UserName"].ToString(), Settings["Password"].ToString());
            try
            {
                PasswordVault = new PasswordVault();
            }
            catch (Exception)
            {
                // wtf is going on here!
            }

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer(CONTAINER_NAME, ApplicationDataCreateDisposition.Always);
            Settings = container.Values;
            if (!Settings.ContainsKey("UserName"))
                Settings.Add("UserName", "");
            if (!Settings.ContainsKey("SavePassword"))
                Settings.Add("SavePassword", false);

            if (!String.IsNullOrEmpty(Settings["UserName"].ToString()) && (bool)Settings["SavePassword"])
            {
                LoginReddit();
            }
            else
            {
                isLoggedIn = false;
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            LastRefresh = DateTime.Now;
            LoadSettings();
            ShowCollection();
            Window.Current.Activate();
        }

        public static void ShowCollection()
        {
            var page = new CollectionPage();
            if (_sampleData == null) _sampleData = new SampleDataSource(page.BaseUri);
            page.Items = _sampleData.GroupedCollections.Select((obj) => (Object)obj);
            Window.Current.Content = page;
        }

        public static void ShowSplit(IGroupInfo collection)
        {
            var page = new SplitPage();
            if (_sampleData == null) _sampleData = new SampleDataSource(page.BaseUri);
            if (collection == null) collection = _sampleData.GroupedCollections.First();
            page.Items = collection;
            page.Context = collection.Key;
            Window.Current.Content = page;
        }

        async protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            base.OnSearchActivated(args);
            await EnsureSplitPageAsync(args);
            ((SplitPage)Window.Current.Content).Search(args.QueryText);
        }

        async private Task EnsureSplitPageAsync(IActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // rehydrate any settings we need
            }

            if (Window.Current.Content == null)
            {
                await Task.Run(() =>
                    {
                        var page = new SplitPage();
                        page.Items = null;
                        page.Context = null;
                        Window.Current.Content = page;
                        Window.Current.Activate();
                    });
            }
        }

        /// <summary>
        /// TODO: Make ASYNC when MS fixes PasswordVault.FindAllByResource threading issues
        /// </summary>
        private static void LoginReddit()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://www.reddit.com/api/");

            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("api_type", "json");
            values.Add("user", Settings["UserName"].ToString());

            try
            {
                
                IReadOnlyList<IPasswordCredential> passwords = null;
                passwords = PasswordVault.FindAllByResource("redditMetro");


                foreach (var pass in passwords)
                {
                    if (pass.UserName == Settings["UserName"].ToString())
                    {
                        pass.RetrievePassword();
                        values.Add("passwd", pass.Password);
                        break;
                    }
                }

                FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                var response = client.PostAsync("login/" + Settings["UserName"].ToString(), content).Result;
                var stream = response.EnsureSuccessStatusCode().Content.ContentReadStream;
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(LoginResponse));
                var data = (LoginResponse)deserializer.ReadObject(stream);
                if (data.json.errors.Count == 0)
                {
                    App.modhash = data.json.data.modhash;
                    App.cookie = data.json.data.cookie;
                    App.isLoggedIn = true;
                }
                else
                {
                    // password was probably bad, so we'll set it back to ""
                    App.isLoggedIn = false;
                    Settings["Password"] = "";
                    var passes = PasswordVault.FindAllByResource("redditMetro");
                    foreach (var i in passes)
                    {
                        if (i.UserName == Settings["UserName"].ToString())
                            PasswordVault.Remove(i);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
