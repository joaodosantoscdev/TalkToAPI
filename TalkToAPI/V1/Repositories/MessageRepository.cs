using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.Database;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly TalkToContext _context;
        public MessageRepository(TalkToContext context)
        {
            _context = context;
        }

        public Message GetOne(int id)
        {
            return _context.Message.Find(id);
        }

        List<Message> IMessageRepository.GetMessages(string userOneId, string userTwoId)
        {
           return _context.Message.Where(m => (m.OwnerId == userOneId || m.OwnerId == userTwoId) 
           && (m.ReceiverId == userOneId || m.ReceiverId == userTwoId)).ToList();
        }

        void IMessageRepository.Register(Message message)
        {
            _context.Message.Add(message);
            _context.SaveChanges();
        }

        public void Update(Message message)
        {
            _context.Message.Update(message);
            _context.SaveChanges();
        }
    }
}
