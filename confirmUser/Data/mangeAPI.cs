
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


namespace userInformation.Helper
{
    public class manageAPI
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;


        public manageAPI(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            
            _configuration = configuration;
        }
        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                string baseUrl = _configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    TResponse result = JsonConvert.DeserializeObject<TResponse>(json);
                    return result;
                }
                else
                {
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"API request failed: {ex.Message}");
            }
        }

        public async Task<bool> PostAsync<TModel>(TModel model, string url)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                string baseUrl = _configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");



                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {

                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Error: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"API request failed: {ex.Message}");
            }
        }



    }
}
