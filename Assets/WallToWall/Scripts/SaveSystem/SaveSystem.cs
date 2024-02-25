using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveSystem
{
    private static SaveSystem _instance;
    private const string _privateCode = "0QX6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9";
    private static string[] _encryptionKeys;

    private const string _typeInt = "int";
    private const string _typeFloat = "float";
    private const string _typeString = "string";


    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _encryptionKeys = new string[3];

                _encryptionKeys[0] = "x6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9";
                _encryptionKeys[1] = "3fX6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9";
                _encryptionKeys[2] = "6fX6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9";

                _instance = new SaveSystem();
            }

            return _instance;
        }
    }

    #region Setters

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        SaveEncryption(key, _typeInt, value.ToString());
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        SaveEncryption(key, _typeFloat, value.ToString());
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        SaveEncryption(key, _typeString, value);
    }

    #endregion

    #region Getters

    public int GetInt(string key, int defaultValue = 0)
    {
        return GetIntDefault(key, defaultValue);
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        return GetFloatDefault(key, defaultValue);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return GetStringDefault(key, defaultValue);
    }

    private static int GetIntDefault(string key, int defaultValue)
    {
        int value = PlayerPrefs.GetInt(key, defaultValue);
        if (!VerifyEncryption(key, _typeInt, value.ToString()))
        {
            return defaultValue;
        }

        return PlayerPrefs.GetInt(key, defaultValue);
    }

    private static float GetFloatDefault(string key, float defaultValue)
    {
        float value = PlayerPrefs.GetFloat(key, defaultValue);
        if (!VerifyEncryption(key, _typeFloat, value.ToString(CultureInfo.InvariantCulture)))
        {
            return defaultValue;
        }

        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    private static string GetStringDefault(string key, string defaultValue)
    {
        string value = PlayerPrefs.GetString(key, defaultValue);
        if (!VerifyEncryption(key, _typeString, value))
        {
            return defaultValue;
        }

        return PlayerPrefs.GetString(key, defaultValue);
    }

    #endregion

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey($"{key}_encryption_check");
        PlayerPrefs.DeleteKey($"{key}_used_key");
    }

    private static void SaveEncryption(string key, string type, string value)
    {
        int keyIndex = (int)Mathf.Floor(Random.value * _encryptionKeys.Length);
        string keyToSave = _encryptionKeys[keyIndex];
        string encryptedValue = ComputeHash($"{type}_{_privateCode}_{keyToSave}_{value}");
        PlayerPrefs.SetString($"{key}_encryption_check", encryptedValue);
        PlayerPrefs.SetInt($"{key}_used_key", keyIndex);
    }

    private static bool VerifyEncryption(string key, string type, string value)
    {
        int keyIndex = PlayerPrefs.GetInt($"{key}_used_key", -1);
        if (keyIndex == -1)
        {
            Debug.LogError("No key found");
            return false;
        }

        string keyToSave = _encryptionKeys[keyIndex];
        string encryptedValue = ComputeHash($"{type}_{_privateCode}_{keyToSave}_{value}");
        string savedValue = PlayerPrefs.GetString($"{key}_encryption_check", "");

        Debug.Log($"Saved value: {savedValue} - Encrypted value: {encryptedValue}");

        if (encryptedValue != savedValue)
        {
            Debug.LogError("Encryption check failed");
        }

        return encryptedValue == savedValue;
    }

    private static string ComputeHash(string str)
    {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(str);

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        StringBuilder hashStringBuilder = new StringBuilder();

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashStringBuilder.Append((hashBytes[i].ToString("x2")));
        }

        return hashStringBuilder.ToString();
    }
}