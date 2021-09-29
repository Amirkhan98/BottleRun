// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

namespace ShatterToolkit
{
    public class TargetUvMapper : UvMapper
    {
        public Material material;
        
        [HideInInspector]
        public Rect uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        /// <summary>
        /// Determines the start of the target area in the texture, in uv-coordinates.
        /// </summary>
        public Vector2 targetStart = Vector2.zero;
        
        /// <summary>
        /// Determines the size of the target area in the texture, in uv-coordinates.
        /// </summary>
        public Vector2 targetSize = Vector2.one;
        
        /// <summary>
        /// If true, the aspect ratio of the vertex uv-coordinates, before being mapped to the target area, is locked to 1.
        /// Useful if the desired target area is square and the final texture should keep it's proportions.
        /// </summary>
        public bool square = false;
        
        /// <summary>
        /// If true, the mesh origo (0,0,0) will be the center of the vertex uv-coordinates, before being mapped to the target area.
        /// Useful when for example splitting a watermelon multiple times and a coherent mapping is desired.
        /// </summary>
        public bool centerMeshOrigo = false;
        
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
            var max = Vector2.zero;
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                var point = points[i];
                
                uvs[i].x = Vector3.Dot(point, u);
                uvs[i].y = Vector3.Dot(point, v);
                
                if (i == 0)
                {
                    min = uvs[i];
                    max = uvs[i];
                }
                else
                {
                    min = Vector2.Min(uvs[i], min);
                    max = Vector2.Max(uvs[i], max);
                }
            }
            
            Vector2 originalSize = max - min;
            
            if (square)
            {
                float largestSide = Mathf.Max(originalSize.x, originalSize.y);
                
                var offset = Vector2.zero;
                
                offset.x = (largestSide - originalSize.x) * 0.5f;
                offset.y = (largestSide - originalSize.y) * 0.5f;
                
                min -= offset;
                max += offset;
            }
            
            if (centerMeshOrigo)
            {
                var largestExtent = Vector2.zero;
                
                largestExtent.x = Mathf.Max(Mathf.Abs(min.x), Mathf.Abs(max.x));
                largestExtent.y = Mathf.Max(Mathf.Abs(min.y), Mathf.Abs(max.y));
                
                min = -largestExtent;
                max = largestExtent;
            }
            
            var size = max - min;
            var invSize = new Vector2(1.0f / size.x, 1.0f / size.y);
            
            for (int i = 0, n = points.Count; i < n; i++)
            {
                // Convert uvs to the range [0, 1]
                uvs[i].x = (uvs[i].x - min.x) * invSize.x;
                uvs[i].y = (uvs[i].y - min.y) * invSize.y;
                
                // Convert uvs to the range [targetStart, targetStart + targetSize]
                uvs[i].x = targetStart.x + targetSize.x * uvs[i].x;
                uvs[i].y = targetStart.y + targetSize.y * uvs[i].y;
            }
            
            uvsA = uvs;
            uvsB = uvs;
        }
    }
}