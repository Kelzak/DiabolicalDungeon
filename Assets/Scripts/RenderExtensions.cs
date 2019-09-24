using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: http://wiki.unity3d.com/index.php/IsVisibleFrom?_ga=2.242159618.1212712638.1569354473-236799797.1540929820#C.23_-_RendererExtensions.cs
public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}