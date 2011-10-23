using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using redditMetro.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace redditMetro
{
    public sealed partial class SplitPage
    {
        public SplitPage()
        {
            InitializeComponent();
            App.SearchPane = SearchPane.GetForCurrentView();
            App.SearchPane.QuerySubmitted += new TypedEventHandler<SearchPane, SearchPaneQuerySubmittedEventArgs>(SearchPane_QuerySubmitted);
            ShareSourceLoad();
            ShareButton.Click += new RoutedEventHandler(ShareButton_Click);
        }
        
        #region Search Code
        void SearchPane_QuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            // search something
        }

        public void Search(string queryText)
        {
            // do a search here for the text sent
        }
        #endregion

        #region Sharing Code
        public void ShareSourceLoad()
        {
            try
            {
                DataTransferManager datatransferManager;
                datatransferManager = DataTransferManager.GetForCurrentView();
                datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(datatransferManager_DataRequested);
            }
            catch (Exception)
            {
                // it keeps throwing some exception...not sure why
                // found the exception. Having two versions of the same app installed = bad juju
                // swallow here just incase something bad happens anyways
            }
        }

        void datatransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            ListingItem item = ItemListView.SelectedItem as ListingItem;
            if (item == null)
            {
                args.Request.FailWithDisplayText("You must select a story from the left to share first.");
            }
            args.Request.Data.Properties.Title = "Link Shared from reddit Metro";
            args.Request.Data.Properties.Description = item.data.title;
            args.Request.Data.Properties.ApplicationName = "reddit Metro";
            if (item.data.is_self)
            {
                args.Request.Data.SetHtml(item.data.selftext_html);
            }
            else
            {
                args.Request.Data.SetUri(new Uri(item.data.url));
            }
        }

        void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        #endregion

        private bool _ignoreReentrancy;
        public Subreddit selectedSubreddit { get; set; }
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsSnappedOrPortrait() || _ignoreReentrancy) return;

            //Temporary: Workaround for view state change disturbing selection
            _ignoreReentrancy = true;
            var selectedItem = ItemListView.SelectedItem;

            this.SetCurrentViewState(this);
            ItemListView.SelectedItem = selectedItem;

            // Need to think and be careful here about how I load the comments data. 
            // I don't want to poll for too much data from reddit, as to cause extra load.
            LoadComments();

            _ignoreReentrancy = false;
        }

        void LoadComments()
        {

        }

        void LinkClicked(object sender, RoutedEventArgs e)
        {
            var selectedItem = ItemListView.SelectedItem as ListingItem;
            Launcher.LaunchDefaultProgram(new Uri(selectedItem.data.url, UriKind.Absolute), LaunchingOptions.None);
        }

        void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.ShowCollection();
        }

        private Object _context;
        public Object Context
        {
            get
            {
                return this._context;
            }

            set
            {
                this._context = value;
                this.PageTitle.DataContext = value;
            }
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
                this.CollectionViewSource.Source = value;
                if (!this.IsSnappedOrPortrait()) this.CollectionViewSource.View.MoveCurrentToFirst();
            }
        }

        private bool IsSnappedOrPortrait()
        {
            var state = GetViewState();
            return !(state.Equals("Full") || state.Equals("Fill"));
        }

        // View state management for switching among Full, Fill, Snapped, and Portrait states.
        // Complicated by additional states representing two logical pages (master + detail) for
        // portrait and snapped states.

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

            if (App.PreviousSubreddit == null || App.PreviousSubreddit != App.SelectedSubreddit || App.LastRefresh < DateTime.Now.Subtract(App.RefreshInterval))
            {
                App.JsonClient = new HttpClient();
                var response = App.JsonClient.GetAsync("http://www.reddit.com" + App.SelectedSubreddit.data.url + ".json").Result.Content;
                LoadCollection(response);
                App.LastRefresh = DateTime.Now;
            }
            else
            {
                Items = App.Posts.children;
                PageTitle.Text = App.SelectedSubreddit.data.url;
            }
            SetCurrentViewState(this);
        }

        private void LoadCollection(HttpContent response)
        {
            App.PreviousSubreddit = App.SelectedSubreddit;
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ListingBaseResponse));
            StreamReader sr = new StreamReader(response.ContentReadStream);
            string dataString = sr.ReadToEnd();
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            var data = (ListingBaseResponse)deserializer.ReadObject(response.ContentReadStream);

            foreach (var o in data.data.children)
            {
                if (o.data.thumbnail.Contains("http:"))
                    continue;
                else if (!o.data.is_self && !string.IsNullOrEmpty(o.data.thumbnail))
                    o.data.thumbnail = "http://www.reddit.com" + o.data.thumbnail;
                else if (o.data.is_self)
                    o.data.thumbnail = this.BaseUri.AbsoluteUri + "/Images/self_default2.png";
                else
                    o.data.thumbnail = this.BaseUri.AbsoluteUri + "/Images/noimage.png";
            }
            PageTitle.Text = App.SelectedSubreddit.data.url;
            App.Posts = data.data;
            Items = data.data.children;
            //CollectionViewSource.Source = data.data.children;
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
            var hasSelection = ItemListView.SelectedItem != null;

            var orientation = DisplayProperties.CurrentOrientation;
            if (orientation == DisplayOrientations.Portrait ||
                orientation == DisplayOrientations.PortraitFlipped)
            {
                return hasSelection ? "PortraitDetail" : "Portrait";
            }

            var layout = ApplicationLayout.Value;
            if (layout == ApplicationLayoutState.Filled) return "Fill";
            if (layout == ApplicationLayoutState.Snapped)
            {
                return hasSelection ? "SnappedDetail" : "Snapped";
            }
            return "Full";
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            App.ShowCollection();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }
    }
}
