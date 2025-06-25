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
    private const string _hexFormat = "x2";

    public string ComputeHash(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo), "Нет данных");

        using (var md5 = MD5.Create())
        {
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(inputInfo));
            var sb = new StringBuilder();

            foreach (var b in hashBytes)
                sb.Append(b.ToString(_hexFormat));

            return sb.ToString();
        }
    }
}

public class SHA1Hasher : IHashSystem
{
    private const string _hexFormat = "x2";
    public string ComputeHash(string inputInfo)
    {
        if (inputInfo == null)
            throw new ArgumentNullException(nameof(inputInfo), "Нет данных");

        using (var sha1 = SHA1.Create())
        {
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputInfo));
            var sb = new StringBuilder();

            foreach (var b in hashBytes)
                sb.Append(b.ToString(_hexFormat));

            return sb.ToString();
        }
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

        string hash = _hashSystem.ComputeHash(order.Id.ToString());
        return $"pay.system1.ru/order?amount={order.Amount}RUB&hash={hash}";
    }
}

public class PaymentSystem2 : IPaymentSystem
{

    private readonly IHashSystem _hashSystem;

    public PaymentSystem2(IHashSystem hashSystem)
    {
        if (hashSystem == null)
            throw new ArgumentNullException(nameof(hashSystem), "Нет системы хэширования");

        _hashSystem = hashSystem;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string hash = _hashSystem.ComputeHash($"{order.Id}{order.Amount}");
        return $"order.system2.ru/pay?hash={hash}";
    }
}

public class PaymentSystem3 : IPaymentSystem
{
    private readonly string  _secretKey;
    private readonly IHashSystem _hashSystem;

    public PaymentSystem3(string key, IHashSystem hashSystem)
    {

        if (key == null)
            throw new ArgumentNullException(nameof(key), "Нет ключа");

        if (hashSystem == null)
            throw new ArgumentNullException(nameof(hashSystem), "Нет системы хэширования");

        _secretKey = key;
        _hashSystem = hashSystem;
    }

    public string GetPayingLink(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Нет заказа");

        string hash = _hashSystem.ComputeHash($"{order.Amount}{order.Id}{_secretKey}");
        return $"system3.com/pay?amount={order.Amount}&curency=RUB&hash={hash}";
    }

}

class Program
{
    static void Main(string[] args)
    {
        var md5 = new MD5Hasher();
        var sha1 = new SHA1Hasher();

        var ps1 = new PaymentSystem1(md5);
        var ps2 = new PaymentSystem2( md5);
        var ps3 = new PaymentSystem3("123", sha1);
    }
}