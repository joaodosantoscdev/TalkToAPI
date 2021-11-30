using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.Database
{
    public class TalkToContext : IdentityDbContext<ApplicationUser>
    {
        public TalkToContext(DbContextOptions<TalkToContext> options) :  base(options) 
        {
        }

        public DbSet<Message> Message { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
