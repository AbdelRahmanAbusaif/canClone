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
            byte[] encryptedData = XorEncryptDecrypt(data);
            File.WriteAllBytes(filePath, encryptedData);
            Debug.Log($"File encrypted and saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error encrypting file: {e.Message}");
        }
    }

    public static byte[] LoadAndDecryptFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                byte[] encryptedData = File.ReadAllBytes(filePath);
                return XorEncryptDecrypt(encryptedData); // XOR works both for encryption and decryption
            }
            Debug.LogError($"File not found: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error decrypting file: {e.Message}");
        }
        return null;
    }

    // XOR Encryption/Decryption Function
    private static byte[] XorEncryptDecrypt(byte[] data)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
        byte[] result = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]); // XOR with key in a circular manner
        }

        return result;
    }
}
