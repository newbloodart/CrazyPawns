using UnityEngine;
using System.Collections.Generic;

namespace CrazyPawn
{
    public class PawnsManager : MonoBehaviour
    {
        public CrazyPawnSettings Settings => _settings;
        public CrazyPawnSettingsExtended SettingsExtended => _settingsExtended;
        public IReadOnlyList<PawnView> Pawns => _pawns;

        [SerializeField] private CrazyPawnSettings _settings;
        [SerializeField] private CrazyPawnSettingsExtended _settingsExtended;

        [Space]
        [SerializeField] private GameObject _pawnPrefab;
        [SerializeField] private Checkerboard _board;
        [SerializeField] private WorldPlaneView _worldPlane;
        [SerializeField] private ConnectionsManager _connectionsManager;

        private readonly List<PawnView> _pawns = new();
        private Vector3 _dragOffset;

        private void Start() => SpawnPawns();

        private void AddPawn(PawnView pawn)
        {
            pawn.Initialize(this);
            pawn.OnPointerDown += Pawn_OnPointerDown;
            pawn.OnPointerDrag += Pawn_OnPointerDrag;
            pawn.OnPointerUp += Pawn_OnPointerUp;
            _connectionsManager.RegisterConnectors(pawn);
            _pawns.Add(pawn);
        }

        private void RemovePawn(PawnView pawn)
        {
            _pawns.Remove(pawn);
            _connectionsManager.UnregisterConnectors(pawn);
            pawn.OnPointerUp -= Pawn_OnPointerUp;
            pawn.OnPointerDrag -= Pawn_OnPointerDrag;
            pawn.OnPointerDown -= Pawn_OnPointerDown;     
            Destroy(pawn.gameObject);
        }

        private void SpawnPawns()
        {
            for (int i = 0; i < _settings.InitialPawnCount; i++)
            {
                Vector2 randPos = Random.insideUnitCircle * _settings.InitialZoneRadius;
                Vector3 spawnPos = Vector3Y0Utility.Create(randPos.x, randPos.y);
                PawnView pawn = Instantiate(
                    _pawnPrefab,
                    spawnPos,
                    Quaternion.identity,
                    transform
                ).GetComponent<PawnView>();
                AddPawn(pawn);
                pawn.SetOutOfBounds(_board.IsPointOutOfBound(spawnPos));
            }
        }

        #region Pawns interaction

        private void Pawn_OnPointerDown(PawnView pawn)
        {
            _connectionsManager.CancelPendingConnection();
            _dragOffset = _worldPlane.TryGetPointOnPlaneFromMousePosition(out Vector3 point) 
                ? pawn.transform.position - point 
                : Vector3.zero;
        }

        private void Pawn_OnPointerDrag(PawnView pawn)
        {
            if (_worldPlane.TryGetPointOnPlaneFromMousePosition(out Vector3 point))
            {
                pawn.MoveTo(point + _dragOffset);
                _connectionsManager.UpdateConnectionsVisuals(pawn);
                pawn.SetOutOfBounds(_board.IsPointOutOfBound(pawn.GetCenter()));
            }
        }

        private void Pawn_OnPointerUp(PawnView pawn)
        {
            if (_board.IsPointOutOfBound(pawn.GetCenter()))
                RemovePawn(pawn);
        }

        #endregion
    }
}