using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OpenAI_API;
using OpenAI_API.Models;
using OpenAI_API.Chat;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace chatbot_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIchatController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public OpenAIchatController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // POST api/<OpenAIchatController>
        [HttpPost]
        public async Task<string> Post([FromBody] List<JsonElement> message_json)
        {
            string key = Configuration["openai_key"];
            OpenAIAPI api = new OpenAIAPI(key);

            List<ChatMessage> messages = new List<ChatMessage>();
            for (int i = 0; i < message_json.Count; ++i)
            {
                if (message_json[i].TryGetProperty("role", out var role) && message_json[i].TryGetProperty("content", out var content))
                {
                    // Process role and content
                    if (role.ToString() == "system")
                    {
                        messages.Add(new ChatMessage(ChatMessageRole.System, content.ToString()));
                    }
                    if (role.ToString() == "user")
                    {
                        messages.Add(new ChatMessage(ChatMessageRole.User, content.ToString()));
                    }
                    if (role.ToString() == "assistant")
                    {
                        messages.Add(new ChatMessage(ChatMessageRole.Assistant, content.ToString()));
                    }
                }
            }

            var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo_1106,
                Temperature = 0.1,
                MaxTokens = 200,
                Messages = messages
            });
            var reply = result.Choices[0].Message;
            return reply.TextContent.Trim();
        }

       
    }
}
