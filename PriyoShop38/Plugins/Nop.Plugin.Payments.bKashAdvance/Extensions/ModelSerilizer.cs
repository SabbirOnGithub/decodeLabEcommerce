using System.IO;
using System.Runtime.Serialization.Json;

namespace Nop.Plugin.Payments.bKashAdvance.Extensions
{
    public static class ModelSerilizer
    {
        public static byte[] CreateData<T>(this T value)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return stream.ToArray();
            }
        }
    }
}
