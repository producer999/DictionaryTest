using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DictionaryTest
{
    public class GoogleTranslationResponse
    {
        public GoogleTranslationData data;

        public GoogleTranslationResponse()
        {

        }
        public GoogleTranslationResponse(GoogleTranslationData res)
        {
            data = res;
        }

    }

    public class GoogleTranslationData
    {
        public GoogleTranslation[] translations;

        public GoogleTranslationData()
        {

        }
        public GoogleTranslationData(GoogleTranslation[] trans)
        {
            translations = trans;
        }
    }

    public class GoogleTranslation
    {
        public string translatedText;

        public GoogleTranslation()
        {
            translatedText = "";
        }
        public GoogleTranslation(string trans)
        {
            translatedText = trans;
        }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        private DBHelper dbh = new DBHelper();
        private Definition currentDef;

        public DetailsPage()
        {
            this.InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            currentDef.term = Details_Word.Text;

            if (Details_UserDefinition.Text != "")
                currentDef.definition = Details_UserDefinition.Text;

            if (Details_Example.Text != "")
                currentDef.example = Details_Example.Text;

            dbh.UpdateDefinition(currentDef);
            Status_updateBtn.Text = "Update Successful!";
            updateBtn.IsEnabled = false;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var def = (Definition)e.Parameter;
            currentDef = def;

            Details_Word.Text = def.term;
            Details_DictTranslation.Text = def.importedDefinition;
            if(!String.IsNullOrEmpty(def.googleDefinition))
            {
                Details_GoogleTranslation.Text = def.googleDefinition;
            }
            if (!String.IsNullOrEmpty(def.example))
            {
                Details_Example.Text = def.example;
            }

        }

        private void Definition_Edited(object sender, TextChangedEventArgs e)
        {
            updateBtn.IsEnabled = true;
        }

        private async void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Details_Word.Text != "")
            {
                Uri uri = new Uri("https://translation.googleapis.com/language/translate/v2?key=AIzaSyC1uP0Uw1jEoDFv61cIzLVK2bP4J3E8vaw&source=hi&target=en&q=" + Details_Word.Text);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage responseGet = await client.GetAsync(uri);
                    string response = await responseGet.Content.ReadAsStringAsync();
                    GoogleTranslationResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTranslationResponse>(response);

                    currentDef.googleDefinition = res.data.translations[0].translatedText;

                    dbh.UpdateDefinition(currentDef);
                }

                Details_GoogleTranslation.Text = currentDef.googleDefinition;
            }
            else
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Hindi Word field must not be blank!");
                await md.ShowAsync();
            }
        }

        private void DeleteOneButton_Click(object sender, RoutedEventArgs e)
        {
            dbh.DeleteDefinition(currentDef.id);
            Frame.Navigate(typeof(MainPage));
        }
    }
}
