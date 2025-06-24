using System.Security.Cryptography;
using System.Text;

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

        string info = $"{order.Id}{order.Amount}";
        string hash = CreateMD5(info);
        return hash + $"{order.Id}айди заказа и {order.Amount} количество товара";
    }

    private string CreateMD5(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo),"Нет данных");

        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputInfo);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            
            return "Хэщ заказа" + sb.ToString();
        }
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

        string info = $"{order.Id}{order.Amount}";
        string hash = CreateMD5(info);
        string price = OrderPrice(order, _goodPrice);
        return $"Цена заказа{price} + {order.Id}айди заказа и {order.Amount} количество товара + {hash}";
    }

    private string CreateMD5(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo),"Нет данных");

        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputInfo);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return "Хэщ заказа" + sb.ToString();
        }
    }

    private string OrderPrice(Order order,int price)
    {

        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной");

        int amount = order.Amount;
        amount *= price;
        return $"Цена товара: {amount}";
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

        string info = $"{order.Amount}{order.Id}{_secretKey}";
        string sha1 = CreateSHA1(info);
        string price = OrderPrice(order, _goodPrice);
        return $"ключ заказа{_secretKey} +Цена заказа{price} + {order.Id}айди заказа и {order.Amount} количество товара + {sha1}";
    }

    private string CreateSHA1(string inputInfo)
    {
        using (var sha1 = SHA1.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputInfo);
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return "Sha1 заказа" + sb.ToString();
        }
    }

    private string OrderPrice(Order order, int price)
    {

        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной");

        return $"Цена товара: {order.Amount * price}";
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