using UnityEngine;

namespace CrazyPawn
{
    [CreateAssetMenu(menuName = "CrazyPawn/Settings Extended", fileName = "CrazyPawnSettingsExtended")]
    public class CrazyPawnSettingsExtended : ScriptableObject
    {
        [Header("Material settings")]
        [SerializeField] public Material BaseMaterial;

        [Space]
        [Header("Checkerboard settings")]
        [SerializeField] public bool CheckerboardVisible = true;
        [SerializeField] public float CheckerboardCellSize = 1.5f;

        [Space]
        [Header("Camera settings")]
        [SerializeField] public float PanSensitivity = 1f;
        [SerializeField] public float ZoomSensitivity = 1f;
        [SerializeField] public float MinZoomHeight = 2f;
        [SerializeField] public float MaxZoomHeight = 100f;
    }
}