using UnityEngine;

namespace MVsToolkit.Utilities
{
    public static class MVsRotation
    {
        public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            if (Time.deltaTime == 0) return current;
            if (smoothTime == 0) return target;

            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static void LookAtSmoothDamp(this Transform transform, Vector3 targetPosition, ref Quaternion velocity, float smoothTime)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // SmoothDamp for quaternions (critically damped)
            transform.rotation = SmoothDampQuaternion(transform.rotation, targetRotation, ref velocity, smoothTime);
        }

        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Quaternion deriv, float smoothTime)
        {
            // Ensure shortest path
            if (Quaternion.Dot(current, target) < 0f)
            {
                target.x = -target.x;
                target.y = -target.y;
                target.z = -target.z;
                target.w = -target.w;
            }

            // Smooth damp each component
            Vector4 result = new Vector4(
                Mathf.SmoothDamp(current.x, target.x, ref deriv.x, smoothTime),
                Mathf.SmoothDamp(current.y, target.y, ref deriv.y, smoothTime),
                Mathf.SmoothDamp(current.z, target.z, ref deriv.z, smoothTime),
                Mathf.SmoothDamp(current.w, target.w, ref deriv.w, smoothTime)
            );

            // Normalize to avoid Unity assertion
            return NormalizeQuaternion(result);
        }

        private static Quaternion NormalizeQuaternion(Vector4 q)
        {
            float mag = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (mag < 1e-6f)
                return Quaternion.identity;

            return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

    }
}