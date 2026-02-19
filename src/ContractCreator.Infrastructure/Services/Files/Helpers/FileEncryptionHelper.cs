using System.Security.Cryptography;
using System.Text;

namespace ContractCreator.Infrastructure.Services.Files.Helpers
{
    public static class FileEncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"); // Код шифрования только для теста
        private const byte FileFormatVersion = 2; // Проверка на случай повреждения или смены версии файла

        public static async Task EncryptStreamAsync(Stream inputStream, Stream outputStream)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV(); // Генерируем уникальный вектор для каждого файла

            outputStream.WriteByte(FileFormatVersion);
            await outputStream.WriteAsync(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);

            await inputStream.CopyToAsync(cryptoStream);

            await cryptoStream.FlushFinalBlockAsync();
        }

        public static async Task<byte[]> DecryptFileToBytesAsync(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

            int version = fileStream.ReadByte();
            if (version != FileFormatVersion)
                throw new CryptographicException("Неверная версия формата шифрования файла.");

            using var aes = Aes.Create();
            aes.Key = Key;

            byte[] iv = new byte[aes.BlockSize / 8];
            int read = await fileStream.ReadAsync(iv, 0, iv.Length);
            if (read != iv.Length)
                throw new CryptographicException("Файл поврежден (не удалось прочитать IV).");

            aes.IV = iv;

            using var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var memoryStream = new MemoryStream();

            await cryptoStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
