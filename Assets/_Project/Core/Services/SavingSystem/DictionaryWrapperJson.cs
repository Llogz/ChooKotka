using System.Collections.Generic;

namespace Core.Services.SavingSystem
{
    [System.Serializable]
    public class DictionaryWrapperJson
    {
        public List<KeyValueWrapperJson> WrappedDictionary = new();

        public void AddRawData(string name, string value)
        {
            var keyValueWrapperJson = new KeyValueWrapperJson(name, value);
            WrappedDictionary.Add(keyValueWrapperJson);
        }

        public void AddRange(DictionaryWrapperJson newDictionary)
        {
            if (newDictionary == null)
                throw new System.ArgumentNullException(nameof(newDictionary));

            WrappedDictionary.AddRange(newDictionary.WrappedDictionary);
        }

        public bool ContainsKey(string key)
        {
            foreach (var keyValueWrapperJson in WrappedDictionary)
            {
                if (keyValueWrapperJson.Key == key)
                    return true;
            }

            return false;
        }

        public string GetValue(string key)
        {
            foreach (var keyValueWrapperJson in WrappedDictionary)
            {
                if (keyValueWrapperJson.Key == key)
                    return keyValueWrapperJson.Value;
            }

            return null;
        }

        private void ConvertFromDictionary(Dictionary<string, string> dictionary)
        {
            foreach (var keyValuePair in dictionary)
            {
                var keyValueWrapperJson = new KeyValueWrapperJson(keyValuePair.Key, keyValuePair.Value);
                WrappedDictionary.Add(keyValueWrapperJson);
            }
        }


        [System.Serializable]
        public class KeyValueWrapperJson
        {
            public string Key;
            public string Value;

            public KeyValueWrapperJson(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}