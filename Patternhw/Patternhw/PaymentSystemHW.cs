using System;
using System.Security.Cryptography;
using System.Text;

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public interface IHashSystem
{
    string ComputeHash(string inputInfo);
}

public class MD5Hasher : IHashSystem
{
    public string ComputeHash(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo), "Нет данных");

        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(inputInfo));
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}

public class SHA1Hasher : IHashSystem
{
    public string ComputeHash(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo), "Нет данных");

        using var sha1 = SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputInfo));
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
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
    private readonly IHashSystem _hashSystem;

    public PaymentSystem1(IHashSystem hashSystem)
    {
        if (hashSystem == null)
            throw new ArgumentNullException(nameof(hashSystem), "Нет системы хэширования");

        _hashSystem = hashSystem;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string info = $"{order.Id}{order.Amount}";
        string hash = _hashSystem.ComputeHash(info);
        return $"link?orderId={order.Id}&amount={order.Amount}&hash={hash}";
    }
}

public class PaymentSystem2 : IPaymentSystem
{
    private readonly int _goodPrice;
    private readonly IHashSystem _hashSystem;

    public PaymentSystem2(int price,IHashSystem hashSystem)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price),"Цена не может быть отрицательной");

        if (hashSystem == null)
            throw new ArgumentNullException(nameof(hashSystem), "Нет системы хэширования");

        _goodPrice = price;
        _hashSystem = hashSystem;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string info = $"{order.Id}{order.Amount}";
        string hash = _hashSystem.ComputeHash(info);
        int price = order.Amount * _goodPrice;
        return $"link?orderId={order.Id}&amount={order.Amount}&price={price}&hash={hash}";
    }
}

public class PaymentSystem3 : IPaymentSystem
{
    private readonly string  _secretKey;
    private readonly int _goodPrice;
    private readonly IHashSystem _hashSystem;

    public PaymentSystem3(int price,string key, IHashSystem hashSystem)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной");

        if (key == null)
            throw new ArgumentNullException(nameof(key), "Нет ключа");

        if (hashSystem == null)
            throw new ArgumentNullException(nameof(hashSystem), "Нет системы хэширования");

        _goodPrice = price;
        _secretKey = key;
        _hashSystem = hashSystem;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string info = $"{order.Amount}{order.Id}{_secretKey}";
        string sha1 = _hashSystem.ComputeHash(info);
        int price = order.Amount * _goodPrice;
        return $"link?orderId={order.Id}&amount={order.Amount}&price={price}&key={_secretKey}&hash={sha1}";
    }

}

class Program
{
    static void Main(string[] args)
    {
        var md5 = new MD5Hasher();
        var sha1 = new SHA1Hasher();

        var ps1 = new PaymentSystem1(md5);
        var ps2 = new PaymentSystem2(10, md5);
        var ps3 = new PaymentSystem3(10, "123", sha1);
    }
}