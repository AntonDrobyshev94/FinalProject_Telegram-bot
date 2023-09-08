using Newtonsoft.Json;
using System.Text;

namespace TelegramApp
{
    public class ApplicationDataApi
    {
        private HttpClient httpClient { get; set; }
        public ApplicationDataApi()
        {
            httpClient = new HttpClient();
        }

        public IEnumerable<Service> GetServices()
        {
            string url = @"https://localhost:7037/api/values/GetServices";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<Service>>(json);
        }

        /// <summary>
        /// Запрос на создание новой заявки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="httpContext"></param>
        public void AddApplication(Application application)
        {
            string url = @"https://localhost:7037/api/values/";
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(application), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }
    }
}
