using System.Security.Cryptography;

namespace nfs2iso2nfs.Helpers
{
    public class KeyHelper
    {
        public static byte[]? GetKey(string keyDir)
        {
            using var keyFile = new BinaryReader(File.OpenRead(keyDir));
            var keySize = keyFile.BaseStream.Length;
            if (keySize != 16)
                return null;

            return keyFile.ReadBytes(0x10);
        }
        public static Aes CreateAes128Cbc(byte[] key, byte[] iv)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = key;
            aes.IV = iv;

            return aes;
        }
        public static byte[] CryptAes128Cbc(Aes aes, byte[] data, bool encrypt = false)
        {
            byte[] result = new byte[data.Length];

            using (ICryptoTransform itc = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor())
                result = itc.TransformFinalBlock(data, 0, data.Length);

            //TODO: is this necessary?
            aes.Clear();

            return result;
        }
    }
}
