using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class MessageController : ControllerBase
    {
        // Dependencies Injected | Constructor
        #region DI Injected
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public MessageController(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }
        #endregion

        //GET ALL MessageController Method
        #region GET Method - Controller
        /// <summary>
        /// Busca e retorna todas os Mensagens de um chat na base de dados.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>Mensagens baseadas nos Ids dos usuários na query</returns>
        [Authorize]
        [HttpGet("{userOneId}/{userTwoId}", Name = "GetMessages")]
        public IActionResult Get(string userOneId, string userTwoId) 
        {
            if (userOneId == userTwoId) 
            {
                return UnprocessableEntity();
            }

            var messages = _messageRepository.GetMessages(userOneId, userTwoId);
            var listMsg = _mapper.Map<List<Message>, List<MessageDTO>>(messages);

            var list = new ListDTO<MessageDTO>() { List = listMsg };
            list.Links.Add(new LinkDTO("_self", "GET", Url.Link("GetMessages", new { userOneId = userOneId, userTwoId = userTwoId })));

            return Ok(list);
        }
        #endregion

        //REGISTER MessageController Method
        #region Register Method - Controller
        /// <summary>
        /// Registra Mensagens do chat na base de dados.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>A mensagem registrada</returns>
        [Authorize]
        [HttpPost("", Name = "RegisterMessage")]
        public IActionResult Register(Message message)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _messageRepository.Register(message);

                    var messageDb = _mapper.Map<Message, MessageDTO>(message);
                    messageDb.Links.Add(new LinkDTO("_self", "POST", Url.Link("RegisterMessage", null)));
                    messageDb.Links.Add(new LinkDTO("_attPartial", "PATCH", Url.Link("UpdateMessagePartial", new { id = message.Id })));

                    return Ok(messageDb);
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
        #endregion

        //UPDATE PARTIAL - MessageController Method
        #region UpdatePartial Method - Controller
        /// <summary>
        /// Atualiza parcialmente Mensagens pelo Id na base de dados.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Erro no client-side (formatação, rota, requisição invalida)</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>A mensagem com propriedades atualizadas</returns>
        [Authorize]
        [HttpPatch("{id}", Name = "UpdateMessagePartial")]
        public IActionResult UpdatePartial(int id, JsonPatchDocument<Message> jsonPatch) 
        {
            if (jsonPatch == null)
            {
                return BadRequest();
            }

            var message = _messageRepository.GetOne(id);

            jsonPatch.ApplyTo(message);
            message.Att = DateTime.UtcNow;

            _messageRepository.Update(message);

            var messageDb = _mapper.Map<Message, MessageDTO>(message);
            messageDb.Links.Add(new LinkDTO("_self", "POST", Url.Link("UpdateMessagePartial", new { id = message.Id })));

            return Ok(messageDb);
        }
        #endregion

    }
}
