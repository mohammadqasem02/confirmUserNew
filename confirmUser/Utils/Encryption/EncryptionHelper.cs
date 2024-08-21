using System.Security.Cryptography;
using System.Text;

namespace confirmUser.Utils.Encryption
{
    public class EncryptionHelper
    {
        #region core
        public static string Corekey = "GIGJoKSecure**561988901234567890";
        public static string Encrypt(string plainText)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(Corekey);
                    if (aes.Key.Length != 32)
                    {
                        //LogWriter.WriteLogError("Key must be exactly 32 bytes (256 bits) long for AES-256." + $"<p>Key is {plainText}</p>");
                    }
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }


                return Convert.ToBase64String(array);


            }
            catch (Exception ex)
            {

                // LogWriter.WriteLogError(ex.ToString());
                return null;
            }

        }

        public static string Decrypt(string cipherText)
        {
            try
            {


                byte[] iv = new byte[16]; // 16 bytes for AES IV
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(Corekey);
                    if (aes.Key.Length != 32)
                    {
                        // Email there is an issue with key + (key ) + Server Ip Or Project 
                    }
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                //LogWriter.WriteLogError(ex.ToString());
                return null;
            }

        }
        #endregion

    }
}

