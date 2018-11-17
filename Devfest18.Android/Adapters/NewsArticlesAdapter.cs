using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Devfest18.Droid;
using Devfest18.CodeBaseLibrary.Models;
using Android.Graphics;
using Devfest18.CodeBaseLibrary.Helpers;
using System.Threading.Tasks;
using System.Net;
using Java.Lang;

namespace Devfest18.Ddroid.Adapters
{
    public class NewsArticlesAdapter : BaseAdapter<NewsArticle>
    {
        private Activity _context;
        private List<NewsArticle> _newsArticles;

        public NewsArticlesAdapter(Activity context, List<NewsArticle> newsArticles)
        {
            this._context = context;
            this._newsArticles = newsArticles;
        }

        public override NewsArticle this[int position] => _newsArticles[position];

        public override int Count => _newsArticles.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.news_item_row, null); 
            }

            var newsArticle = _newsArticles.ElementAt(position);
            convertView.FindViewById<TextView>(Resource.Id.title).Text = newsArticle.Title;
            convertView.FindViewById<TextView>(Resource.Id.source).Text = newsArticle.Source;
            convertView.FindViewById<TextView>(Resource.Id.description).Text = newsArticle.Description;
            convertView.FindViewById<TextView>(Resource.Id.publishDate).Text = newsArticle.PublishDate.ToString("dd.MM.yyyy HH:mm");

            Task.Run(() =>
            {
                Bitmap bmp = GetBitmap(newsArticle.ThumbnailURL);
                convertView.FindViewById<ImageView>(Resource.Id.thumbnail).SetImageBitmap(bmp);
            });

            return convertView;
        }

        private static Bitmap GetBitmap(string url)
        {
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    var originalBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                    float aspectRatio = originalBitmap.Width / (float)originalBitmap.Height;
                    int width = 480;
                    int height = Math.Round(width / aspectRatio);

                    return Bitmap.CreateScaledBitmap(
                        originalBitmap, width, height, false);
                }
            }

            return null;
        }
    }
}