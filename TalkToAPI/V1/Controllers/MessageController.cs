using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [Authorize]
        [HttpGet("{userOneId}/{userTwoId}")]
        public IActionResult Get(string userOneId, string userTwoId) 
        {
            if (userOneId == userTwoId) 
            {
                return UnprocessableEntity();
            }
            return Ok(_messageRepository.GetMessages(userOneId, userTwoId));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Register(Message message)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _messageRepository.Register(message);
                    return Ok(message);
                }
                catch (Exception e) 
                {
                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
    }
}
