using AWSExamAi.Helpers;
using AWSExamAi.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace AWSExamAi.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            string jsonSecrets = HelperSecretsManager.GetSecretAsync().GetAwaiter().GetResult();
            KeysModel keysModel = JsonConvert.DeserializeObject<KeysModel>(jsonSecrets);
            _apiKey = keysModel.key;
            _endpoint = keysModel.url;
        }

        public async Task<ChatResponse> GetChatResponseAsync(string prompt)
        {
            try
            {
                var payload = new
                {
                    messages = new object[]
                    {
                    new {
                        role = "system",
                        content = new object[] {
                            new {
                                type = "text",
                                text = "You are a helpful AI assistant."
                            }
                        }
                    },
                    new {
                        role = "user",
                        content = new object[] {
                            new {
                                type = "text",
                                text = prompt
                            }
                        }
                    }
                    },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 800,
                    stream = false
                };
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_endpoint, content);

                if(response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    var reply = jsonObject?.choices?[0]?.message?.content?.ToString();

                    return new ChatResponse
                    {
                        Reply = reply ?? "No response received",
                        Success = true
                    };
                }
                else
                {
                    return new ChatResponse
                    {
                        Success = false,
                        ErrorMessage = $"API Error: {response.StatusCode}"
                    };
                }
            }
            catch(Exception ex)
            {
                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
