using System.Text;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Public.Web.Extensions;

public static class SessionExtension
{
    public static string GetString(this ISession session, string key)
    {
        var data = session.Get(key);
        return data is null ? null : Encoding.UTF8.GetString(data);
    }

    public static void SetString(this ISession session, string key, string value)
    {
        session.Set(key, Encoding.UTF8.GetBytes(value));
    }
}
