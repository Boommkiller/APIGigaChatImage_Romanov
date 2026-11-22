using System.Text;
using System.Text.RegularExpressions;
using APIGigaChatImage.Models;
using APIGigaChatImage.Models.Response;
using Newtonsoft.Json;

namespace APIGigaChatImage;

class Program
{
    static string ClientId = "***";
    static string AuthorizationKey = "***";

    static async Task Main(string[] args)
    {
        string Token = await GetToken(ClientId, AuthorizationKey);
        string ImageId = await GenerateImage(Token, "розового кота");
        Console.WriteLine(ImageId);
    }

    public static async Task<string> GetToken(string rqUID, string bearer)
    {
        string ReturnToken = null;
        string Url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

        using (HttpClientHandler Handler = new HttpClientHandler())
        {
            Handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            using (HttpClient Clien = new HttpClient(Handler))
            {
                HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);

                Request.Headers.Add("Accept", "application/json");
                Request.Headers.Add("RqUID", rqUID);
                Request.Headers.Add("Authorization", $"Basic {bearer}");

                var Data = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("scope", "GIGACHAT_API_PERS")
                };

                Request.Content = new FormUrlEncodedContent(Data);

                HttpResponseMessage Response = await Clien.SendAsync(Request);

                if (Response.IsSuccessStatusCode)
                {
                    string ResponseContent = await Response.Content.ReadAsStringAsync();

                    ResponseToken Token = JsonConvert.DeserializeObject<ResponseToken>(ResponseContent);

                    ReturnToken = Token.access_token;
                }
            }
        }

        return ReturnToken;
    }

    public static async Task<string> GenerateImage(string token, string prompt)
    {
        string ImageId = null;
        string Url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

        using (HttpClientHandler Handler = new HttpClientHandler())
        {
            Handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            using (HttpClient Client = new HttpClient(Handler))
            {
                HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);

                Request.Headers.Add("Accept", "application/json");
                Request.Headers.Add("Authorization", $"Bearer {token}");

                Request DataRequest = new Request()
                {
                    model = "GigaChat",
                    stream = false,
                    function_call = "auto",
                    messages = new List<Request.Message>() {
                        new Request.Message() {
                            role = "system",
                            content = "Ты — художник, который рисует красивые обои для рабочего стола в горизонтальном формате"
                        },
                        new Request.Message() {
                            role = "user",
                            content = "Нарисуй " + prompt
                        }
                    }
                };

                string JsonContent = JsonConvert.SerializeObject(DataRequest);

                Request.Content = new StringContent(JsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage Response = await Client.SendAsync(Request);

                if (Response.IsSuccessStatusCode)
                {
                    string ResponseContent = await Response.Content.ReadAsStringAsync();

                    ResponseMessage Answer = JsonConvert.DeserializeObject<ResponseMessage>(ResponseContent);

                    Match match = Regex.Match(Answer.choices[0].message.content, "<img src=\"(.*?)\"");
                    if (match.Success)
                        ImageId = match.Groups[1].Value;
                }
            }
        }

        return ImageId;
    }
}
