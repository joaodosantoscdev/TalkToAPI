using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Interfaces
{
    public class IMessageRepository
    {
        List<Message> GetMessages(string owner, string receiver);

        void Register(Message message);
    }
}
