using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        List<Message> GetMessages(string userOneId, string userTwoId);

        void Register(Message message);
    }
}
