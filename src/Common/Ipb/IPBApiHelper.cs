﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BioEngine.Common.DB;
using BioEngine.Common.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BioEngine.Common.Ipb
{
    public class IPBApiHelper
    {
        private readonly BWContext _dbContext;
        private readonly string _apiUrl;
        private readonly int _ipbNewsForumId;
        private readonly HttpClient _client;

        public IPBApiHelper(IConfigurationRoot configuration, BWContext dbContext)
        {
            _dbContext = dbContext;
            var apiKey = configuration["BE_IPB_API_KEY"];
            _apiUrl = configuration["BE_IPB_API_URL"];
            _ipbNewsForumId = int.Parse(configuration["BE_IPB_NEWS_FORUM_ID"]);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + Base64Encode(apiKey) + ":");
        }

        private async Task<HttpResponseMessage> DoApiRequest(string method, object data)
        {
            var url = _apiUrl + method;

            var response = await _client.PostAsync(url,
                new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            return response;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public async Task<bool> CreateOrUpdateNewsTopic(News news)
        {
            if (news.ForumTopicId == 0)
            {
                var topicCreateResponse = await DoApiRequest("/forums/topics",
                    new
                    {
                        forum = _ipbNewsForumId,
                        author = news.AuthorId,
                        title = news.Title,
                        hidden = news.Pub == 1 ? 0 : 1,
                        pinned = news.Sticky == 1 ? 1 : 0,
                        post = GetPostContent(news)
                    });
                var response = await topicCreateResponse.Content.ReadAsStringAsync();
                if (topicCreateResponse.IsSuccessStatusCode)
                {
                    var topicCreateData =
                        JObject.Parse(response);
                    news.ForumTopicId = topicCreateData.Value<int>("id");
                    news.ForumPostId = topicCreateData["firstPost"].Value<int>("id");
                    _dbContext.Update(news);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                throw new Exception($"Can't create topic: {response}");
            }
            var topicTitleUpdateResponse = await DoApiRequest("/forums/topics/" + news.ForumTopicId,
                new
                {
                    title = news.Title,
                });
            if (!topicTitleUpdateResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Can't update topic title: {await topicTitleUpdateResponse.Content.ReadAsStringAsync()}");
            }

            var topicStatusUpdateResponse = await DoApiRequest("/forums/topics/" + news.ForumTopicId,
                new
                {
                    hidden = news.Pub == 1 ? 0 : 1,
                    pinned = news.Sticky == 1 ? 1 : 0
                });
            if (topicStatusUpdateResponse.IsSuccessStatusCode)
            {
                var postUpdateResponse = await DoApiRequest("/forums/posts/" + news.ForumPostId,
                    new
                    {
                        post = GetPostContent(news)
                    });
                if (!postUpdateResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Can't update post content: {await postUpdateResponse.Content.ReadAsStringAsync()}");
                }
            }
            return true;
        }

        private static string GetPostContent(News news)
        {
            var postContent = news.ShortText;
            if (news.AddText.Length > 0)
            {
                var addText = "<div class=\"ipsSpoiler\" data-ipsspoiler=\"\">" +
                              "<div class=\"ipsSpoiler_header\">" +
                              "<span>Скрытый текст</span>" +
                              "</div>" +
                              "<div class=\"ipsSpoiler_contents\">" +
                              $"{news.AddText}" +
                              "</div>" +
                              "</div>";

                postContent += addText;
            }

            postContent = postContent.Replace("<ul>", "<ul class=\"bbc\">");
            postContent = postContent.Replace("<ol>", "<ul class=\"bbcol decimal\">");
            postContent = postContent.Replace("</ol>", "</ul>");

            return postContent;
        }
    }
}