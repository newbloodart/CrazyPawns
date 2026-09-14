using UnityEngine;
using UnityEditor;

namespace CrazyPawn
{
    [CustomEditor(typeof(Checkerboard))]
    public class CheckerboardEditor : Editor
    {
        private static readonly Color STROKE_COLOR = new Color(0f, 0f, 1f, 1f);
        private static readonly Color FILL_COLOR = new Color(0f, 0f, 1f, 0.1f);

        private void OnSceneGUI()
        {
            Checkerboard checkerboard = target as Checkerboard;

            if (checkerboard.Settings != null)
            {
                float halfSize = checkerboard.SettingsExtended.CheckerboardCellSize *
                    checkerboard.Settings.CheckerboardSize / 2f;

                Vector3[] vertices = new Vector3[4];
                vertices[0] = Vector3Y0Utility.Create(-halfSize, halfSize);
                vertices[1] = Vector3Y0Utility.Create(-halfSize, -halfSize);
                vertices[2] = Vector3Y0Utility.Create(halfSize, -halfSize);
                vertices[3] = Vector3Y0Utility.Create(halfSize, halfSize);

                Handles.color = STROKE_COLOR;
                Handles.DrawSolidRectangleWithOutline(
                    vertices,
                    FILL_COLOR,
                    STROKE_COLOR
                );
            }
        }
    }
}