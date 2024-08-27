using Playroom;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(PlayroomkitDevManager))]
public class PlayroomkitDevManagerEditor : Editor
{
    private string[] mockModeOptions = new string[] { "Local (simulated)", "Browser Bridge (live)" };
    private int selectedMockModeIndex = 0;
    private AnimBool showInsertCoinHelp; // Use AnimBool for smooth fade transitions
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;

    void OnEnable()
    {
        showInsertCoinHelp = new AnimBool(false);
        showInsertCoinHelp.valueChanged.AddListener(Repaint);


        labelStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,

            margin = new RectOffset(500, 0, 0, 0),

            fontSize = 14,
            normal = { textColor = Color.white },
        };

        buttonStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 18,
            normal = { textColor = Color.white },
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        selectedMockModeIndex = EditorGUILayout.Popup("Mock Mode", selectedMockModeIndex, mockModeOptions);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Insert Coin Caller", labelStyle);
        if (GUILayout.Button("(?)", buttonStyle))
        {
            showInsertCoinHelp.target = !showInsertCoinHelp.target;
        }


        EditorGUILayout.PropertyField(serializedObject.FindProperty("insertCoinCaller"), GUIContent.none);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(showInsertCoinHelp.faded))
        {
            EditorGUILayout.HelpBox(
                "InsertCoin must be called in order to connect PlayroomKit to a server.\nChoose the script which calls `InsertCoin`, here.",
                MessageType.Info);
        }

        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Launch Player"))
        {
            Debug.Log("Player Launched");
        }

        serializedObject.ApplyModifiedProperties();
    }
}