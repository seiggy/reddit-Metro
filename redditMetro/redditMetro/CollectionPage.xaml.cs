using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using redditMetro.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Search;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ApplicationSettings;
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
            BackButton.IsEnabled = false;
            App.SearchPane = SearchPane.GetForCurrentView();
        }

        void ItemView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Construct the appropriate destination page and set its context appropriately
            var selectedItem = (sender as Selector).SelectedItem as IGroupInfo;
            var subreddit = (sender as Selector).SelectedItem as Subreddit;
            App.SelectedSubreddit = subreddit;
            App.ShowSplit(selectedItem);
        }

        private IEnumerable<Object> _items;
        public IEnumerable<Object> Items
        {
            get
            {
                return this._items;
            }

            set
            {
                this._items = value;
                CollectionViewSource.Source = value;
            }
        }

        // View state management for switching among Full, Fill, Snapped, and Portrait states

        private DisplayPropertiesEventHandler _displayHandler;
        private TypedEventHandler<ApplicationLayout, ApplicationLayoutChangedEventArgs> _layoutHandler;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged += _layoutHandler;

            if (App.Subreddits == null || App.Subreddits.Count == 0)
            {
                var client = new HttpClient();
                var response = client.GetAsync("http://www.reddit.com/reddits.json").Result.Content;
                LoadCollection(response);
            }
            else
            {
                CollectionViewSource.Source = App.Subreddits;
            }

            SettingsPane settingsPane = SettingsPane.GetForCurrentView();
            
            if(settingsPane.ApplicationCommands.Count == 0)
                settingsPane.ApplicationCommands.Add(new SettingsCommand(KnownSettingsCommand.Account, new UICommandInvokedHandler(AccountCommandHandler)));
            
            SetCurrentViewState(this);
        }

        public void AccountCommandHandler(IUICommand command)
        {
            
        }

        private void LoadCollection(HttpContent messageTask)
        {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(SubredditResponse));
            var data = (SubredditResponse)deserializer.ReadObject(messageTask.ContentReadStream);
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
            App.Subreddits = data.data.children;
            CollectionViewSource.Source = data.data.children;
        }

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
    }
}
