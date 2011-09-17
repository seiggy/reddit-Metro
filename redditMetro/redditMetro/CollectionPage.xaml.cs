using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using redditMetro.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Graphics.Display;
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
            SetCurrentViewState(this);
        }

        private void LoadCollection(HttpContent messageTask)
        {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(SubredditResponse));
            var data = (SubredditResponse)deserializer.ReadObject(messageTask.ContentReadStream);
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
    }
}
