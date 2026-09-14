using UnityEngine;

namespace CrazyPawn
{
    public class WorldPlaneView : MonoBehaviour, IInputView<WorldPlaneView>
    {
        public event IInputView<WorldPlaneView>.Click OnClick;
        public event IInputView<WorldPlaneView>.PointerDown OnPointerDown;
        public event IInputView<WorldPlaneView>.PointerDrag OnPointerDrag;
        public event IInputView<WorldPlaneView>.PointerUp OnPointerUp;

        [SerializeField] private Camera _mainCamera;

        public bool TryGetPointOnPlaneFromMousePosition(out Vector3 point)
        {
            bool success = Physics.Raycast(
                _mainCamera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit,
                float.PositiveInfinity,
                1 << gameObject.layer
            );
            point = success ? hit.point.ToY0() : Vector3.zero;
            return success;
        }

        private void OnMouseDown() => OnPointerDown?.Invoke(this);
        private void OnMouseDrag() => OnPointerDrag?.Invoke(this);
        private void OnMouseUp() => OnPointerUp?.Invoke(this);
        private void OnMouseUpAsButton() => OnClick?.Invoke(this);
    }
}