using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public enum SortStatus
    {
        SortByIdAscend,
        SortByIdDescend,
        SortByHindiAscend,
        SortByHindiDescend,
        SortByDefAscend,
        SortByDefDescend
    }



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DBHelper dbh = new DBHelper();
        private ObservableCollection<Definition> dictionary = new ObservableCollection<Definition>();
        private SortStatus sortStatus = SortStatus.SortByIdDescend;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AddDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (WordTextBox.Text != "")
            {
                AddProgress.IsActive = true;
                AddProgress.Visibility = Visibility.Visible;

                Uri uri = new Uri("https://translation.googleapis.com/language/translate/v2?key=AIzaSyC1uP0Uw1jEoDFv61cIzLVK2bP4J3E8vaw&source=hi&target=en&q=" + WordTextBox.Text);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage responseGet = await client.GetAsync(uri);
                    string response = await responseGet.Content.ReadAsStringAsync();
                    //GoogleResultsBox.Text = response;
                    GoogleTranslationResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTranslationResponse>(response);
                    dbh.Insert(new Definition(WordTextBox.Text, DefTextBox.Text, res.data.translations[0].translatedText));
                }

                WordTextBox.Text = "";
                DefTextBox.Text = "";

                AddProgress.IsActive = false;
                AddProgress.Visibility = Visibility.Collapsed;

                GoogleResultsBox.Text = "Definition Added to Database.";
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
            DictionarySort(sortStatus);
            GoogleResultsBox.Text = "";
        }


        private void RefreshDefinitions()
        {
            dictionary = dbh.GetAllDefinitions();
            if (dictionary.Count > 0)
            {
                deleteBtn.IsEnabled = true;
            }
            DictionarySort(sortStatus);
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

        private void SortSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            var cbox = sender as ComboBox;
            if(cbox != null)
            {
                var item = (cbox.SelectedItem as ComboBoxItem).Content;
                var sortSelection = item as string;
                //Status_Sorting.Text = sortSelection;

                dictionary = dbh.GetAllDefinitions();
                
                switch(sortSelection)
                {
                    case "Id - Ascending":
                        DictionarySort(SortStatus.SortByIdAscend);
                        break;
                    case "Id - Descending":
                        DictionarySort(SortStatus.SortByIdDescend);
                        break;
                    case "Hindi Term - Ascending":
                        DictionarySort(SortStatus.SortByHindiAscend);
                        break;
                    case "Hindi Term - Descending":
                        DictionarySort(SortStatus.SortByHindiDescend);
                        break;
                    case "Translation - Ascending":
                        DictionarySort(SortStatus.SortByDefAscend);
                        break;
                    case "Translation - Descending":
                        DictionarySort(SortStatus.SortByDefDescend);
                        break;
                }
                
                  
            }
        
        }

        private void DictionarySort(SortStatus status)
        {
            switch (status)
            {
                case SortStatus.SortByIdAscend:
                    DefList.ItemsSource = dictionary.OrderBy(i => i.id).ToList();
                    sortStatus = SortStatus.SortByIdAscend;
                    break;
                case SortStatus.SortByIdDescend:
                    DefList.ItemsSource = dictionary.OrderByDescending(i => i.id).ToList();
                    sortStatus = SortStatus.SortByIdDescend;
                    break;
                case SortStatus.SortByHindiAscend:
                    DefList.ItemsSource = dictionary.OrderBy(i => i.term).ToList();
                    sortStatus = SortStatus.SortByHindiAscend;
                    break;
                case SortStatus.SortByHindiDescend:
                    DefList.ItemsSource = dictionary.OrderByDescending(i => i.term).ToList();
                    sortStatus = SortStatus.SortByHindiDescend;
                    break;
                case SortStatus.SortByDefAscend:
                    DefList.ItemsSource = dictionary.OrderBy(i => i.definition).ToList();
                    sortStatus = SortStatus.SortByDefAscend;
                    break;
                case SortStatus.SortByDefDescend:
                    DefList.ItemsSource = dictionary.OrderByDescending(i => i.definition).ToList();
                    sortStatus = SortStatus.SortByDefDescend;
                    break;
            }
        }

        private async void ImportDefinitions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.FileTypeFilter.Add(".txt");
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    ImportProgress.IsActive = true;
                    ImportProgress.Visibility = Visibility.Visible;

                    var lines = await FileIO.ReadLinesAsync(file);
                    List<string> importedList = lines.ToList<string>();
                    List<string> errors = new List<string>();

                    foreach (var i in importedList)
                    {
                        Debug.WriteLine("PROCESSING: " + i);
                        Debug.WriteLine("");
                        string[] delimiters = { "[", "]\"", "]”", "]{}\"", "]{}”", "\"(", "”(", ")<", ">;" };
                        string[] data = i.Split(delimiters, StringSplitOptions.None);

                        string term, def, posstr, ex;
                        string[] pos;
                        

                        if(data.Length == 6)
                        {
                         
                            term = data[1];

                            if (!String.IsNullOrEmpty(data[2]))
                                def = data[2];
                            else
                                def = "";

                            pos = data[3].Split(new char[] { ',' }, 1);
                            posstr = pos[0];

                            if (!String.IsNullOrEmpty(data[5]))
                                ex = data[5];
                            else
                                ex = "";

                            dbh.Insert(new Definition(term, def, posstr, ex));
                        }

                        else
                        {
                            errors.Add(i);
                            errors.Add("");
                            Debug.WriteLine("ERROR ON IMPORT - ADDED TO LOG");
                            Debug.WriteLine("");
                            for(int j=0;j<data.Length;j++)
                            {
                                Debug.WriteLine("data " + j + ": " + data[j]);
                                errors.Add("data " + j + ": " + data[j]);
                            }
                            errors.Add("");
                        }



                        Debug.WriteLine("");
                    }
                    //return true;

                    errors.ForEach(y => Debug.WriteLine(y));

                    RefreshDefinitions();

                    ImportProgress.IsActive = false;
                    ImportProgress.Visibility = Visibility.Collapsed;

                    
                }
                    
                
                else
                {
                    //return false;
                }
            }
            catch
            {
                ImportResultsBox.Text = "Error in opening file.\n\nOpenFile() Something went wrong.";
                ImportProgress.IsActive = false;
                ImportProgress.Visibility = Visibility.Collapsed;
                //return false;
            }
        }
    }
}
