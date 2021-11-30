using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        void Add(Token token);

        Token Get(string refreshToken);

        void Update(Token token);
    }
}
