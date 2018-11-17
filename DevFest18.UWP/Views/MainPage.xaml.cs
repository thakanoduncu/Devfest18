using Devfest18.CodeBaseLibrary.Helpers;
using Devfest18.CodeBaseLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DevFest18.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<NewsArticle> _newsArticles;
        private string _searchQuery;
        private ICommand _searchCommand;
        private Visibility _progressRingVisibility;

        public List<NewsArticle> NewsArticles
        {
            get { return _newsArticles; }
            set
            {
                _newsArticles = value;
                RaisePropertyChanged("NewsArticles");
            }
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                RaisePropertyChanged("SearchQuery");
            }
        }
  
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new CommandHandler(() => SearchNews(), true));
            }
        }

        public Visibility ProgressRingVisibility
        {
            get { return _progressRingVisibility; }
            set
            {
                _progressRingVisibility = value;
                RaisePropertyChanged("ProgressRingVisibility");
            }
        }

        protected void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MainPage()
        {
            this.InitializeComponent();
            ProgressRingVisibility = Visibility.Collapsed;
        }

        private async void SearchNews()
        {
            ProgressRingVisibility = Visibility.Visible;

            var newsArticles = await WebOperations.GetNewsAsync(SearchQuery);

            if (newsArticles != null && newsArticles.Any())
            {
                NewsArticles = newsArticles;
            }
            else
            {
                NewsArticles = new List<NewsArticle>();
                await new MessageDialog("No result found", "Bad News!").ShowAsync();
            }
         
            ProgressRingVisibility = Visibility.Collapsed;           
        }

        private async void NewsArticles_ItemClick(object sender, ItemClickEventArgs e)
        {
            var newsArticle = e.ClickedItem as NewsArticle;
            await Windows.System.Launcher.LaunchUriAsync(new Uri(newsArticle.ContentURL));
        }
    }
}
