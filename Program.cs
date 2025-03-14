using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Program generate AES key file "aes_key",
/// and encrypt input in file  "aes_hash";
/// When files exists, decrypt "aes_hash".
/// </summary>
public static class AesEncryption
{
    public static byte[] GenerateAESKey()
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.GenerateKey();
            return aesAlg.Key;
        }
    }
    /// <summary>
    /// AES encryprt plainText with key keyBytes
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="keyBytes"></param>
    /// <returns>encrypted Base64String</returns>
    public static string AESEncrypt(string plainText, byte[] keyBytes)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.GenerateIV(); // Generate a new random IV

            byte[] ivBytes = aesAlg.IV;

            aesAlg.Padding = PaddingMode.PKCS7; // Explicitly set PKCS7 padding

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                    csEncrypt.FlushFinalBlock();

                    byte[] ciphertextBytes = msEncrypt.ToArray();

                    // ***Prepend the IV to the ciphertext***
                    byte[] combinedBytes = new byte[ivBytes.Length + ciphertextBytes.Length];
                    Array.Copy(ivBytes, combinedBytes, ivBytes.Length);
                    Array.Copy(ciphertextBytes, 0, combinedBytes, ivBytes.Length, ciphertextBytes.Length);

                    return Convert.ToBase64String(combinedBytes);
                }
            }
        }
    }
    /// <summary>
    /// AES Decryprt cipherTextBase64 with key keyBytes
    /// </summary>
    /// <param name="cipherTextBase64"></param>
    /// <param name="keyBytes"></param>
    /// <returns>decrypted string</returns>
    public static string AESDecrypt(string cipherTextBase64, byte[] keyBytes)
    {
        try
        {
            byte[] combinedBytes = Convert.FromBase64String(cipherTextBase64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;

                int ivLength = aesAlg.BlockSize / 8; // 16 bytes
                byte[] ivBytes = new byte[ivLength];
                Array.Copy(combinedBytes, ivBytes, ivLength);
                aesAlg.IV = ivBytes;

                byte[] ciphertextBytes = new byte[combinedBytes.Length - ivLength];
                Array.Copy(combinedBytes, ivLength, ciphertextBytes, 0, ciphertextBytes.Length);

                aesAlg.Padding = PaddingMode.PKCS7; // Explicitly set PKCS7 padding

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(ciphertextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string plaintext = srDecrypt.ReadToEnd();
                            return plaintext;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decryption Error: {ex.Message}"); // Handle exceptions appropriately
            return null; // Or throw the exception if you prefer
        }
    }
}


// Example Usage:
public class App
{
    public static void Main(string[] args)
    {
        string keyFile = "aes_key";
        string encryptedFile = "aes_hash";
        string plainText ="password readed from file ";
        string l = new String('-', 50);
        string keyBase64;

        Console.WriteLine($"\n{CC.RED}{l}{CC.NORMAL}" +
            $"\n   Application creates AES key in file " +
            $"\n   {CC.YELLOW}'aes_key'{CC.NORMAL} " +
            $"\n   and encrypts string in file " +
            $"\n   {CC.YELLOW}'aes_hash'{CC.NORMAL}." +
            $"\n   When files exists - shows decrypted string." +
            $"\n   \t b1b17@outlook.com, 2025" +
            $"\n{CC.RED}{l}{CC.NORMAL}");
        if ( File.Exists(encryptedFile))
        {
            if (File.Exists(keyFile) && File.Exists(encryptedFile))
            {
                Console.WriteLine($"{CC.BLUE }{l}{CC.NORMAL}");
                string stmp = AesEncryption.AESDecrypt(File.ReadAllText(encryptedFile), Convert.FromBase64String(File.ReadAllText(keyFile)));
                Console.WriteLine($"   password in hash file: {stmp}");
                Console.WriteLine($"{CC.BLUE}{l}{CC.NORMAL}");

            }
            
            Console.WriteLine($"   Files \n   {CC.YELLOW}'aes_key'{CC.NORMAL} or {CC.YELLOW}'aes_hash'{CC.NORMAL} are exists." +
                $"\n   Remove them first, then try again." +
                $"\n{CC.BLUE}{l}{CC.NORMAL}" +
                $"\n\t   {CC.BLUE}Bye.\n\t   Key to exit.{CC.NORMAL}" +
                $"\n{CC.BLUE}{l}{CC.NORMAL}");
            Console.ReadKey();
            Environment.Exit(0);
        }

        if (!File.Exists(keyFile))
        {
             byte[]  key = AesEncryption.GenerateAESKey(); // Generate the key ONCE and store it securely!
             keyBase64 = Convert.ToBase64String(key);
             File.WriteAllText(keyFile, keyBase64);
        }
        keyBase64 = File.ReadAllText(keyFile);


        if (!File.Exists(encryptedFile))
        {
            do
            {
                Console.WriteLine("   Enter string to encrypt: ");
                plainText = Console.ReadLine();
            }
            while (String.IsNullOrEmpty(plainText));
            //Console.WriteLine("Decrypted: " + plainText);

            string encrypted = AesEncryption.AESEncrypt(plainText, Convert.FromBase64String(keyBase64));

            File.WriteAllText(encryptedFile, encrypted);
        }

        //string decrypted = AesEncryption.AESDecrypt(File.ReadAllText(encryptedFile), Convert.FromBase64String(keyBase64));

        Console.WriteLine($"{CC.YELLOW}{l}{CC.NORMAL}");
        Console.WriteLine("   Key: " + keyBase64);
        Console.WriteLine("   Hash: " + File.ReadAllText(encryptedFile));
        //Console.WriteLine("Decrypted for check: " + decrypted);





        Console.WriteLine($"{CC.YELLOW}{l}{CC.NORMAL}");
        Console.WriteLine($"   Job done" +
            $"\n{CC.YELLOW}{l}{CC.NORMAL}" +
            $"\n   Key in file {CC.YELLOW}'aes_key'{CC.NORMAL}" +
            $"\n   Hash in file {CC.YELLOW}'aes_hash'{CC.NORMAL}" +
            $"\n{CC.YELLOW}{l}{CC.NORMAL}" +
            $"\n   Key to exit.");
        Console.ReadKey();
    }
}

public static class CC
{
    public static string NL = Environment.NewLine; // shortcut
    public static string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
    public static string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
    public static string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
    public static string YELLOW = Console.IsOutputRedirected ? "" : "\x1b[93m";
    public static string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
    public static string MAGENTA = Console.IsOutputRedirected ? "" : "\x1b[95m";
    public static string CYAN = Console.IsOutputRedirected ? "" : "\x1b[96m";
    public static string GREY = Console.IsOutputRedirected ? "" : "\x1b[97m";
    public static string BOLD = Console.IsOutputRedirected ? "" : "\x1b[1m";
    public static string NOBOLD = Console.IsOutputRedirected ? "" : "\x1b[22m";
    public static string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
    public static string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
    public static string REVERSE = Console.IsOutputRedirected ? "" : "\x1b[7m";
    public static string NOREVERSE = Console.IsOutputRedirected ? "" : "\x1b[27m";
}