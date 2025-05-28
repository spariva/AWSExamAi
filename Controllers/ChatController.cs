using AWSExamAi.Models;
using AWSExamAi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWSExamAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController: ControllerBase
    {
        private readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [Route("Chat")]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new ChatResponse
                {
                    Success = false,
                    ErrorMessage = "Prompt is required"
                });
            }

            var response = await _chatService.GetChatResponseAsync(request.Prompt);

            return response.Success ? Ok(response) : StatusCode(500, response);
        }
    }
}
