using System;
using System.Collections.Generic;

namespace Core.Services.SavingSystem
{
    public abstract class DataParser
    {
        protected double GetDoubleData(DictionaryWrapperJson gameData, string key) =>
            ParseData<double>(gameData, key, double.TryParse, $"{key} game data not parsed.");

        protected double GetDoublePositiveData(DictionaryWrapperJson gameData, string key)
        {
            double result = GetDoubleData(gameData, key);

            if (result < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "Amount must be equal or greater than 0.");

            return result;
        }

        protected int GetIntData(DictionaryWrapperJson gameData, string key) =>
            ParseData<int>(gameData, key, int.TryParse, $"{key} game data not parsed.");

        protected int GetIntPositiveData(DictionaryWrapperJson gameData, string key)
        {
            int result = GetIntData(gameData, key);

            if (result < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "Amount must be equal or greater than 0.");

            return result;
        }

        protected bool GetBoolData(DictionaryWrapperJson gameData, string key) =>
            ParseData<bool>(gameData, key, bool.TryParse, $"{key} game data not parsed.");

        protected TEnum GetEnumData<TEnum>(DictionaryWrapperJson gameData, string key, bool ignoreCase = true)
            where TEnum : struct, Enum
        {
            string value = GetValue(gameData, key);

            if (Enum.TryParse<TEnum>(value, ignoreCase, out var result))
                return result;

            throw new FormatException($"Value '{value}' could not be parsed as enum of type {typeof(TEnum).Name}.");
        }

        protected bool TryGetValue(DictionaryWrapperJson gameData, string key, out string value)
        {
            value = null;

            if (gameData.ContainsKey(key) == false)
                return false;

            value = gameData.GetValue(key);
            return string.IsNullOrEmpty(value) == false;
        }

        private static T ParseData<T>(DictionaryWrapperJson gameData, string key,
            TryParseDelegate<T> tryParse, string errorMessage)
            where T : struct
        {
            string value = GetValue(gameData, key);

            if (tryParse(value, out T result))
                return result;

            throw new FormatException(errorMessage);
        }

        protected static string GetValue(DictionaryWrapperJson gameData, string key)
        {
            if (gameData.ContainsKey(key) == false)
                throw new KeyNotFoundException($"Key '{key}' not found in game data.");

            string value = gameData.GetValue(key);
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Value for key '{key}' is missing or empty.");

            return value;
        }

        private delegate bool TryParseDelegate<T>(string input, out T result);
    }
}