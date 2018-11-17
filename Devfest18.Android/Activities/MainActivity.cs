using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Devfest18.Droid;
using Android.Views;
using Devfest18.CodeBaseLibrary.Helpers;
using System.Linq;
using Devfest18.Ddroid.Adapters;
using System.Collections.Generic;
using Devfest18.CodeBaseLibrary.Models;
using Android.Content;
using Android.Views.InputMethods;

namespace Devfest18.Ddroid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private EditText _searchNewsEditText;
        private Button _searchNewsButton;
        private ListView _newsArticlesListView;
        private ProgressBar _searchProgressBar;
        private NewsArticlesAdapter _newsArticlesAdapter;
        private List<NewsArticle> _newsArticles;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _searchNewsEditText = FindViewById<EditText>(Resource.Id.searchNewsEditText);
            _searchNewsButton = FindViewById<Button>(Resource.Id.searchNewsButton);
            _newsArticlesListView = FindViewById<ListView>(Resource.Id.newsArticlesListView);
            _searchProgressBar = FindViewById<ProgressBar>(Resource.Id.searchProgressBar);
         
            _searchNewsButton.Click += _searchNewsButton_Click;
            _newsArticlesListView.ItemClick += _newsArticlesListView_ItemClick;
        }

        private async void _searchNewsButton_Click(object sender, System.EventArgs e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            _searchProgressBar.Visibility = ViewStates.Visible;
            _searchProgressBar.Activated = true;
            _newsArticles = await WebOperations.GetNewsAsync(_searchNewsEditText.Text);

            if (_newsArticles != null && _newsArticles.Any())
            {
                _newsArticlesListView.Adapter = new NewsArticlesAdapter(this, _newsArticles);
            }
            else
            {
                _newsArticlesListView.Adapter = new NewsArticlesAdapter(this, new List<NewsArticle>());
                Toast.MakeText(this, "No result found", ToastLength.Long).Show();
            }

            _searchProgressBar.Visibility = ViewStates.Gone;
            _searchProgressBar.Activated = false;
        }

        private void _newsArticlesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var uri = Android.Net.Uri.Parse(_newsArticles[e.Position].ContentURL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
    }
}