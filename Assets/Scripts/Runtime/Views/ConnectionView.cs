using UnityEngine;

namespace CrazyPawn
{
    public class ConnectionView : MonoBehaviour
    {
        public PawnConnectorView Start => _start;
        public PawnConnectorView End => _end;

        [SerializeField] private LineRenderer _renderer;

        private PawnConnectorView _start = null;
        private PawnConnectorView _end = null;


        public void Initialize(PawnConnectorView start, PawnConnectorView end)
        {
            _start = start;
            _end = end;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            _renderer.SetPosition(0, _start.GetConnectionAnchor());
            _renderer.SetPosition(1, _end.GetConnectionAnchor());
        }
    }
}