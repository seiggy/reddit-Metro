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

        public App()
        {
            InitializeComponent();
        }

        public static void LoadSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Settings = localSettings.CreateContainer(CONTAINER_NAME, ApplicationDataCreateDisposition.Always).Values;
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
                var page = new SplitPage();
                page.Items = null;
                page.Context = null;
                Window.Current.Content = page;
                Window.Current.Activate();
            }
        }
    }
}
