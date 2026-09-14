using UnityEngine;
using UnityEditor;

namespace CrazyPawn
{
    [CustomEditor(typeof(PawnsManager))]
    public class PawnsManagerEditor : Editor
    {
        private static readonly Color STROKE_COLOR = new Color(0f, 1f, 0f, 1f);
        private static readonly Color FILL_COLOR = new Color(0f, 1f, 0f, 0.1f);

        private void OnSceneGUI()
        {
            PawnsManager manager = target as PawnsManager;

            if (manager.Settings != null)
            {
                Handles.color = STROKE_COLOR;
                Handles.DrawWireDisc(
                    manager.transform.position,
                    Vector3.up,
                    manager.Settings.InitialZoneRadius
                );
                Handles.color = FILL_COLOR;
                Handles.DrawSolidDisc(
                    manager.transform.position,
                    Vector3.up,
                    manager.Settings.InitialZoneRadius
                );
            }
        }
    }
}