using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatableEntity),true)]
[CanEditMultipleObjects]
public class AnimEditor : Editor
{
    SerializedProperty _animator;
    SerializedProperty _inputCount;
    SerializedProperty _entityActions;

    private void OnEnable()
    {
        _animator = serializedObject.FindProperty("animator");
        _inputCount = serializedObject.FindProperty("inputCount");
        _entityActions = serializedObject.FindProperty("entityActions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(_animator);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Action/State Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_inputCount);
        if (GUILayout.Button("Update Key Count"))
        {
            UpdateKeys();
        }
        EditorGUILayout.EndHorizontal();

        if (_entityActions != null)
            EditorGUILayout.PropertyField(_entityActions);


        serializedObject.ApplyModifiedProperties();
    }
    public void UpdateKeys()
    {
        AnimatableEntity aEnt = target as AnimatableEntity;
        aEnt.UpdateActionKeys();
    }
}