using System.Linq;
using TalkToAPI.Database;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        // Dependencies Injected | Constructor
        #region DI - Injected
        private readonly TalkToContext _context;
        public TokenRepository(TalkToContext context)
        {
            _context = context;
        }
        #endregion

        // Get Token on DB
        #region GET Token - Database
        public Token Get(string refreshToken)
        {
            return _context.Tokens.FirstOrDefault(t => t.RefreshToken == refreshToken && t.Used == false);
        }
        #endregion

        // Add Token on DB
        #region ADD Token - Database
        public void Add(Token token)
        {
            _context.Tokens.Add(token);
            _context.SaveChanges();
        }
        #endregion

        // Update Token on DB
        #region UPDATE Token - Database
        public void Update(Token token)
        {
            _context.Tokens.Update(token);
            _context.SaveChanges();
        }
        #endregion

    }
}
