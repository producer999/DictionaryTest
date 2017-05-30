using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DictionaryTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DBHelper dbh = new DBHelper();
        private ObservableCollection<Definition> dictionary = new ObservableCollection<Definition>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AddDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (WordTextBox.Text != "")
            {
                Uri uri = new Uri("https://translation.googleapis.com/language/translate/v2?key=AIzaSyC1uP0Uw1jEoDFv61cIzLVK2bP4J3E8vaw&source=hi&target=en&q=" + WordTextBox.Text);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage responseGet = await client.GetAsync(uri);
                    string response = await responseGet.Content.ReadAsStringAsync();
                    GoogleResultsBox.Text = response;
                    GoogleTranslationResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTranslationResponse>(response);
                    dbh.Insert(new Definition(WordTextBox.Text, DefTextBox.Text, res.data.translations[0].translatedText));
                }

                WordTextBox.Text = "";
                DefTextBox.Text = "";
                RefreshDefinitions();
            }
            else
            {
                MessageDialog md = new MessageDialog("Hindi Word field must not be blank!");
                await md.ShowAsync();
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            dictionary = dbh.GetAllDefinitions();
            if(dictionary.Count > 0)
            {
                deleteBtn.IsEnabled = true;
            }
            DefList.ItemsSource = dictionary.OrderByDescending(i => i.id).ToList();
        }

        private void RefreshDefinitions()
        {
            dictionary = dbh.GetAllDefinitions();
            if (dictionary.Count > 0)
            {
                deleteBtn.IsEnabled = true;
            }
            DefList.ItemsSource = dictionary.OrderByDescending(i => i.id).ToList();
        }

        private void ClearDefinitions_Click(object sender, RoutedEventArgs e)
        {
            dbh.DeleteAllDefinitions();
            dictionary.Clear();
            deleteBtn.IsEnabled = false;
            DefList.ItemsSource = dictionary;
        }

        private void DefList_Selected(object sender, SelectionChangedEventArgs e)
        {
            if(DefList.SelectedIndex != -1)
            {
                Definition selectedDef = DefList.SelectedItem as Definition;

                Frame.Navigate(typeof (DetailsPage), selectedDef);
            }
                   
        }
    }
}
