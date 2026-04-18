using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using VContainer;

namespace Core.Services.SavingSystem
{
    public interface ISecureSaveSystem
    {
        void SaveGame();
        void LoadGame();
    }
    
    public class SecureSaveSystem : ISecureSaveSystem
    {
        private IDataSubject _gameDataProvider;

        private string _filePath = Path.Combine(Application.persistentDataPath, "secureSave.dat");
        //private static readonly string encryptionKey = "3Up4cYHImJ9r6v1A3Up4cYHImJ9r6v1A"; // ⚠ Храни ключ в безопасном месте!

        [Inject]
        private void Construct(IDataSubject gameDataProvider)
        {
            _gameDataProvider = gameDataProvider;
        }

        public void SaveGame()
        {
            DictionaryWrapperJson data = _gameDataProvider.GetGameData();

            string json = JsonUtility.ToJson(data);
            //string encryptedJson = Encrypt(json, encryptionKey);

            //File.WriteAllText(filePath, encryptedJson);
            File.WriteAllText(_filePath, json);
            Debug.Log("Game saved securely. Path: " +  _filePath);
        }

        public void LoadGame()
        {
            if (File.Exists(_filePath))
            {
                string encryptedJson = File.ReadAllText(_filePath);
                //string decryptedJson = Decrypt(encryptedJson, encryptionKey);

                //GameData data = JsonUtility.FromJson<GameData>(decryptedJson);
                DictionaryWrapperJson data = JsonUtility.FromJson<DictionaryWrapperJson>(encryptedJson);
                _gameDataProvider.SetGameData(data);

                Debug.Log("Loaded securely. Path: " +  _filePath);
                Debug.Log(encryptedJson);
            }
            else
            {
                Debug.Log("No save file found.");
            }
        }

        private static string Encrypt(string text, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16]; // Инициализационный вектор (IV)
            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(textBytes, 0, textBytes.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string encryptedText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16];
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream();
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}