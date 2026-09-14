using UnityEngine;

namespace CrazyPawn
{
    public class PawnView : MonoBehaviour, IInputView<PawnView>
    {
        public PawnConnectorView[] Connectors => _connectors;
        public bool IsOutOfBounds => _outOfBounds;

        public event IInputView<PawnView>.Click OnClick;
        public event IInputView<PawnView>.PointerDown OnPointerDown;
        public event IInputView<PawnView>.PointerDrag OnPointerDrag;
        public event IInputView<PawnView>.PointerUp OnPointerUp;

        [SerializeField] private PawnConnectorView[] _connectors;
        [SerializeField] private Renderer _renderer;

        private PawnsManager _manager;
        private bool _outOfBounds = false;

        public void Initialize(PawnsManager manager)
        {
            _manager = manager;
            for (int i = 0; i < _connectors.Length; i++)
                _connectors[i].Initialize(manager);
        }

        public void MoveTo(Vector3 target) => transform.position = target;

        public Vector3 GetCenter() => transform.position.ToY0();

        public void SetOutOfBounds(bool value)
        {
            if (_outOfBounds == value)
                return;

            _outOfBounds = value;

            _renderer.sharedMaterial = value ? _manager.Settings.DeleteMaterial : _manager.SettingsExtended.BaseMaterial;
            for (int i = 0; i < _connectors.Length; i++)
                _connectors[i].SetOutOfBounds(value);
        }

        private void OnMouseDown() => OnPointerDown?.Invoke(this);

        private void OnMouseDrag() => OnPointerDrag?.Invoke(this);

        private void OnMouseUp() => OnPointerUp?.Invoke(this);

        private void OnMouseUpAsButton() => OnClick?.Invoke(this);
    }
}