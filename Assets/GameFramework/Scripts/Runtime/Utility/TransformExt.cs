using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public static class TransformExt
    {
        public static Transform Find2(this Transform transform, string name)
        {
            if (string.IsNullOrEmpty(name))
                return transform;
            for (int index = 0; index < transform.childCount; ++index)
            {
                Transform child = transform.GetChild(index);
                if (child.name == name)
                    return child;
                if (child.childCount > 0)
                {
                    Transform transform1 = child.Find2(name);
                    if ((Object)transform1 != (Object)null)
                        return transform1;
                }
            }

            return (Transform)null;
        }

        public static void LookAt2(this Transform transform, Transform target, Vector3 worldUp)
        {
            if (!((Object)target != (Object)null) || !((Object)target != (Object)transform))
                return;
            transform.LookAt2(target.position, worldUp);
        }

        public static void LookAt2(this Transform transform, Transform target)
        {
            if (!((Object)target != (Object)null) || !((Object)target != (Object)transform))
                return;
            transform.LookAt2(target.position, Vector3.up);
        }

        public static void LookAt2(this Transform transform, Vector3 position, Vector3 worldUp)
        {
            if ((Object)transform == (Object)null || transform.position == position)
                return;
            Vector3 toDirection = position - transform.position;
            if (toDirection.normalized == Vector3.back)
                transform.rotation = new Quaternion(0.0f, 1f, 0.0f, 0.0f);
            else
                transform.rotation = Quaternion.FromToRotation(Vector3.forward, toDirection);
        }

        public static void LookAt2(this Transform transform, Vector3 position) =>
            transform.LookAt2(position, Vector3.up);

        public static void LookAtXZ(this Transform transform, Transform target)
        {
            if (!((Object)target != (Object)null) || !((Object)target != (Object)transform))
                return;
            transform.LookAtXZ(target.position);
        }

        public static void LookAtXZ(this Transform transform, Vector3 position)
        {
            position.y = transform.position.y;
            transform.LookAt2(position);
        }

      
        public static Vector3 TransformPoint2(this Transform transform, Vector3 position)
        {
            for (; (Object)transform != (Object)null; transform = transform.parent)
            {
                position.Scale(transform.localScale);
                position = transform.localRotation * position;
                position += transform.localPosition;
            }

            return position;
        }

        public static Vector3 TransformPoint2(this Transform transform, int x, int y, int z) =>
            transform.TransformPoint2(new Vector3((float)x, (float)y, (float)z));

        public static void Translate2(this Transform transform, Vector3 translation) =>
            transform.position += transform.rotation * translation;

        public static void Translate2(this Transform transform, Vector3 translation, Space relativeTo)
        {
            if (relativeTo == Space.World)
                transform.position += translation;
            else
                transform.position += transform.rotation * translation;
        }

        public static void Translate2(this Transform transform, float x, float y, float z) =>
            transform.Translate2(new Vector3(x, y, z));

        public static void Translate2(
            this Transform transform,
            float x,
            float y,
            float z,
            Space relativeTo)
        {
            transform.Translate2(new Vector3(x, y, z), relativeTo);
        }

        public static void Translate(this Transform transform, Vector3 translation, float distance)
        {
            translation *= distance;
            transform.Translate2(translation);
        }

        public static void Translate(
            this Transform transform,
            Vector3 translation,
            float distance,
            Space relativeTo)
        {
            translation *= distance;
            transform.Translate2(translation, relativeTo);
        }
    }
}