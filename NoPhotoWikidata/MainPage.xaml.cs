namespace NoPhotoWikidata
{
    public partial class MainPage : ContentPage
    {
        private readonly AppSettings context;

        public MainPage()
        {
            InitializeComponent();
            context = new AppSettings();
            BindingContext = context;
        }
    }
}
