using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace chatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class chatController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public chatController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // POST api/<chatController>
        [HttpPost]
        public IActionResult Post([FromBody] List<JsonElement> user_prompt)
        {
            //process to ChatRequestMessage
            List<ChatRequestMessage> messages = new List<ChatRequestMessage>();
            for (int i = 0; i < user_prompt.Count; ++i) 
            {
                if (user_prompt[i].TryGetProperty("role", out var role) && user_prompt[i].TryGetProperty("content", out var content))
                {
                    // Process role and content
                    if (role.ToString() == "system")
                    {
                        messages.Add(new ChatRequestSystemMessage(content.ToString()));
                    }
                    if (role.ToString() == "user")
                    {
                        messages.Add(new ChatRequestUserMessage(content.ToString()));
                    }
                    if (role.ToString() == "assistant")
                    {
                        messages.Add(new ChatRequestAssistantMessage(content.ToString()));
                    }
                }
            }


            string endpoint = Configuration["endpoint"];
            string key = Configuration["key"];

            OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

            
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-35-turbo-16k", //This must match the custom deployment name you chose for your model
                Messages = { new ChatRequestSystemMessage("You are a helpful assistant.") },
                MaxTokens = 100,
            };


            foreach (ChatRequestMessage message in messages)
            {
                chatCompletionsOptions.Messages.Add(message);
            }

            Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

            return Ok(response.Value.Choices[0].Message.Content);
        }
    }
}
