using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class AESManager
    {
        /// <summary>
		/// Function that encrypts the plaintext from inFile and stores cipher text to outFile
		/// </summary>
        /// <param name="message"> message that we want to encrypt</param>
		/// <param name="secretKey"> symmetric encryption key </param>
		public static byte[] Encrypt(string message, string secretKey)
        {
            CipherMode mode = CipherMode.ECB;
            byte[] encryptedBody;
            byte[] messageArray = Encoding.ASCII.GetBytes(message);

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();
            var newMsg = aesEncryptTransform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
            return newMsg;
        }


        /// <summary>
        /// Function that decrypts the cipher text from inFile and stores as plaintext to outFile
        /// </summary>
        /// <param name="secretKey"> symmetric encryption key </param>
        public static string Decrypt(byte[] enctyptedMsg, string secretKey)
        {
            CipherMode mode = CipherMode.ECB;
            byte[] decryptedBody = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
            using (MemoryStream memoryStream = new MemoryStream(enctyptedMsg))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedBody = new byte[enctyptedMsg.Length];     //decrypted image body - the same lenght as encrypted part
                    cryptoStream.Read(decryptedBody, 0, decryptedBody.Length);
                }
            }

            return ASCIIEncoding.ASCII.GetString(decryptedBody);
        }
    }
}
