using UnityEngine;

namespace CrazyPawn
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private WorldPlaneView _worldPlane;
        [SerializeField] private CrazyPawnSettingsExtended _settingsExtended;

        private Camera _camera;
        private Vector3 _panStart;

        private void Awake() => _camera = GetComponent<Camera>();

        private void OnEnable()
        {
            _worldPlane.OnPointerDown += WorldPlane_OnPointerDown;
            _worldPlane.OnPointerDrag += WorldPlane_OnPointerDrag;
        }

        private void OnDisable()
        {
            _worldPlane.OnPointerDown -= WorldPlane_OnPointerDown;
            _worldPlane.OnPointerDrag -= WorldPlane_OnPointerDrag;
        }

        private void Update() => UpdateZoom();

        private void WorldPlane_OnPointerDown(WorldPlaneView view)
        {
            if (TryGetPointOnGroundPlane(out Vector3 point))
                _panStart = point;
        }

        private void WorldPlane_OnPointerDrag(WorldPlaneView view)
        {
            if (!TryGetPointOnGroundPlane(out Vector3 point))
                return;

            transform.position += (_panStart - point) * _settingsExtended.PanSensitivity;
        }

        private void UpdateZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Approximately(scroll, 0f))
                return;

            if (!TryGetPointOnGroundPlane(out Vector3 target))
                return;

            Vector3 offset = transform.position - target;
            float distance = offset.magnitude;
            if (Mathf.Approximately(distance, 0f))
                return;

            float newDistance = distance - scroll * _settingsExtended.ZoomSensitivity * distance;
            Vector3 newPosition = target + (offset / distance) * newDistance;
            newPosition.y = Mathf.Clamp(newPosition.y, _settingsExtended.MinZoomHeight, _settingsExtended.MaxZoomHeight);

            transform.position = newPosition;
        }

        private bool TryGetPointOnGroundPlane(out Vector3 point)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Mathf.Approximately(ray.direction.y, 0f) || ray.origin.y * ray.direction.y >= 0f)
            {
                point = default;
                return false;
            }

            float t = -ray.origin.y / ray.direction.y;
            point = ray.GetPoint(t);
            return true;
        }
    }
}