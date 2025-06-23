using System.Diagnostics;

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id,int amount)
    {

        if (id < 0)
            throw new ArgumentOutOfRangeException(nameof(id),"Айди не может быть меньше 0");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "количество не может быть меньше 0");

        Id = id;
        Amount = amount;
    }
}

public class PaymentSystem1 : IPaymentSystem
{
    public string GetPayingLink(Order order)
    {

        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

         string info = $"{order.Id}айди заказа и {order.Amount} количество товара";
         string hash = CreateMD5(info);
         return  hash + info;
    }

    private string CreateMD5(string inputID)
    {
        return inputID = new Random().Next().ToString("Хэщ");
    }
}

public class PaymentSystem2 : IPaymentSystem
{
    private readonly int _goodPrice;

    public PaymentSystem2(int price)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price),"Цена не может быть отрицательной");

        _goodPrice = price;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string info = $"{order.Id}айди заказа и {order.Amount} количество товара";
        string hash = CreateMD5(info);
        string price = OrderPrice(order, _goodPrice);
        return price + hash + info;
    }

    private string CreateMD5(string inputID)
    {
        return inputID = new Random().Next().ToString();
    }

    private string OrderPrice(Order order,int price)
    {
        int amount = order.Amount;
        amount *= price;
        return $"Цена товара: {price}";
    }
}

public class PaymentSystem3 : IPaymentSystem
{
    private readonly string  _secretKey;
    private readonly int _goodPrice;

    public PaymentSystem3(int price,string key)
    {

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной");

        if (key == null)
            throw new ArgumentNullException(nameof(key), "Нет ключа");

        _goodPrice = price;

        _secretKey = key;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string info = $"{order.Id}айди заказа и {order.Amount} количество товара";
        string hash = CreateMD5(info);
        string price = OrderPrice(order, _goodPrice);
        return _secretKey +price + hash + info;
    }

    private string CreateMD5(string inputID)
    {
        return inputID = new Random().Next().ToString();
    }

    private string OrderPrice(Order order, int price)
    {
        int amount = order.Amount;
        amount *= price;
        return  $"Цена товара: {price}";

    }
}


class Program
{
    static void Main(string[] args)
    {
        var ps1 = new PaymentSystem1();
        var ps2 = new PaymentSystem2(10);
        var ps3 = new PaymentSystem3(10, "123");
    }
}