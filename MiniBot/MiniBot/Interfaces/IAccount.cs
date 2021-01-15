using MiniBot.Activity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    class IAccount
    {
        [Email]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
