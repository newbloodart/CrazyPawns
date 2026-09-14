using UnityEngine;
using System.Collections.Generic;

namespace CrazyPawn
{
    public class ConnectionsManager : MonoBehaviour
    {
        [SerializeField] private GameObject _connectionPrefab;
        [SerializeField] private PawnsManager _pawnsManager;
        [SerializeField] private WorldPlaneView _worldPlane;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _connectorsLayer;

        private PawnConnectorView _firstConnector;
        private PawnConnectorView _pressedConnector;
        private bool _clickHandled;

        private readonly HashSet<(PawnConnectorView start, PawnConnectorView end)> _lookup = new();
        private readonly List<PawnConnectorView> _highlightedConnectors = new();
        private readonly Dictionary<PawnView, List<ConnectionView>> _connectionsByPawn = new();

        private void OnEnable() => _worldPlane.OnPointerDown += OnWorldPlanePointerDown;
        private void OnDisable() => _worldPlane.OnPointerDown -= OnWorldPlanePointerDown;

        public void CancelPendingConnection()
        {
            if (_firstConnector != null)
                ResetState();
        }

        public void RegisterConnectors(PawnView pawn)
        {
            foreach (PawnConnectorView connector in pawn.Connectors)
            {
                connector.OnPointerDown += Connector_OnPointerDown;
                connector.OnClick += Connector_OnClick;
                connector.OnPointerUp += Connector_OnPointerUp;
            }
        }

        public void UnregisterConnectors(PawnView pawn)
        {
            if (_connectionsByPawn.TryGetValue(pawn, out List<ConnectionView> connections))
            {
                ConnectionView[] snapshot = connections.ToArray();
                foreach (ConnectionView connection in snapshot)
                    RemoveConnection(connection);
            }
            _connectionsByPawn.Remove(pawn);

            foreach (PawnConnectorView connector in pawn.Connectors)
            {
                if (_firstConnector == connector || _pressedConnector == connector)
                {
                    ResetState();
                }
                else if (_highlightedConnectors.Remove(connector))
                {
                    connector.SetHighlighted(false);
                }

                connector.OnPointerDown -= Connector_OnPointerDown;
                connector.OnClick -= Connector_OnClick;
                connector.OnPointerUp -= Connector_OnPointerUp;
            }
        }

        public void UpdateConnectionsVisuals(PawnView pawn)
        {
            if (!_connectionsByPawn.TryGetValue(pawn, out List<ConnectionView> connections))
                return;

            for (int i = 0; i < connections.Count; i++)
                connections[i].UpdateVisual();
        }

        #region Connector interaction

        private void OnWorldPlanePointerDown(WorldPlaneView view) => CancelPendingConnection();

        private void Connector_OnPointerDown(PawnConnectorView connector)
        {
            if (_firstConnector != null)
                ResetState();

            _pressedConnector = connector;
            _clickHandled = false;
            HighlightAvailableConnectors(connector);
        }

        private void Connector_OnClick(PawnConnectorView connector)
        {
            _clickHandled = true;

            if (_firstConnector == null)
                _firstConnector = connector;
            else
            {
                TryCreateConnection(_firstConnector, connector);
                ResetState();
            }
        }

        private void Connector_OnPointerUp(PawnConnectorView connector)
        {
            if (_pressedConnector == null)
                return;

            PawnConnectorView pressed = _pressedConnector;
            _pressedConnector = null;

            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            PawnConnectorView target = RaycastConnector();
            if (target != null && target != pressed)
                TryCreateConnection(pressed, target);

            ResetState();
        }

        #endregion

        #region Helpers

        private void HighlightAvailableConnectors(PawnConnectorView startConnector)
        {
            foreach (PawnView pawn in _pawnsManager.Pawns)
            {
                if (pawn == startConnector.Owner)
                    continue;

                foreach (PawnConnectorView endConnector in pawn.Connectors)
                {
                    if (!CheckConnectionAvailability(startConnector, endConnector))
                        continue;

                    endConnector.SetHighlighted(true);
                    _highlightedConnectors.Add(endConnector);
                }
            }
        }

        private void ClearHighlights()
        {
            foreach (PawnConnectorView connector in _highlightedConnectors)
            {
                if (connector != null)
                    connector.SetHighlighted(false);
            }
            _highlightedConnectors.Clear();
        }

        private void ResetState()
        {
            _firstConnector = null;
            _pressedConnector = null;
            _clickHandled = false;
            ClearHighlights();
        }

        private PawnConnectorView RaycastConnector()
        {
            if (Physics.Raycast(
                _mainCamera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit,
                float.PositiveInfinity,
                _connectorsLayer.value))
                return hit.collider.GetComponentInParent<PawnConnectorView>();

            return null;
        }

        private bool TryCreateConnection(PawnConnectorView start, PawnConnectorView end)
        {
            if (!CheckConnectionAvailability(start, end))
                return false;

            ConnectionView connection = Instantiate(
                _connectionPrefab,
                Vector3.zero,
                Quaternion.identity,
                transform
            ).GetComponent<ConnectionView>();
            connection.Initialize(start, end);
            _lookup.Add((start, end));
            RegisterConnectionWithPawn(connection, start.Owner);
            RegisterConnectionWithPawn(connection, end.Owner);

            return true;
        }

        private void RemoveConnection(ConnectionView connection)
        {
            if (connection == null)
                return;

            _lookup.Remove((connection.Start, connection.End));
            DetachFromPawn(connection, connection.Start != null ? connection.Start.Owner : null);
            DetachFromPawn(connection, connection.End != null ? connection.End.Owner : null);

            Destroy(connection.gameObject);
        }

        private void RegisterConnectionWithPawn(ConnectionView connection, PawnView pawn)
        {
            if (pawn == null)
                return;

            if (!_connectionsByPawn.TryGetValue(pawn, out List<ConnectionView> list))
            {
                list = new List<ConnectionView>();
                _connectionsByPawn[pawn] = list;
            }
            list.Add(connection);
        }

        private void DetachFromPawn(ConnectionView connection, PawnView pawn)
        {
            if (pawn == null)
                return;

            if (!_connectionsByPawn.TryGetValue(pawn, out List<ConnectionView> list))
                return;

            list.Remove(connection);
            if (list.Count == 0)
                _connectionsByPawn.Remove(pawn);
        }

        private bool CheckConnectionAvailability(PawnConnectorView start, PawnConnectorView end)
        {
            if (start == null || end == null)
                return false;

            if (start.Owner == end.Owner)
                return false;

            if (start.Owner.IsOutOfBounds || end.Owner.IsOutOfBounds)
                return false;

            return !_lookup.Contains((start, end)) && !_lookup.Contains((end, start));
        }

        #endregion
    }
}