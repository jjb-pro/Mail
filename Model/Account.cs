using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Mail.Model;

public class Account
{
    public string Name { get; set; } = "Unnamed Account";

    public string EmailAddress { get; set; } = string.Empty;

    public string EncryptedPassword { get; set; } = string.Empty;

    [XmlIgnore]
    public string Password
    {
        get => DecryptPassword();
        set => EncryptPassword(value);
    }

    public string Smtp { get; set; } = string.Empty;

    public uint Port { get; set; } = 0;

    public bool EnableSSL { get; set; } = true;

    // fields for en-/decryption
    private static readonly byte[] _salt = [0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76];
    private const string _encryptionKey = "I love C#.";

    // not very safe, but should be sufficient
    private void EncryptPassword(string password)
    {
        try
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(password);
            using (Aes encryptor = Aes.Create())
            {
#pragma warning disable SYSLIB0041
                Rfc2898DeriveBytes pdb = new(_encryptionKey, _salt);
#pragma warning restore SYSLIB0041

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                password = Convert.ToBase64String(ms.ToArray());
            }

            EncryptedPassword = password;
        }
        catch
        {
            EncryptedPassword = string.Empty;
        }
    }

    // not very safe, but should be sufficient
    private string DecryptPassword()
    {
        try
        {
            string cipherText = EncryptedPassword;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
#pragma warning disable SYSLIB0041
                Rfc2898DeriveBytes pdb = new(_encryptionKey, [0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76]);
#pragma warning restore SYSLIB0041
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = System.Text.Encoding.Unicode.GetString(ms.ToArray());
            }
            return cipherText;
        }
        catch
        {
            return string.Empty;
        }
    }
}