using System.Text;

namespace ConsoleLauncher.Extensions
{
    public static class HashExtensions
    {
        public static byte[] Xor(string payload, string key)
        {
            var data = Encoding.UTF8.GetBytes(payload);
            byte[] result = new byte[data.Length];
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            for (int x = 0, y = 0; x < data.Length; x++, y++) {
                if (y == keyByte.Length) {
                    y = 0;
                }
                result[x] = (byte) (data[x] ^ keyByte[y]);
            }
            return result;
        }
    }
}