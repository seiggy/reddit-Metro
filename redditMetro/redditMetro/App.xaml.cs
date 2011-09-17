using Expression.Blend.SampleData.SampleDataSource;
using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using redditMetro.Models;
using System.Collections;
using System.Collections.Generic;

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
        public static TimeSpan RefreshInterval = new TimeSpan(0, 0, 5, 0, 0);

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            LastRefresh = DateTime.Now;
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
    }
}
