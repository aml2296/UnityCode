using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(Entity),true)]
public class EntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Entity");
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("---------------------------------");
    }
}
