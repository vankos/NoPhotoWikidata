using System.Runtime.Serialization.DataContracts;

namespace NoPhotoWikidata
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            if(BindingContext is AppSettings context)
                context.OnError += OnError;
        }

        private async void OnError(object? sender, string errorMessage)
        {
            await DisplayAlert("No luck", errorMessage, "OK");
        }
    }
}
