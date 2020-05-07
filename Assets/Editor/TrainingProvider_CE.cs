using Assets.Scripts.Providers;
using Assets.Scripts.Stores;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TrainerProvider))]
public class TrainingProvider_CE : PropertyDrawer
{
    private SerializedProperty gaTrainerProp, bpaTrainerProp, trainingParadigmProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var rect1 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(rect, trainingParadigmProp);
        switch ((Paradigms.TrainingParadigm)trainingParadigmProp.intValue)
        {
            case Paradigms.TrainingParadigm.GENETIC:
                EditorGUI.PropertyField(rect1, gaTrainerProp, true);
                break;

            case Paradigms.TrainingParadigm.BPA:
                EditorGUI.PropertyField(rect1, bpaTrainerProp, true);
                break;
        }

        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        gaTrainerProp = property.FindPropertyRelative("gaTrainer");
        bpaTrainerProp = property.FindPropertyRelative("bpaTrainer");
        trainingParadigmProp = property.FindPropertyRelative("trainingParadigm");
        switch ((Paradigms.TrainingParadigm)trainingParadigmProp.intValue)
        {
            case Paradigms.TrainingParadigm.GENETIC:
                return 3 * EditorGUIUtility.singleLineHeight + (gaTrainerProp.isExpanded ? EditorGUIUtility.singleLineHeight * 6 : 0);

            case Paradigms.TrainingParadigm.BPA:
                return 3 * EditorGUIUtility.singleLineHeight + (bpaTrainerProp.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : 0);
        }
        return base.GetPropertyHeight(property, label);
    }
}