#if UNITY_EDITOR
 using UnityEngine;
using UnityEditor;


public class RemoteEditor : EditorWindow
{
    private string[] _tabs = new string[] { "Remote" };
    private int _selectedTab = 0;
    private RemoteData _remoteData;

    [MenuItem("Window/Remote Editor")]
    public static void ShowWindow()
    {
        GetWindow<RemoteEditor>("Remote Editor");
    }

    private void OnGUI()
    {
        _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);
        switch (_selectedTab)
        {
            case 0:
                DrawRemoteTab();
                break;
        }
    }

    private void DrawRemoteTab()
    {
        if (_remoteData == null)
        {
            _remoteData = CreateInstance<RemoteData>();
        }

        _remoteData.sheetId = EditorGUILayout.TextField("Sheet Id", _remoteData.sheetId);
        _remoteData.sheetName = EditorGUILayout.TextField("Sheet Name", _remoteData.sheetName);

        if (GUILayout.Button("Save"))
        {
            string path = "Assets/Resources/RemoteData.asset";

            if (AssetDatabase.LoadAssetAtPath<RemoteData>(path) != null) return;

            AssetDatabase.CreateAsset(_remoteData, path);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif