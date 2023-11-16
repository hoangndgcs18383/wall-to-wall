#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[Serializable]
public class BackgroundData
{
    public string key;
    public Texture2D backgroundTexture;
}

[CreateAssetMenu(fileName = "BackgroundAddressTools", menuName = "Editor/BackgroundAddressTools", order = 1)]
public class BackgroundAddressTools : ScriptableObject
{
    [TableList] public List<BackgroundData> backgroundDataList = new List<BackgroundData>();

    [Sirenix.OdinInspector.FilePath]
    public string templatePath = "WallToWall/Scripts/Template/BackgroundAddressTemplate.txt";

    public string generatePath = "WallToWall/Scripts/Generated/BackgroundAddress.cs";


    [Button]
    public void Build()
    {
        string template = File.ReadAllText(Path.Combine(Application.dataPath, templatePath));
        template = template.Replace("{0}", "BackgroundAddress");
        template = template.Replace("{1}", "GetAddress");
        StringBuilder sb = new StringBuilder();
        foreach (var backgroundData in backgroundDataList)
        {
            AssignAddress(backgroundData.backgroundTexture, "Backgrounds", backgroundData.key);
            sb.AppendLine("\t\t\tcase \"" + backgroundData.key + "\":");
            sb.AppendLine("\t\t\t\treturn \"" +
                          GetGuid(backgroundData.backgroundTexture) + "\";");
        }

        template = template.Replace("{2}", sb.ToString());
        File.WriteAllText(Path.Combine(Application.dataPath, generatePath),
            template);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button]
    public void ParseTemplate()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("// This file is auto-generated. DO NOT EDIT");
        sb.AppendLine("public static class {0}");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static string {1}(string key)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tswitch (key)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("{2}");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\treturn null;");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(Application.dataPath, templatePath), sb.ToString());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void AssignAddress<T>(T data, string groupName, string address) where T : UnityEngine.Object
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
            group = settings.CreateGroup(groupName, false, false, true, new List<AddressableAssetGroupSchema>());
        AddressableAssetEntry entry =
            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data)), group);
        entry.address = address;
        //create schema content upload restriction
        AddressableAssetGroupSchema schema = group.GetSchema<AddressableAssetGroupSchema>();
        if(schema == null)
            schema = group.AddSchema<AddressableAssetGroupSchema>();
    }

    private string GetGuid<T>(T data) where T : UnityEngine.Object
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetEntry entry =
            settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data)));
        return entry.guid;
    }
}
#endif