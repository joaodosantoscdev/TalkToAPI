using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        // Dependencies Injected | Constructor
        #region DI Injected
        private readonly IMessageRepository _messageRepository;
        public MessageController(IMessageRepository messageRepository)
        {
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
        [HttpGet("{userOneId}/{userTwoId}")]
        public IActionResult Get(string userOneId, string userTwoId) 
        {
            if (userOneId == userTwoId) 
            {
                return UnprocessableEntity();
            }
            return Ok(_messageRepository.GetMessages(userOneId, userTwoId));
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
        #endregion

        //UPDATE PARTIAL - MessageController Method
        #region UpdatePartial Method - Controller
        /// <summary>
        /// Atualiza Parcialmente Mensagens pelo Id na base de dados.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Erro no client-side (formatação, rota, requisição invalida)</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>A mensagem com propriedades atualizadas</returns>
        [Authorize]
        [HttpPatch("{id}")]
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

            return Ok(message);
        }
        #endregion

    }
}
