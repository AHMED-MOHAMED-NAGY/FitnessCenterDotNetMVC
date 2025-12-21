using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

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

            // 1. Prepare Image (Resize for Stability AI)
            string base64Image = null;
            byte[] imageBytes = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    ms.Position = 0; // Reset for reading

                    // Resize to 1024x1024 for SDXL
                    try 
                    {
                        using (var image = SixLabors.ImageSharp.Image.Load(ms))
                        {
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(1024, 1024),
                                Mode = ResizeMode.Crop // Crop to fill 1024x1024
                            }));
                            
                            using (var outMs = new MemoryStream())
                            {
                                await image.SaveAsPngAsync(outMs);
                                imageBytes = outMs.ToArray();
                                base64Image = Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Image resize failed: " + ex.Message);
                        // Fallback to original if resize fails (though likely to fail API too)
                        imageBytes = ms.ToArray();
                        base64Image = Convert.ToBase64String(imageBytes);
                    }
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

            // 4. Image Generation using Stability AI (Image-to-Image)
            string afterImageUrl = "https://placehold.co/600x400?text=Image+Generation+Failed";

            try
            {
                var stabilityApiKey = _configuration["StabilityAI:ApiKey"];
                if (string.IsNullOrEmpty(stabilityApiKey))
                {
                    Console.WriteLine("Stability AI API Key missing.");
                }
                else if (imageBytes != null) // Require init image for img2img
                {
                    // Customized prompt based on plan & stats
                    string stylePrompt;
                    string negativePrompt = "bad anatomy, blurred, low quality, distortion, ugly face";
                    string strength = "0.60"; // Default strength

                    if (planType.ToLower().Contains("bulk")) 
                    {
                        stylePrompt = $"muscular bodybuilder, massive muscle gain, bigger arms, broad shoulders, pectoral hypertrophy. He has transformed from {user.wight}kg to a muscular build over {duration} months.";
                        negativePrompt += ", skinny, frail, small";
                        if (duration >= 6) strength = "0.55"; // Allow more change for long bulking
                    } 
                    else // Cutting
                    {
                        stylePrompt = $"lean athletic physique, significant weight loss, defined six pack abs, flat stomach, vascularity. He has lost weight from {user.wight}kg to a shredded physique over {duration} months.";
                        negativePrompt += ", obese, fat, belly, chubby, overweight";
                        if (duration >= 6) strength = "0.50"; // Allow significant change for long cutting
                    }

                    // Prompt emphasizing identity retention AND clothing
                    var imagePrompt = $"A realistic fitness transformation result of this man: {stylePrompt}. He is {user.age} years old, {user.boy}cm tall. Wearing athletic gym shorts, shirtless torso. Keep the exact same facial features, beard, hairstyle, and head structure. High quality, photorealistic, gym lighting, 8k.";

                    using var form = new MultipartFormDataContent();
                    form.Add(new StringContent(imagePrompt), "text_prompts[0][text]");
                    form.Add(new StringContent("1"), "text_prompts[0][weight]");
                    
                    form.Add(new StringContent(negativePrompt), "text_prompts[1][text]");
                    form.Add(new StringContent("-1"), "text_prompts[1][weight]");

                    form.Add(new StringContent(strength), "image_strength"); 
                    form.Add(new StringContent("35"), "steps");
                    form.Add(new ByteArrayContent(imageBytes), "init_image", "image.png");

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/image-to-image");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", stabilityApiKey);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = form;

                    var imageResponse = await _httpClient.SendAsync(request);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageResultJson = await imageResponse.Content.ReadAsStringAsync();
                        using var imageDoc = JsonDocument.Parse(imageResultJson);

                        if (imageDoc.RootElement.TryGetProperty("artifacts", out var artifacts) && artifacts.GetArrayLength() > 0)
                        {
                            var base64 = artifacts[0].GetProperty("base64").GetString();
                            afterImageUrl = $"data:image/png;base64,{base64}";
                        }
                    }
                    else
                    {
                        var errorContent = await imageResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Stability AI Failed: {imageResponse.StatusCode} - {errorContent}");
                        
                        // Log error to file for debugging
                        try {
                            await File.WriteAllTextAsync(@"C:\Users\moham\.gemini\antigravity\brain\e22cb01f-472d-41b0-a0bf-3b962bf552dc\last_ai_error.txt", $"Status: {imageResponse.StatusCode}\nError: {errorContent}");
                        } catch { /* ignore file write error */ }

                        afterImageUrl = $"https://placehold.co/600x400?text=Error+{(int)imageResponse.StatusCode}";
                    }
                }
                else 
                {
                    // Fallback or handle no image case (optional: could do text-to-image here if needed)
                    Console.WriteLine("No image uploaded for Image-to-Image generation.");
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
