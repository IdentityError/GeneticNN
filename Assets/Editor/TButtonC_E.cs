// Copyright (c) 2020 Matteo Beltrame

using Assets.Scripts.TUtils.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(TButton))]
public class TButtonC_E : Editor
{
    private TButton script;
    private SerializedProperty pointerDownProp, pointerUpProp;

    private void OnEnable()
    {
        script = (TButton)target;

        pointerDownProp = serializedObject.FindProperty("onPressed");
        pointerUpProp = serializedObject.FindProperty("onReleased");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(pointerDownProp);
        EditorGUILayout.PropertyField(pointerUpProp);
        script.colorPressEffect = EditorGUILayout.Toggle("Pressed color effect", script.colorPressEffect);
        if (script.colorPressEffect)
        {
            script.pressedColor = EditorGUILayout.ColorField("Pressed color", script.pressedColor);
        }
        script.resizeOnPress = EditorGUILayout.Toggle("Resize on press", script.resizeOnPress);
        if (script.resizeOnPress)
        {
            script.pressedScaleMultiplier = EditorGUILayout.Vector2Field("Resize multiplier", script.pressedScaleMultiplier);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
            serializedObject.ApplyModifiedProperties();
        }
    }
}