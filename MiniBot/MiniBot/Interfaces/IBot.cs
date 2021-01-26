using static MiniBot.Activity.Sources;

namespace MiniBot.Interfaces
{
    interface IBot
    {
        void SendMessage(string msg, BotState nextState);
        void GetAnswer();
        void DoAction(string command);
        void Start();
    }
}
