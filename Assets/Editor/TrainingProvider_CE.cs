using Assets.Scripts.Stores;
using Assets.Scripts.TWEANN;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PopulationTrainerProvider))]
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
            case Paradigms.TrainingParadigm.NEAT:
                EditorGUI.PropertyField(rect1, gaTrainerProp, true);
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
            case Paradigms.TrainingParadigm.NEAT:
                return 2 * EditorGUIUtility.singleLineHeight + (gaTrainerProp.isExpanded ? EditorGUI.GetPropertyHeight(gaTrainerProp) : 0);
        }
        return base.GetPropertyHeight(property, label);
    }
}