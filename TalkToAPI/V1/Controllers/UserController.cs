using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class UserController : ControllerBase
    {
        // Dependencies Injected | Constructor
        #region DI Injected
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserRepository userRepository,
                              SignInManager<ApplicationUser> signIn,
                              UserManager<ApplicationUser> userManager,
                              ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _signIn = signIn;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }
        #endregion

        // Login UserController Method (using JWT)
        #region Login Method - Controller 
        /// <summary>
        /// Efetua o Login do usuário e libera o token de acesso.
        /// </summary>
        /// <param name="userDTO">Usuário</param>
        /// /// <response code="200">Sucesso</response>
        /// <response code="422">Entidade não processada.</response>
        /// <response code="404">Não foi encontrado usuário especificado.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>Token com data de expiração e emissão</returns>
        [HttpPost("login")]
        public ActionResult Login(UserDTO userDTO)
        {
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Name");

            if (ModelState.IsValid)
            {
                ApplicationUser user = _userRepository.Get(userDTO.Email, userDTO.Password);
                if (user != null)
                {
                    //Identity login
                    /*_signIn.SignInAsync(user, false);*/

                    // Return JWT Token 
                    return GenerateToken(user);
                }
                else
                {
                    return NotFound("Usuário não localizado");
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        #endregion

        // Renew Token UserController Method
        #region Renew Token Method - Controller 
        /// <summary>
        /// Refresh/Renova o Token de acesso.
        /// </summary>
        /// <param name="tokenDTO">Token</param>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não foi encontrado usuário especificado.</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>Renova e entrega Token com data de expiração e emissão</returns>
        [HttpPost("renovar")]
        public IActionResult Renew(TokenDTO tokenDTO)
        {
            var refreshTokenDB = _tokenRepository.Get(tokenDTO.RefreshToken);

            if (refreshTokenDB == null)
            {
                return NotFound();
            }

            //  Gets old RefreshToken, update and disable
            refreshTokenDB.Att = DateTime.Now;
            refreshTokenDB.Used = true;
            _tokenRepository.Update(refreshTokenDB);

            //Generate a new token and save
            var user = _userRepository.GetById(refreshTokenDB.UserId);

            return GenerateToken(user);
        }
        #endregion

        // Register UserController Method
        #region Register Method - Controller 
        /// <summary>
        /// Cadastra um novo usuário na base de dados.
        /// </summary>
        /// <param name="userDTO">Usuário</param>
        /// <response code="200">Sucesso</response>
        /// <response code="422">Entidade não processada.</response>
        /// <returns>Usuário registrado na base de dados</returns>
        [HttpPost("")]
        public ActionResult Register(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = userDTO.Email,
                    FullName = userDTO.Name,
                    Email = userDTO.Email
                };
                var result = _userManager.CreateAsync(user, userDTO.Password).Result;

                if (!result.Succeeded)
                {
                    List<string> errorsList = new();
                    foreach (var error in result.Errors)
                    {
                        errorsList.Add(error.Description);
                    }
                    return UnprocessableEntity(errorsList);
                }
                else
                {
                    return Ok(user);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        #endregion

        //Update UserController Method
        #region Update Method - Controller 
        /// <summary>
        /// Atualiza um novo usuário na base de dados.
        /// </summary>
        /// <param name="userDTO">Usuário</param>
        /// <param name="id">Usuário</param>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="422">Entidade não processada.</response>
        /// <returns>Usuário registrado e atualizado na base de dados</returns>
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult Update(string id, UserDTO userDTO)
        {
            ApplicationUser user = _userManager.GetUserAsync(HttpContext.User).Result;
            // implement a validation filter (TO DO)
            if (user.Id != id) 
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {

                // need update for automapper (TO DO)
                user.UserName = userDTO.Email;
                user.FullName = userDTO.Name;
                user.Email = userDTO.Email;
                user.Slogan = userDTO.Slogan;

                // remove identity password requirements (TO DO)
                var result = _userManager.UpdateAsync(user).Result;
                _userManager.RemovePasswordAsync(user);
                _userManager.AddPasswordAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    List<string> errorsList = new();
                    foreach (var error in result.Errors)
                    {
                        errorsList.Add(error.Description);
                    }
                    return UnprocessableEntity(errorsList);
                }
                else
                {
                    return Ok(user);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        #endregion

        // Token Generation
        #region Private Methods (Token Generation)
        private TokenDTO BuildToken(ApplicationUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim (JwtRegisteredClaimNames.Sub, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-api-jwt-to-do-application"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: exp,
                signingCredentials: sign
                );

            var tokenJwt = new JwtSecurityTokenHandler().WriteToken(token).ToString();

            var refreshToken = Guid.NewGuid().ToString();
            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO { Token = tokenJwt, Expiration = exp, ExpirationRefreshToken = expRefreshToken, RefreshToken = refreshToken };

            return tokenDTO;
        }

        private ActionResult GenerateToken(ApplicationUser user)
        {
            var token = BuildToken(user);
            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationToken = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                User = user,
                Created = DateTime.Now,
                Used = false
            };
            _tokenRepository.Add(tokenModel);
            return Ok(token);
        }
        #endregion
    }
}

