using UnityEngine;


namespace Game.Environment.LMixTable
{
    public static class ScreenPositionInWorldPosition
    {
        public static Vector3 GetWorldPositionOnPlaneYZ(Vector3 screenPosition, float x)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane yz = new Plane(Vector3.right, new Vector3(x, 0, 0));
            float distance;
            yz.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        public static Vector3 GetWorldPositionOnPlaneXZ(Vector3 screenPosition, float y)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane xz = new Plane(Vector3.up, new Vector3(0, y, 0));
            float distance;
            xz.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        public static Vector3 GetWorldPositionOnPlaneXY(Vector3 screenPosition, float z)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, z, 0));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    }
}
