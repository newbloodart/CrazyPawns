using UnityEngine;

namespace CrazyPawn
{
    public class PawnConnectorView : MonoBehaviour, IInputView<PawnConnectorView>
    {
        public PawnView Owner => _owner;
        public bool IsHighlighted => _highlighted;

        public event IInputView<PawnConnectorView>.Click OnClick;
        public event IInputView<PawnConnectorView>.PointerDown OnPointerDown;
        public event IInputView<PawnConnectorView>.PointerDrag OnPointerDrag;
        public event IInputView<PawnConnectorView>.PointerUp OnPointerUp;

        [SerializeField] private PawnView _owner;
        [SerializeField] private Renderer _renderer;

        private PawnsManager _manager;
        private bool _outOfBounds = false;
        private bool _highlighted = false;

        public void Initialize(PawnsManager manager) => _manager = manager;

        public void SetOutOfBounds(bool value)
        {
            if (_outOfBounds == value)
                return;

            _outOfBounds = value;
            UpdateVisual();
        }

        public void SetHighlighted(bool value)
        {
            if (_highlighted == value)
                return;

            _highlighted = value;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            Material material;
            if (_outOfBounds)
                material = _manager.Settings.DeleteMaterial;
            else if (_highlighted)
                material = _manager.Settings.ActiveConnectorMaterial;
            else
                material = _manager.SettingsExtended.BaseMaterial;

            _renderer.sharedMaterial = material;
        }

        public Vector3 GetConnectionAnchor() => transform.position;

        private void OnMouseDown() => OnPointerDown?.Invoke(this);

        private void OnMouseDrag() => OnPointerDrag?.Invoke(this);

        private void OnMouseUp() => OnPointerUp?.Invoke(this);

        private void OnMouseUpAsButton() => OnClick?.Invoke(this);
    }
}