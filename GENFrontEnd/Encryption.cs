using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GENFrontEnd
{
    public class Encryption
    {
        const string passPhrase = "Pas5pr@se";        // can be any string
        const string saltValue = "s@1tValue";        // can be any string
        const string hashAlgorithm = "SHA1";            // can be "MD5"
        const int passwordIterations = 2;                  // can be any number
        const string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        const int keySize = 256;                // can be 192 or 128

        public string getMd5Hash(string input)
        {
            if (input != "")
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                MD5 md5Hasher = MD5.Create();

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
            else
            {
                return "";
            }
        }

        public string Encrypt(string rawPassword)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

            byte[] rawPasswordBytes = Encoding.UTF8.GetBytes(rawPassword);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);


            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(rawPasswordBytes, 0, rawPasswordBytes.Length);
            cryptoStream.FlushFinalBlock(); // Finish encrypting.

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }

        public string Decrypt(string storedPassword, string secretKey)
        {
            string rawPassword = "";

            if (secretKey == "istananegara")
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

                // Convert our ciphertext into a byte array.
                byte[] storedPasswordBytes = Convert.FromBase64String(storedPassword);


                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream(storedPasswordBytes);

                // Define cryptographic stream (always use Read mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] rawPasswordBytes = new byte[storedPasswordBytes.Length];

                int decryptedByteCount = cryptoStream.Read(rawPasswordBytes, 0, rawPasswordBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                rawPassword = Encoding.UTF8.GetString(rawPasswordBytes, 0, decryptedByteCount);
            }

            return rawPassword;
        }
    }
}