using UnityEngine;

namespace MVsToolkit.Utilities
{
    public static class MVsGizmos
    {
        public static void Draw2DCapsule(Vector2 center, Vector2 size)
        {
            float radius = Mathf.Min(size.x, size.y) * 0.5f;
            float height = Mathf.Max(size.x, size.y);
            float cylinderLength = height - radius * 2f;

            bool vertical = size.y > size.x;

            if (vertical)
            {
                Vector2 top = center + Vector2.up * (cylinderLength * 0.5f);
                Vector2 bottom = center + Vector2.down * (cylinderLength * 0.5f);

                Gizmos.DrawLine(top + Vector2.left * radius, bottom + Vector2.left * radius);
                Gizmos.DrawLine(top + Vector2.right * radius, bottom + Vector2.right * radius);

                DrawCircle(top, radius, Vector3.forward);
                DrawCircle(bottom, radius, Vector3.forward);
            }
            else
            {
                Vector2 right = center + Vector2.right * (cylinderLength * 0.5f);
                Vector2 left = center + Vector2.left * (cylinderLength * 0.5f);

                Gizmos.DrawLine(left + Vector2.up * radius, right + Vector2.up * radius);
                Gizmos.DrawLine(left + Vector2.down * radius, right + Vector2.down * radius);

                DrawCircle(left, radius, Vector3.forward);
                DrawCircle(right, radius, Vector3.forward);
            }
        }

        public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height)
        {
            height = Mathf.Max(height, radius * 2f);

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

            float cylinderHeight = height - 2f * radius;
            float half = cylinderHeight * 0.5f;

            Vector3 up = Vector3.up;
            Vector3 right = Vector3.right;
            Vector3 forward = Vector3.forward;

            Vector3 top = up * half;
            Vector3 bottom = -up * half;

            // --- Cylindre ---
            Gizmos.DrawLine(top + right * radius, bottom + right * radius);
            Gizmos.DrawLine(top - right * radius, bottom - right * radius);
            Gizmos.DrawLine(top + forward * radius, bottom + forward * radius);
            Gizmos.DrawLine(top - forward * radius, bottom - forward * radius);

            // --- Cercles des extrémités ---
            DrawCircle(top, radius, up);
            DrawCircle(bottom, radius, -up);

            // --- Arcs hémisphères (vraie capsule) ---
            DrawCapsuleArc(top, radius, true);
            DrawCapsuleArc(bottom, radius, false);

            Gizmos.matrix = oldMatrix;
        }
        public static void DrawCircle(Vector3 center, float radius, Vector3 normal, int segments = 32)
        {
            if (normal == Vector3.zero)
                normal = Vector3.up;

            normal.Normalize();

            // Axes du plan
            Vector3 tangent = Vector3.Cross(normal, Vector3.right);
            if (tangent == Vector3.zero)
                tangent = Vector3.Cross(normal, Vector3.up);

            tangent.Normalize();
            Vector3 bitangent = Vector3.Cross(normal, tangent);

            float step = Mathf.PI * 2f / segments;
            Vector3 prev = center + tangent * radius;

            for (int i = 1; i <= segments; i++)
            {
                float angle = step * i;
                Vector3 next =
                    center +
                    (tangent * Mathf.Cos(angle) + bitangent * Mathf.Sin(angle)) * radius;

                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        public static void DrawHemisphere(Vector3 center, float radius, bool top)
        {
            const int segments = 16;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float phi = t * Mathf.PI * 0.5f;

                float y = Mathf.Sin(phi) * radius;
                float r = Mathf.Cos(phi) * radius;

                if (!top) y = -y;

                DrawCircle(center + Vector3.up * y, r, Vector3.up);
            }
        }
        static void DrawCapsuleArc(Vector3 center, float radius, bool top)
        {
            const int segments = 16;

            Vector3 up = Vector3.up;
            Vector3 right = Vector3.right;
            Vector3 forward = Vector3.forward;

            float sign = top ? 1f : -1f;

            Vector3 prevR = center + right * radius;
            Vector3 prevF = center + forward * radius;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                float angle = t * Mathf.PI * 0.5f;

                float y = Mathf.Sin(angle) * radius * sign;
                float r = Mathf.Cos(angle) * radius;

                Vector3 nextR = center + up * y + right * r;
                Vector3 nextF = center + up * y + forward * r;

                Gizmos.DrawLine(prevR, nextR);
                Gizmos.DrawLine(prevF, nextF);

                prevR = nextR;
                prevF = nextF;
            }
        }
    }
}