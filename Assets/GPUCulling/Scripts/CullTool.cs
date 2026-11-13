using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CullTool 
{
    // 用法线和面上一点构造平面方程 (A, B, C, D)
    public static Vector4 GetPlane(Vector3 normal, Vector3 point)
    {
        return new Vector4(
            normal.x, normal.y, normal.z,
            -Vector3.Dot(normal, point));
    }
    
    // 用三个点构造平面
    public static Vector4 GetPlane(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 normal = Vector3.Normalize(
            Vector3.Cross(b - a, c - a));

        return GetPlane(normal, a);
    }
    
    
    // 计算远裁剪平面的四个角
    public static Vector3[] GetCameraFarClipPlanePoint(Camera camera)
    {
        Vector3[] points = new Vector3[4];
        Transform transform = camera.transform;

        float distance = camera.farClipPlane;
        float halfFovRad =
            Mathf.Deg2Rad * camera.fieldOfView * 0.5f;

        float upLen = distance * Mathf.Tan(halfFovRad);
        float rightLen = upLen * camera.aspect;

        Vector3 farCenterPoint =
            transform.position + distance * transform.forward;
        Vector3 up = upLen * transform.up;
        Vector3 right = rightLen * transform.right;

        points[0] = farCenterPoint - up - right; // 左下
        points[1] = farCenterPoint - up + right; // 右下
        points[2] = farCenterPoint + up - right; // 左上
        points[3] = farCenterPoint + up + right; // 右上

        return points;
    }
    
    // 计算视锥体的六个平面
    public static Vector4[] GetFrustumPlane(Camera camera)
    {
        Vector4[] planes = new Vector4[6];
        Transform transform = camera.transform;
        Vector3 cameraPosition = transform.position;
        Vector3[] points = GetCameraFarClipPlanePoint(camera);

        planes[0] = GetPlane(cameraPosition, points[0], points[2]); // 左
        planes[1] = GetPlane(cameraPosition, points[3], points[1]); // 右
        planes[2] = GetPlane(cameraPosition, points[1], points[0]); // 下
        planes[3] = GetPlane(cameraPosition, points[2], points[3]); // 上
        planes[4] = GetPlane(
            -transform.forward,
            transform.position + transform.forward * camera.nearClipPlane); // 近
        planes[5] = GetPlane(
            transform.forward,
            transform.position + transform.forward * camera.farClipPlane); // 远

        return planes;
    }
    
    
    
    
}
