using UnityEngine;
using UnityEditor;

namespace HatFeather.HealthControl.Editors
{
    [CustomPropertyDrawer(typeof(HealthPool))]
    public class HealthPoolEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spaceHeight = EditorGUIUtility.standardVerticalSpacing;
            var indentWidth = 16;
            var maxHealthProp = property.FindPropertyRelative("_maxHealth");
            var healthProp = property.FindPropertyRelative("_health");

            // health/max health properties
            var maxHealthRect = new Rect(position.x, position.y + lineHeight + spaceHeight,
                position.width, lineHeight);
            var healthRect = new Rect(position.x, position.y + lineHeight * 2 + spaceHeight * 2,
                position.width, lineHeight);

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(maxHealthRect, maxHealthProp, new GUIContent("Max Health"));
            EditorGUI.PropertyField(healthRect, healthProp, new GUIContent("Health"));

            // health bar display
            var maxHealth = maxHealthProp.intValue;
            var health = healthProp.intValue;
            var healthPercent = (float)health / maxHealth;

            var healthBarRect_BG = new Rect(position.x + indentWidth, position.y + lineHeight * 3 + spaceHeight * 3,
                position.width - indentWidth, lineHeight);
            var healthBarRect_FG = new Rect(position.x + indentWidth, position.y + lineHeight * 3 + spaceHeight * 3,
                (position.width - indentWidth) * Mathf.Clamp01(healthPercent), lineHeight);

            EditorGUI.DrawRect(healthBarRect_BG, new Color(0, 0, 0, 0.1f));
            EditorGUI.DrawRect(healthBarRect_FG, new Color(0, 1, 1, 0.3f));
            GUI.Label(healthBarRect_BG, string.Format($"{health} / {maxHealth} ({healthPercent * 100:0.00}%)"));

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        }

        // private HealthPool instance => target as HealthPool;

        // public override void OnInspectorGUI()
        // {
        //     DrawDefaultInspector();
        //     EditorGUILayout.Space();

        //     var space = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        //     EditorGUI.DrawRect(space, new Color(0, 0, 0, 0.1f));
        //     var hRect = new Rect(space.x, space.y, space.width * instance.percentHealth, space.height);
        //     EditorGUI.DrawRect(hRect, new Color(0, 1, 1, 0.3f));

        //     GUILayout.Label(string.Format("{0} / {1} ({2:0.00}%)", instance.health,
        //         instance.maxHealth, instance.percentHealth * 100));

        //     EditorGUILayout.EndVertical();
        // }
    }
}