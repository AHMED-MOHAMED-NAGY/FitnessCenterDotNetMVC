using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace fitnessCenter.Services
{
    public class AIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<(string PlanText, string ImageUrl)> GenerateFitnessPlan(User user, string planType, int duration, IFormFile imageFile)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return ("Error: API Key is missing. Please configure Gemini:ApiKey in appsettings.json.", null);
            }

            // 1. Prepare Image
            string base64Image = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    base64Image = Convert.ToBase64String(ms.ToArray());
                }
            }

            // 2. Prepare Prompt
            var prompt = $@"
                Act as an expert fitness coach and nutritionist.
                Create a detailed {duration}-month {planType} plan for a client with the following stats:
                - Height: {user.boy} cm
                - Weight: {user.wight} kg
                - Age: {user.age} years
                
                The plan should include:
                1. Workout Routine (Weekly split)
                2. Nutrition Guide (Macros, sample meals)
                3. Progress milestones for a {duration}-month period.
                
                Format the response in clean Markdown.
            ";

            // 3. Call Gemini API (Text Generation)
            // Note: This matches the structure for Gemini 1.5 Flash/Pro via REST
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            base64Image != null ? new { inline_data = new { mime_type = imageFile.ContentType, data = base64Image } } : null
                        }.Where(p => p != null).ToArray()
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}", jsonContent);

            string planText = "Failed to generate plan.";
            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resultJson);
                // Extract text from response structure
                try
                {
                    planText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }
                catch
                {
                    planText = "Error parsing AI response.";
                }
            }
            else
            {
                planText = $"Error: {response.ReasonPhrase} - {await response.Content.ReadAsStringAsync()}";
            }

            // 4. Image Generation using Imagen
            string afterImageUrl = "https://placehold.co/600x400?text=Image+Generation+Failed"; // Fallback

            try
            {
                var imagePrompt = $"A realistic full-body fitness transformation photo of a male, {planType} physique, fit and muscular, {user.boy}cm height. High quality, photorealistic, professional lighting, 4k.";
                
                // Imagen API structure
                var imageRequestBody = new
                {
                    instances = new[]
                    {
                        new { prompt = imagePrompt }
                    },
                    parameters = new
                    {
                        sampleCount = 1,
                        aspectRatio = "1:1"
                    }
                };

                var imageJsonContent = new StringContent(JsonSerializer.Serialize(imageRequestBody), Encoding.UTF8, "application/json");
                // Using the available model from the list: imagen-4.0-fast-generate-001
                var imageResponse = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/imagen-4.0-fast-generate-001:predict?key={apiKey}", imageJsonContent);

                if (imageResponse.IsSuccessStatusCode)
                {
                    var imageResultJson = await imageResponse.Content.ReadAsStringAsync();
                    using var imageDoc = JsonDocument.Parse(imageResultJson);
                    
                    // Parse response: { "predictions": [ { "bytesBase64Encoded": "..." } ] }
                    if (imageDoc.RootElement.TryGetProperty("predictions", out var predictions) && predictions.GetArrayLength() > 0)
                    {
                        var base64 = predictions[0].GetProperty("bytesBase64Encoded").GetString();
                        afterImageUrl = $"data:image/png;base64,{base64}";
                    }
                }
                else
                {
                    // If 4.0 fails, try 3.0 as fallback or just log (visible in UI via fallback image)
                     Console.WriteLine($"Image Gen Failed: {imageResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image Gen Error: {ex.Message}");
            }

            return (planText, afterImageUrl);
        }
    }
}
