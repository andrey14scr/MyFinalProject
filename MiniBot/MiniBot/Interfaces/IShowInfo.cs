namespace MiniBot.Interfaces
{
    interface IShowInfo
    {
        void ShowShortInfo(string space = "");

        void ShowInfo(string space = "", bool isSpacedName = false);
    }
}
