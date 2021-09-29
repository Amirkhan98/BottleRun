// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

namespace ShatterToolkit
{
    public class WorldUvMapper : UvMapper
    {
        /// <summary>
        /// Determines the scale to be applied to the uv-coordinates. (1,1) will repeat the texture once for every local-space unit, (2,2) twice and so on.
        /// </summary>
        public Vector2 scale = Vector2.one;
        
        public override void Map(IList<Vector3> points, Vector3 planeNormal, out Vector4[] tangentsA, out Vector4[] tangentsB, out Vector2[] uvsA, out Vector2[] uvsB)
        {
            // Calculate texture direction vectors
            var u = Vector3.Cross(planeNormal, Vector3.up);
            
            if (u == Vector3.zero)
            {
                u = Vector3.Cross(planeNormal, Vector3.forward);
            }
            
            var v = Vector3.Cross(u, planeNormal);
            
            u.Normalize();
            v.Normalize();
            
            // Set tangents
            var tangentA = new Vector4(u.x, u.y, u.z, 1.0f);
            var tangentB = new Vector4(u.x, u.y, u.z, -1.0f);
            
            tangentsA = new Vector4[points.Count];
            tangentsB = new Vector4[points.Count];
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                tangentsA[i] = tangentA;
                tangentsB[i] = tangentB;
            }
            
            // Set uvs
            var uvs = new Vector2[points.Count];
            
            var min = Vector2.zero;
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                var point = points[i];
                
                uvs[i].x = Vector3.Dot(point, u);
                uvs[i].y = Vector3.Dot(point, v);
                
                min = (i == 0)
                    ? uvs[i]
                    : Vector2.Min(uvs[i], min);
            }
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                uvs[i] -= min;
                
                uvs[i].x *= scale.x;
                uvs[i].y *= scale.y;
            }
            
            uvsA = uvs;
            uvsB = uvs;
        }
    }
}