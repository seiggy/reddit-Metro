using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Expression.Blend.SampleData.SampleDataSource
{
    using System;

    // To significantly reduce the sample data footprint in your production application, you can set
    // the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class SampleDataSource { }
#else

    class SampleDataCollection : SampleDataItem, IGroupInfo
    {
        public Object Key
        {
            get { return this; }
        }

        private List<SampleDataItem> _itemCollection = new List<SampleDataItem>();

        public void Add(SampleDataItem item)
        {
            _itemCollection.Add(item);
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return _itemCollection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class SampleDataItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private SampleDataCollection _collection;
        public SampleDataCollection Collection
        {
            get
            {
                return this._collection;
            }

            set
            {
                if (this._collection != value)
                {
                    this._collection = value;
                    this.OnPropertyChanged("Collection");
                }
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return this._title;
            }

            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get
            {
                return this._subtitle;
            }

            set
            {
                if (this._subtitle != value)
                {
                    this._subtitle = value;
                    this.OnPropertyChanged("Subtitle");
                }
            }
        }

        private ImageSource _image = null;
        private Uri _imageBaseUri = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (_image == null && _imageBaseUri != null && _imagePath != null)
                {
                    _image = new BitmapImage(new Uri(_imageBaseUri, _imagePath));
                }
                return this._image;
            }

            set
            {
                if (this._image != value)
                {
                    this._image = value;
                    this._imageBaseUri = null;
                    this._imagePath = null;
                    this.OnPropertyChanged("Image");
                }
            }
        }

        public void SetImage(Uri baseUri, String path)
        {
            _image = null;
            _imageBaseUri = baseUri;
            _imagePath = path;
            this.OnPropertyChanged("Image");
        }

        private string _link = string.Empty;
        public string Link
        {
            get
            {
                return this._link;
            }

            set
            {
                if (this._link != value)
                {
                    this._link = value;
                    this.OnPropertyChanged("Link");
                }
            }
        }

        private string _category = string.Empty;
        public string Category
        {
            get
            {
                return this._category;
            }

            set
            {
                if (this._category != value)
                {
                    this._category = value;
                    this.OnPropertyChanged("Category");
                }
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        private string _content = string.Empty;
        public string Content
        {
            get
            {
                return this._content;
            }

            set
            {
                if (this._content != value)
                {
                    this._content = value;
                    this.OnPropertyChanged("Content");
                }
            }
        }
    }

    class SampleDataSource
    {
        public List<SampleDataCollection> GroupedCollections { get; private set; }

        private void AddCollection(String title, String subtitle, Uri baseUri, String imagePath, String link, String category, String description, String content)
        {
            var collection = new SampleDataCollection();
            collection.Title = title;
            collection.Subtitle = subtitle;
            collection.SetImage(baseUri, imagePath);
            collection.Link = link;
            collection.Category = category;
            collection.Description = description;
            collection.Content = content;
            GroupedCollections.Add(collection);
        }

        private void AddItem(String title, String subtitle, Uri baseUri, String imagePath, String link, String category, String description, String content)
        {
            SampleDataCollection lastCollection = GroupedCollections.LastOrDefault() as SampleDataCollection;

            var item = new SampleDataItem();
            item.Title = title;
            item.Subtitle = subtitle;
            item.SetImage(baseUri, imagePath);
            item.Link = link;
            item.Category = category;
            item.Description = description;
            item.Content = content;
            item.Collection = lastCollection;
            if (lastCollection != null)
            {
                lastCollection.Add(item);
            }
        }

        public SampleDataSource(Uri baseUri)
        {
            String LONG_LOREM_IPSUM = String.Format("{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            GroupedCollections = new List<SampleDataCollection>();

            AddCollection("Collection 1",
                    "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.adatum.com/",
                    "Pellentesque nam",
                    "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat",
                    LONG_LOREM_IPSUM);

            AddItem("Aliquam integer",
                    "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.adatum.com/",
                    "Pellentesque nam",
                    "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat",
                    LONG_LOREM_IPSUM);

            AddItem("Maecenas quisque class",
                    "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.adventure-works.com/",
                    "Suspendisse cras",
                    "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci",
                    LONG_LOREM_IPSUM);

            AddItem("Vivamus aliquam",
                    "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.adventure-works.com/",
                    "Curabitur vestibulum",
                    "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum",
                    LONG_LOREM_IPSUM);

            AddItem("Nam curae donec",
                    "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.alpineskihouse.com/",
                    "Consectetuer class",
                    "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant",
                    LONG_LOREM_IPSUM);

            AddCollection("Collection 2",
                    "Senectus sem lacus erat sociosqu eros suscipit primis nibh nisi nisl gravida torquent",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.baldwinmuseumofscience.com/",
                    "Phasellus duis",
                    "Est auctor inceptos congue interdum egestas scelerisque pellentesque fermentum ullamcorper cursus dictum lectus suspendisse condimentum libero vitae vestibulum lobortis ligula fringilla euismod class scelerisque feugiat habitasse diam litora adipiscing sollicitudin parturient hendrerit curae himenaeos imperdiet ullamcorper suspendisse nascetur hac gravida pharetra eget donec leo mus nec non malesuada vestibulum pellentesque elit penatibus vestibulum per condimentum porttitor sed adipiscing scelerisque ullamcorper etiam iaculis enim tincidunt erat parturient sem vestibulum eros",
                    LONG_LOREM_IPSUM);

            AddItem("Aenean mauris nullam cras",
                    "Senectus sem lacus erat sociosqu eros suscipit primis nibh nisi nisl gravida torquent",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.baldwinmuseumofscience.com/",
                    "Phasellus duis",
                    "Est auctor inceptos congue interdum egestas scelerisque pellentesque fermentum ullamcorper cursus dictum lectus suspendisse condimentum libero vitae vestibulum lobortis ligula fringilla euismod class scelerisque feugiat habitasse diam litora adipiscing sollicitudin parturient hendrerit curae himenaeos imperdiet ullamcorper suspendisse nascetur hac gravida pharetra eget donec leo mus nec non malesuada vestibulum pellentesque elit penatibus vestibulum per condimentum porttitor sed adipiscing scelerisque ullamcorper etiam iaculis enim tincidunt erat parturient sem vestibulum eros",
                    LONG_LOREM_IPSUM);

            AddItem("Duis sed aliquet aptent",
                    "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.blueyonderairlines.com/",
                    "Sed adipiscing",
                    "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac",
                    LONG_LOREM_IPSUM);

            AddItem("Auctor congue",
                    "Lacinia massa pede bibendum hac quam leo quis mus dictumst urna laoreet nec eleifend non tempor per metus",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.cpandl.com/",
                    "Maecenas",
                    "Ipsum torquent scelerisque consequat pellentesque vestibulum adipiscing aliquam sollicitudin leo ultrices pellentesque parturient justo ullamcorper vestibulum mus convallis vehicula lacus lorem consectetuer volutpat ornare aliquet dignissim suspendisse magna nec blandit pellentesque condimentum platea commodo massa elementum primis metus conubia scelerisque nunc vestibulum amet ullamcorper sollicitudin morbi pellentesque maecenas ante adipiscing non facilisis fermentum neque parturient netus consectetuer cubilia arcu nulla suspendisse dapibus rutrum praesent per vestibulum fringilla sapien pellentesque diam egestas vestibulum porta adipiscing eget accumsan condimentum",
                    LONG_LOREM_IPSUM);

            AddItem("Nunc cursus etiam",
                    "Natoque sed tempus posuere morbi facilisi tortor neque sem potenti cras pretium faucibus sit netus turpis vel nam duis habitant varius inceptos",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.cohovineyard.com/",
                    "Aenean",
                    "Parturient euismod purus vestibulum semper sed sociis feugiat taciti elit sem vestibulum sit habitasse scelerisque enim erat vel hendrerit tellus himenaeos adipiscing eros imperdiet malesuada bibendum sollicitudin risus velit gravida pellentesque nibh vitae iaculis class nam ullamcorper dictumst eleifend lacinia sed nisi parturient facilisi faucibus penatibus consectetuer habitant porttitor tincidunt dis tristique laoreet nisl curae pellentesque donec ultricies est vestibulum",
                    LONG_LOREM_IPSUM);

            AddItem("Dis fusce dictum",
                    "Interdum aenean nulla mauris lobortis nullam sed nunc aptent auctor nascetur congue pharetra dis",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.cohowinery.com/",
                    "Mauris consequat",
                    "Nunc hac odio suspendisse etiam sollicitudin natoque inceptos posuere vestibulum fusce venenatis potenti lorem morbi pretium leo vulputate mus nec interdum rhoncus curabitur phasellus tempor nulla adipiscing non proin tempus condimentum augue sodales parturient tortor per pellentesque scelerisque turpis consequat orci varius sed ullamcorper aenean mauris dolor pede consectetuer vestibulum suspendisse convallis lobortis sem quam dignissim nullam nascetur viverra pellentesque fames aliquam aptent quis sollicitudin pellentesque felis elementum consectetuer urna pharetra cras auctor integer condimentum duis quisque scelerisque ipsum sit vel placerat nunc",
                    LONG_LOREM_IPSUM);

            AddItem("Lectus amet",
                    "Porta est hac amet ante cursus leo purus mus risus placerat nec arcu dictum rhoncus non velit diam sodales pulvinar eget per vitae viverra sagittis sed",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.contoso.com/",
                    "Pellentesque",
                    "Vivamus aliquam justo vestibulum congue facilisis fermentum adipiscing aliquet blandit cursus commodo parturient nam ullamcorper pellentesque dictum amet pulvinar lacus vestibulum ante vestibulum lorem arcu magna sollicitudin massa sagittis metus conubia suspendisse fringilla morbi condimentum adipiscing habitasse neque scelerisque sed lectus cubilia diam parturient pellentesque dis eget elit senectus ullamcorper suspendisse condimentum sociosqu libero dapibus ligula hendrerit consectetuer himenaeos enim",
                    LONG_LOREM_IPSUM);

            AddCollection("Collection 3",
                    "Class aliquam curae donec etiam integer quisque nam maecenas cras vivamus duis sed dis aliquam aliquet praesent fusce",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.consolidatedmessenger.com/",
                    "Condimentum convallis",
                    "Imperdiet litora erat netus pellentesque egestas vestibulum euismod eros nibh nulla nisi porta feugiat scelerisque purus est sollicitudin luctus hac nisl ullamcorper vestibulum adipiscing magnis malesuada mattis mauris suspendisse suscipit leo parturient torquent vestibulum gravida pellentesque mollis consectetuer condimentum iaculis risus penatibus pellentesque lacinia ultrices vestibulum vehicula mus laoreet nunc velit porttitor tincidunt volutpat nec adipiscing tristique natoque scelerisque montes odio nostra maecenas ultricies non praesent venenatis accumsan per ullamcorper sollicitudin orci pede suspendisse condimentum posuere scelerisque ornare quam vulputate sed platea pellentesque parturient primis rutrum quis sem vestibulum curabitur sapien",
                    LONG_LOREM_IPSUM);

            AddItem("Maecenas aliquam class",
                    "Class aliquam curae donec etiam integer quisque nam maecenas cras vivamus duis sed dis aliquam aliquet praesent fusce",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.consolidatedmessenger.com/",
                    "Condimentum convallis",
                    "Imperdiet litora erat netus pellentesque egestas vestibulum euismod eros nibh nulla nisi porta feugiat scelerisque purus est sollicitudin luctus hac nisl ullamcorper vestibulum adipiscing magnis malesuada mattis mauris suspendisse suscipit leo parturient torquent vestibulum gravida pellentesque mollis consectetuer condimentum iaculis risus penatibus pellentesque lacinia ultrices vestibulum vehicula mus laoreet nunc velit porttitor tincidunt volutpat nec adipiscing tristique natoque scelerisque montes odio nostra maecenas ultricies non praesent venenatis accumsan per ullamcorper sollicitudin orci pede suspendisse condimentum posuere scelerisque ornare quam vulputate sed platea pellentesque parturient primis rutrum quis sem vestibulum curabitur sapien",
                    LONG_LOREM_IPSUM);

            AddItem("Nunc aenean",
                    "Accumsan est blandit bibendum lorem commodo amet ante dictumst mauris morbi nulla eleifend hac",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.fabrikam.com/",
                    "Scelerisque",
                    "Ullamcorper sit potenti semper consectetuer vel sociis taciti pellentesque nam sed dis bibendum vitae est dictumst vestibulum adipiscing eleifend sollicitudin class tellus curae donec suspendisse pretium condimentum etiam phasellus rhoncus consequat parturient tempor facilisi urna cras convallis sodales scelerisque tempus viverra pellentesque duis faucibus consectetuer fusce tortor habitant lorem ullamcorper turpis morbi nulla dignissim hac inceptos vestibulum proin interdum lobortis augue leo elementum nascetur aliquam varius vestibulum mus nec suspendisse condimentum dolor scelerisque nunc pharetra aenean amet facilisis fames placerat non mauris per ullamcorper ante nullam pulvinar fermentum integer fringilla",
                    LONG_LOREM_IPSUM);

            AddItem("Arcu conubia leo",
                    "Facilisi proin faucibus diam nullam aptent eget elit cubilia dapibus auctor congue enim cursus dictum lectus erat libero eros mus augue",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.fourthcoffee.com/",
                    "Dignissim",
                    "Aptent quisque pellentesque auctor arcu congue felis habitasse diam hendrerit sagittis cursus vivamus dictum sollicitudin sed lectus sem eget adipiscing libero elit pellentesque sit aliquam suspendisse vel condimentum ligula nam parturient sed senectus aliquet sociosqu blandit litora commodo consectetuer enim dis conubia suscipit torquent est himenaeos pellentesque imperdiet scelerisque hac sollicitudin cubilia dapibus vestibulum ipsum justo egestas pellentesque consectetuer luctus magnis mattis lacus leo ullamcorper euismod vestibulum pellentesque suspendisse ultrices adipiscing condimentum erat mus malesuada sollicitudin vehicula mauris pellentesque scelerisque ullamcorper feugiat nec mollis eros montes penatibus suspendisse lorem magna consectetuer nostra volutpat porttitor parturient vestibulum nibh maecenas vestibulum praesent",
                    LONG_LOREM_IPSUM);

            AddItem("Habitant egestas ligula litora",
                    "Nec nibh euismod dolor fames inceptos interdum lobortis luctus non per nisi nisl nunc magnis felis mattis feugiat nascetur mauris mollis gravida sed iaculis lacinia montes nostra",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.graphicdesigninstitute.com/",
                    "Praesent",
                    "Condimentum non adipiscing massa metus per ornare platea sed primis parturient nisi pellentesque sem gravida scelerisque rutrum sollicitudin pellentesque accumsan ullamcorper consectetuer tincidunt tristique ultricies sapien semper bibendum morbi pellentesque vestibulum sollicitudin venenatis sociis pellentesque suspendisse vulputate condimentum sit vel nam neque vestibulum scelerisque dictumst ullamcorper iaculis eleifend nisl consectetuer lacinia netus curabitur nunc adipiscing pellentesque taciti odio facilisi sollicitudin parturient suspendisse orci pede phasellus vestibulum laoreet faucibus quam habitant nulla natoque condimentum scelerisque inceptos interdum quis vestibulum sed",
                    LONG_LOREM_IPSUM);

            AddItem("Sem laoreet ornare",
                    "Sit platea odio natoque primis orci ipsum posuere justo potenti pretium rutrum sapien lacus lorem magna pharetra pede rhoncus semper",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.humongousinsurance.com/",
                    "Ullamcorper",
                    "Ullamcorper pellentesque adipiscing tellus tempor posuere potenti tempus pretium consequat dis lobortis nascetur porta urna purus risus cras convallis velit parturient tortor suspendisse turpis pharetra vestibulum consectetuer rhoncus vestibulum dignissim duis condimentum placerat est adipiscing pellentesque sollicitudin sodales parturient elementum nunc viverra pellentesque consectetuer amet hac facilisis vitae fermentum fringilla class aliquam integer quisque curae vivamus leo aliquam vestibulum habitasse pulvinar mus sagittis scelerisque aliquet ante pellentesque blandit senectus arcu donec vestibulum varius sollicitudin diam aenean commodo pellentesque etiam conubia nec adipiscing parturient non mauris vestibulum nullam per ullamcorper fusce consectetuer sociosqu vestibulum sed eget sem suspendisse elit cubilia",
                    LONG_LOREM_IPSUM);

            AddItem("Sodales quam viverra vel",
                    "Placerat quis urna nam aliquam integer massa quisque cras pulvinar sed metus dis morbi vivamus duis sagittis senectus neque est sociosqu hac nunc aliquam amet netus",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.litwareinc.com/",
                    "Accumsan",
                    "Pellentesque enim lorem adipiscing dapibus aptent suscipit sit egestas auctor sollicitudin vel nam erat morbi hendrerit condimentum congue euismod feugiat parturient scelerisque pellentesque himenaeos nulla proin vestibulum sed cursus ullamcorper eros torquent augue gravida dictum lectus suspendisse dis dolor vestibulum condimentum imperdiet scelerisque adipiscing ullamcorper fames consectetuer libero iaculis lacinia felis parturient ligula",
                    LONG_LOREM_IPSUM);

            AddItem("Sociis ante arcu leo",
                    "Suscipit aliquet torquent diam ultrices eget mus nulla nec taciti porta vehicula elit tellus purus volutpat enim tempor tempus risus blandit maecenas praesent commodo non",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.lucernepublishing.com/",
                    "Aliquam curae",
                    "Malesuada vestibulum est ipsum ultrices laoreet justo penatibus vehicula vestibulum volutpat lacus litora adipiscing maecenas suspendisse pellentesque condimentum scelerisque hac parturient porttitor leo sollicitudin nibh vestibulum mus luctus magnis tincidunt ullamcorper tristique mattis nisi ultricies vestibulum lorem nisl magna praesent pellentesque nec mauris consectetuer massa accumsan bibendum natoque mollis dictumst posuere",
                    LONG_LOREM_IPSUM);

            AddCollection("Collection 4",
                    "Tortor cubilia velit sed erat vitae eros sem nibh turpis dapibus varius aenean nisi",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.margiestravel.com/",
                    "Suspendisse condimentum",
                    "Venenatis potenti adipiscing eleifend non parturient nunc per suspendisse pretium pellentesque montes condimentum scelerisque sollicitudin ullamcorper odio orci suspendisse facilisi condimentum vulputate metus sed vestibulum morbi pede vestibulum curabitur neque faucibus pellentesque adipiscing quam netus quis phasellus consequat convallis sem urna sit nostra cras vel parturient rhoncus nam nulla dignissim ornare platea sodales consectetuer elementum sed porta facilisis scelerisque vestibulum habitant primis viverra fermentum ullamcorper pellentesque inceptos vestibulum interdum suspendisse duis lobortis nascetur adipiscing aliquam parturient purus nunc sollicitudin dis condimentum est integer pellentesque scelerisque risus quisque ullamcorper vivamus velit consectetuer pellentesque vestibulum suspendisse aliquam vestibulum fringilla adipiscing vitae hac",
                    LONG_LOREM_IPSUM);

            AddItem("Conubia per",
                    "Tortor cubilia velit sed erat vitae eros sem nibh turpis dapibus varius aenean nisi",
                    baseUri, "SampleData/Images/MediumGray.png",
                    "http://www.margiestravel.com/",
                    "Suspendisse condimentum",
                    "Venenatis potenti adipiscing eleifend non parturient nunc per suspendisse pretium pellentesque montes condimentum scelerisque sollicitudin ullamcorper odio orci suspendisse facilisi condimentum vulputate metus sed vestibulum morbi pede vestibulum curabitur neque faucibus pellentesque adipiscing quam netus quis phasellus consequat convallis sem urna sit nostra cras vel parturient rhoncus nam nulla dignissim ornare platea sodales consectetuer elementum sed porta facilisis scelerisque vestibulum habitant primis viverra fermentum ullamcorper pellentesque inceptos vestibulum interdum suspendisse duis lobortis nascetur adipiscing aliquam parturient purus nunc sollicitudin dis condimentum est integer pellentesque scelerisque risus quisque ullamcorper vivamus velit consectetuer pellentesque vestibulum suspendisse aliquam vestibulum fringilla adipiscing vitae hac",
                    LONG_LOREM_IPSUM);

            AddItem("Mauris accumsan egestas euismod",
                    "Bibendum feugiat nullam nisl nunc sit vel gravida dictumst odio eleifend facilisi nam sed dis est aptent iaculis auctor class hac curae faucibus",
                    baseUri, "SampleData/Images/DarkGray.png",
                    "http://www.northwindtraders.com/",
                    "Nullam dis",
                    "Amet habitasse aliquet blandit pharetra parturient vestibulum vestibulum placerat sollicitudin adipiscing leo hendrerit condimentum scelerisque pulvinar pellentesque commodo parturient sagittis conubia vestibulum rutrum sapien consectetuer ante vestibulum himenaeos semper imperdiet arcu senectus ullamcorper pellentesque suspendisse class sociosqu mus cubilia nec dapibus adipiscing sociis suscipit torquent condimentum taciti sollicitudin pellentesque consectetuer scelerisque diam pellentesque malesuada ultrices egestas tellus eget euismod tempor non elit tempus ullamcorper parturient curae enim vestibulum sollicitudin penatibus pellentesque suspendisse consectetuer porttitor erat vestibulum feugiat adipiscing parturient per condimentum scelerisque tincidunt tortor ullamcorper pellentesque vehicula donec turpis gravida suspendisse iaculis volutpat",
                    LONG_LOREM_IPSUM);

            AddItem("Congue orci donec",
                    "Cursus leo habitant pede mus nec non inceptos quam interdum etiam quis fusce lorem per dictum lectus lobortis sed morbi nulla lacinia",
                    baseUri, "SampleData/Images/LightGray.png",
                    "http://www.proseware.com/",
                    "Est",
                    "Eros sed varius lacinia laoreet vestibulum etiam tristique maecenas sem aenean sit sollicitudin fusce lorem praesent condimentum pellentesque accumsan mauris natoque posuere morbi potenti vel consectetuer scelerisque ultricies pellentesque nullam pretium vestibulum bibendum aptent ullamcorper venenatis adipiscing auctor sollicitudin nam dictumst rhoncus nulla suspendisse congue cursus condimentum parturient proin vulputate sodales viverra dictum sed nibh curabitur scelerisque ullamcorper aliquam eleifend suspendisse facilisi vestibulum nisi faucibus habitant integer phasellus lectus consequat vestibulum inceptos dis interdum condimentum pellentesque nisl libero",
                    LONG_LOREM_IPSUM);
        }
    }
#endif
}
