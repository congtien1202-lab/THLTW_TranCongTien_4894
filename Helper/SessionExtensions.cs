using System.Text.Json;

namespace SellingWebsite.Extensions
{
    public static class SessionExtensions
    {
        // Chuyển đối tượng (List, Object,...) thành chuỗi JSON và lưu vào Session
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Lấy chuỗi JSON từ Session và chuyển ngược lại thành đối tượng (List, Object,...)
        public static T? GetJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}