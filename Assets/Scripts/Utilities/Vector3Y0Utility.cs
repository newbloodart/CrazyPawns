using UnityEngine;

namespace CrazyPawn
{
    public static class Vector3Y0Utility
    {
        public static Vector3 ToY0(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

        public static Vector3 Create(float x, float z) => new Vector3(x, 0, z);

    }
}