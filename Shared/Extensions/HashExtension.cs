using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SmartMonitoring.Shared.Extensions;

public static class HashExtension
{
    /// <summary>
    /// Конвертация строчки в хэш алгоритма SHA256.
    /// </summary>
    /// <param name="input">Строчка.</param>
    /// <returns>Хэш алгоритма SHA256.</returns>
    public static string ToHashSHA256(this string input)
    {
        StringBuilder Sb = new StringBuilder();

        using (var hash = SHA256.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(input));

            foreach (byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }
    
    /// <summary>
    /// Конвертация листа обьектов в хэш алгоритма SHA256.
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static string ToHashSHA256(this List<object> datas)
    {
        return ToHashSHA256(JsonConvert.SerializeObject(datas,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }
    
    /// <summary>
    /// Конвертация обьекта в хэш алгоритма SHA256.
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static string ToHashSHA256(this object data)
    {
        return ToHashSHA256(JsonConvert.SerializeObject(data,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }

    
    /// <summary>
    /// Конвертация строчки в хэш алгоритма SHA1.
    /// </summary>
    /// <param name="input">Строчка.</param>
    /// <returns>Хэш алгоритма SHA1.</returns>
    public static string ToHashSHA1(this string input)
    {
        StringBuilder Sb = new StringBuilder();

        using (var hash = SHA1.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(input));

            foreach (byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }   
}