namespace MiniBot.Interfaces
{
    interface IGetInfo
    {
        string GetInfo(string space = "");

        string GetShortInfo(string space = "");
    }
}
