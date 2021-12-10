using Best20HackerNews.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Best20HackerNews.Controllers
{

    [ApiController]
    [Route("")]
    public class StoryController : ControllerBase
    {
        private readonly ILogger<StoryController> _logger;
        private string getBestStoriesId = "https://hacker-news.firebaseio.com/v0/beststories.json";
        private string getStoryById = "https://hacker-news.firebaseio.com/v0/item/{0}.json";



        /// <summary>
        /// Get best 20 hacker news stories
        /// </summary>
        /// <returns></returns>
        public StoryController(ILogger<StoryController> logger)
        {
            _logger = logger;
        }

        private async Task<IEnumerable<int>> GetBest20StoriesId()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(getBestStoriesId);
            if (response.IsSuccessStatusCode)
            {
                string serializedResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<int>>(serializedResponse).Take(20);
            }
            else
            {
                throw new Exception("Could not connect with Hacker News API to get the best stories.");
            }
        }

        private async Task<Story> GetStoryById(int id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(String.Format(getStoryById, id));
            if (response.IsSuccessStatusCode)
            {
                string serializedResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Story>(serializedResponse);
            }
            else
            {
                throw new Exception("Could not connect with Hacker News API to get the best stories.");
            }
        }

        [HttpGet("best20")]
        public async Task<IEnumerable<Story>> GetAsync()
        {
            IEnumerable<int> bestStoriesIds = new List<int>();
            List<Story> stories = new List<Story>();
            List<Task<Story>> taskStories = new List<Task<Story>>();
            Task<IEnumerable<int>> getBest20StoriesIds = Task.Run(() =>
            {
                return GetBest20StoriesId();
            });
            bestStoriesIds = await getBest20StoriesIds;
            foreach(var id in bestStoriesIds)
            {
                taskStories.Add(Task.Run(() =>
                {
                    return GetStoryById(id);
                }));
            }
            await Task.WhenAll(taskStories);
            stories.AddRange(taskStories.Select(x => x.Result));
            return stories.OrderByDescending(x => x.Score);
        }
    }
}
