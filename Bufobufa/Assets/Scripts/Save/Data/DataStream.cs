using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class DataStream : IDisposable
{
    private FileStream stream;

    private static byte[] GetIV(string ivSecret)
    {
        using MD5 md5 = MD5.Create();
        return md5.ComputeHash(Encoding.UTF8.GetBytes(ivSecret));
    }
    private static byte[] GetKey(string key)
    {
        using SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted = null;
        try
        {
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
        }
        catch { }
        return encrypted;
    }

    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        string plaintext = null;

        try
        {
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
        }
        catch { }

        return plaintext;
    }

    public async void Serialize(string path, object data)
    {
        if (data == null)
            return;
        if (IsCreateFileSave(path) == false)
            stream = File.Create(path);
        else
            stream = File.Open(path, FileMode.Open);


        string key = "93a1b87b-a4be-4ca8-8fdd-9d312af679a4"; // Ключ для шифрования
        string ivSecret = "BufaBufo"; // вектор шифрования

        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(EncryptStringToBytes(JsonConvert.SerializeObject(data), GetKey(key), GetIV(ivSecret)));
        
        binaryWriter.Flush();
        binaryWriter.Close();
        stream.Close();
    }
    public T Deserialize<T>(string path)
    {
        string key = "93a1b87b-a4be-4ca8-8fdd-9d312af679a4"; // Ключ для шифрования
        string ivSecret = "BufaBufo"; // вектор шифрования

        if (CountSymbolInFile(path) == 0)
            return default(T);

        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            string jsonText = DecryptStringFromBytes(bytes, GetKey(key), GetIV(ivSecret));
            return JsonConvert.DeserializeObject<T>(jsonText);
        }
        catch { }

        File.Delete(path);

        return default(T);
    }

    public void Delete(string path)
    {
        if (IsCreateFileSave(path) == false)
            return;
        else
            File.Delete(path);
    }

    public void DeleteAll(string path)
    {
        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }

    }

    public int CountSymbolInFile(string path)
    {
        if (IsCreateFileSave(path) == false)
            return 0;

        return File.ReadAllText(path).Length;
    }

    public bool IsCreateFileSave(string path)
    {
        return File.Exists(path);
    }

    public void Dispose()
    {
        stream.Close();
    }

    ~DataStream() 
    { 
    Dispose();
    }
}
