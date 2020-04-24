using UnityEngine;
using UnityEditor;

namespace HatFeather.HealthControl.Editors
{
    [CustomEditor(typeof(HealthPool))]
    public class HealthPoolEditor : Editor
    {
        private HealthPool instance => target as HealthPool;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            var space = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            EditorGUI.DrawRect(space, new Color(0, 0, 0, 0.1f));
            var hRect = new Rect(space.x, space.y, space.width * instance.percentHealth, space.height);
            EditorGUI.DrawRect(hRect, new Color(0, 1, 1, 0.3f));

            GUILayout.Label(string.Format("{0} / {1} ({2:0.00}%)", instance.health,
                instance.maxHealth, instance.percentHealth * 100));

            EditorGUILayout.EndVertical();
        }
    }
}