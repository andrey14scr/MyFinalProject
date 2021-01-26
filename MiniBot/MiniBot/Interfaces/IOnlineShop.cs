using MiniBot.Products;

namespace MiniBot.Interfaces
{
    interface IOnlineShop
    {
        void Buy(Product product, byte amount = 1);
        void Estimate(byte score);
        void ComplectOrder();
        void Deliver();
    }
}
