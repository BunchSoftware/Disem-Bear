using System.Diagnostics;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogManagerEditor : Editor
{
    SerializedProperty dialogue;
    bool dialogeGroup = false;

    private void OnEnable()
    {
        dialogue = serializedObject.FindProperty("dialogue");
    }

    public override void OnInspectorGUI()
    {
        DialogueTrigger _dialogueTrigger = (DialogueTrigger)target;

        serializedObject.Update(); // начало нашего отображения

        dialogeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(dialogeGroup, "Реплики диалога:");
        if (dialogeGroup)
        {
            for (int i = 0; i < dialogue.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(dialogue.GetArrayElementAtIndex(i).FindPropertyRelative("dialogType"));
                if (_dialogueTrigger.dialogue[i].dialogType == DialogType.Text) EditorGUILayout.PropertyField(dialogue.GetArrayElementAtIndex(i).FindPropertyRelative("DialogueText"));
                else EditorGUILayout.PropertyField(dialogue.GetArrayElementAtIndex(i).FindPropertyRelative("DialogueChoice"));
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Добавить", GUILayout.Height(20), GUILayout.Width(100))) dialogue.InsertArrayElementAtIndex(dialogue.arraySize);
        if (dialogue.arraySize > 0) if (GUILayout.Button("Удалить", GUILayout.Height(20), GUILayout.Width(100))) dialogue.DeleteArrayElementAtIndex(dialogue.arraySize - 1);
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties(); // конец нашего отображения
    }
}
