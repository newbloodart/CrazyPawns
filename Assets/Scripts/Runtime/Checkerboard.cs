using UnityEngine;

namespace CrazyPawn
{
    public class Checkerboard : MonoBehaviour
    {
        public CrazyPawnSettings Settings => _settings;
        public CrazyPawnSettingsExtended SettingsExtended => _settingsExtended;
        public float Size => _size;
        public float HalfSize => _halfSize;

        [SerializeField] private CrazyPawnSettings _settings;
        [SerializeField] private CrazyPawnSettingsExtended _settingsExtended;
        [SerializeField] private Shader _checkerboardShader;

        private float _size = 0f;
        private float _halfSize = 0f;
        private Material _material;

        private void Awake()
        {
            _size = _settingsExtended.CheckerboardCellSize * _settings.CheckerboardSize;
            _halfSize = _size / 2f;

            if (_settingsExtended.CheckerboardVisible)
                GenerateVisual();
        }

        private void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }

        public bool IsPointOutOfBound(Vector3 point) =>
            Mathf.Abs(point.x) > _halfSize || Mathf.Abs(point.z) > _halfSize;

        private void GenerateVisual()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            vertices[0] = Vector3Y0Utility.Create(-_halfSize, -_halfSize);
            vertices[1] = Vector3Y0Utility.Create(_halfSize, -_halfSize);
            vertices[2] = Vector3Y0Utility.Create(-_halfSize, _halfSize);
            vertices[3] = Vector3Y0Utility.Create(_halfSize, _halfSize);

            int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            Vector3[] normals = new Vector3[4];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = Vector3.up;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.normals = normals;
            mesh.name = "Checkerboard";
            mesh.RecalculateBounds();

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            _material = new Material(_checkerboardShader);
            _material.SetColor("_WhiteCellColor", _settings.WhiteCellColor);
            _material.SetColor("_BlackCellColor", _settings.BlackCellColor);
            _material.SetFloat("_Size", _settings.CheckerboardSize);

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _material;
        }
    }
}