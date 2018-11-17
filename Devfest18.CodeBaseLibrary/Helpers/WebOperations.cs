using Devfest18.CodeBaseLibrary.Commons;
using Devfest18.CodeBaseLibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Devfest18.CodeBaseLibrary.Helpers
{
    public class WebOperations
    {
        private static async Task<string> GetNewsJSONAsync(NameValueCollection parameters, string apiURL)
        {
            string jsonResponse = string.Empty;

            using (var webClient = new WebClient())
            {
                webClient.QueryString.Add(parameters);
                jsonResponse = await webClient.DownloadStringTaskAsync(apiURL);
            }

            return jsonResponse; 
        }

        public static async Task<List<NewsArticle>> GetNewsAsync(string searchParam)
        {
            var newsArticles = new List<NewsArticle>();
            var parameters = new NameValueCollection();
            string apiURL = Constants.NewsAPIBaseURL;

            if (!string.IsNullOrEmpty(searchParam))
            {
                apiURL += "everything";
                parameters.Add(new NameValueCollection()
                {
                    {"apiKey", Constants.APIKey},
                    {"language", "en"},
                    {"q", searchParam}
                });
            }
            else
            {
                apiURL += "top-headlines";
                parameters.Add(new NameValueCollection()
                {
                    {"apiKey", Constants.APIKey},
                    {"country", "us"}                    
                });
            }

            string jsonResponse = await GetNewsJSONAsync(parameters, apiURL);

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var response = JsonConvert.DeserializeObject<object>(jsonResponse);
                var articles = ((JProperty)((JContainer)response).Last).Value.ToList();

                if (articles != null && articles.Any())
                {
                    foreach (var article in articles)
                    {
                        try
                        {
                            var newsArticle = new NewsArticle()
                            {
                                Author = (((JObject)article).GetValue("author").ToString() == null) ? string.Empty : ((JObject)article).GetValue("author").ToString(),
                                ContentURL = (((JObject)article).GetValue("url").ToString() == null) ? string.Empty : ((JObject)article).GetValue("url").ToString(),
                                Description = (((JObject)article).GetValue("description").ToString() == null) ? string.Empty : ((JObject)article).GetValue("description").ToString(),
                                PublishDate = DateTime.Parse( ((JObject)article).GetValue("publishedAt").ToString()),
                                Source = (((JObject)((JObject)article).GetValue("source")).GetValue("name").ToString() == null) ? string.Empty : ((JObject)((JObject)article).GetValue("source")).GetValue("name").ToString(),
                                ThumbnailURL = (string.IsNullOrEmpty(((JObject)article).GetValue("urlToImage").ToString())) ? Constants.DummyNewsImageURL : ((JObject)article).GetValue("urlToImage").ToString(),
                                Title = (((JObject)article).GetValue("title").ToString() == null) ? string.Empty : ((JObject)article).GetValue("title").ToString()
                            };

                            newsArticles.Add(newsArticle);
                        }
                        catch (Exception)
                        {
                            // Do nothing, iterate to next item
                            continue;
                        }
                    }
                }
            }

            return newsArticles.OrderByDescending(x => x.PublishDate).ToList();
        }
    }
}
