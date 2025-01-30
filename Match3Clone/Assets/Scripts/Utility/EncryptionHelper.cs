using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class EncryptionHelper : MonoBehaviour
{
    private static string EncryptionKey = "YourSecretKey123"; // Change it from Remote Config
    
    private void Awake()
    {
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
    }

    private void ApplyRemoteConfig(ConfigResponse response)
    {
        EncryptionKey = RemoteConfigService.Instance.appConfig.GetString("EncryptionKey");
    }

    // Encrypt and save file

    public static void EncryptAndSaveFile(string filePath, byte[] data)
    {
        try
        {
            byte[] encryptedData = EncryptData(data, EncryptionKey);
            File.WriteAllBytes(filePath, encryptedData);
            Debug.Log($"File encrypted and saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error encrypting file: {e.Message}");
        }
    }

    // Load and decrypt file
    public static byte[] LoadAndDecryptFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                byte[] encryptedData = File.ReadAllBytes(filePath);
                return DecryptData(encryptedData, EncryptionKey);
            }
            Debug.LogError($"File not found: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error decrypting file: {e.Message}");
        }
        return null;
    }

    // Encrypt data using AES
    private static byte[] EncryptData(byte[] data, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = GenerateKey(key);
        aes.IV = new byte[16]; // Zero IV (for simplicity, but should be unique for each encryption)

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return PerformCryptography(data, encryptor);
    }

    // Decrypt data using AES
    private static byte[] DecryptData(byte[] encryptedData, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = GenerateKey(key);
        aes.IV = new byte[16]; // Must be the same IV used for encryption

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return PerformCryptography(encryptedData, decryptor);
    }

    // Convert key to 256-bit key (AES standard)
    private static byte[] GenerateKey(string key)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }

    // Helper function to perform encryption and decryption
    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();
        return memoryStream.ToArray();
    }
}
