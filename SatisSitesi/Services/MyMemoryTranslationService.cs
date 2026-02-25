using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SatisSitesi.Services.Interfaces;

namespace SatisSitesi.Services
{
    public class MyMemoryTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;

        // Optionally, MyMemory offers an email parameter for higher rate limits.
        // For a demonstration, we use anonymous access.
        private const string ApiBaseUrl = "https://api.mymemory.translated.net/get";

        public MyMemoryTranslationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (sourceLang.Equals(targetLang, StringComparison.OrdinalIgnoreCase)) return text;

            try
            {
                var url = $"{ApiBaseUrl}?q={Uri.EscapeDataString(text)}&langpair={sourceLang}|{targetLang}";

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var translationResponse = JsonSerializer.Deserialize<MyMemoryResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (translationResponse?.ResponseData?.TranslatedText != null)
                    {
                        return translationResponse.ResponseData.TranslatedText;
                    }
                }
            }
            catch (Exception)
            {
                // In a production environment, this should log the exception.
                // We fallback to the original text securely if API goes offline or drops rate limit.
            }
            
            return text;
        }

        private class MyMemoryResponse
        {
            public ResponseData ResponseData { get; set; }
        }

        private class ResponseData
        {
            public string TranslatedText { get; set; }
        }
    }
}
