using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioTagEditor : Editor
{
    /*private AudioManager _audioManager;
    private SerializedProperty _audioTags;

    private void OnEnable()
    {
        _audioManager = (AudioManager)target;
        _audioTags = serializedObject.FindProperty("_sfxClips");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_audioTags, true);
        serializedObject.ApplyModifiedProperties();
    }*/
}