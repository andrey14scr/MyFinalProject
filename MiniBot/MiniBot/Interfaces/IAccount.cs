using MiniBot.Activity;

namespace MiniBot.Interfaces
{
    class IAccount
    {
        [Email]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
