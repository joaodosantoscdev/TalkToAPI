using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Repositories
{
    public class UserRepository : IUserRepository
    {
        // Dependencies Injected | Constructor
        #region DI - Injected
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        #endregion

        // Get IdentityUser on DB
        #region GET User - Database
        public ApplicationUser Get(string email, string password)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(user, password).Result)
            {
                return user;
            }
            else 
            {
                // USE >> Domain notification
                throw new Exception("Usuário não encontrado");
            }

        }
        #endregion

        //Get IdentityUser by Id on DB
        #region GETBYID User - Database
        public ApplicationUser GetById(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
            
        }
        #endregion

        //Add IdentityUser on DB
        #region ADD User - Database
        public void Add(ApplicationUser user, string password)
        {
            var result = _userManager.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                StringBuilder sb = new();
                foreach (var error in result.Errors) 
                {
                    sb.Append(error.Description);
                }
                // USE >> Domain notification
                throw new Exception($"Usuário nçaio cadastrado {sb}");
            }

        }
        #endregion

    }
}
