// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

namespace ShatterToolkit
{
    /// <summary>
    /// Advanced triangulator capable of filling concave polygons with holes and recursive geometry.
    /// An algorithm based on ear-clipping, developed by Gustav Olsson, see http://gustavolsson.squarespace.com/ for more information.
    /// </summary>
    public class Triangulator : ITriangulator
    {
        // Geometry
        protected List<Vector3> points;
        protected List<int> edges;
        protected List<int> triangles;
        protected List<int> triangleEdges;
        
        protected List<List<int>> loops;
        protected List<List<bool>> concavities;
        protected List<int> duplicateEdges;
        
        // Properties
        protected Vector3 planeNormal;
        protected int originalEdgeCount;
        
        public Triangulator(IList<Vector3> points, IList<int> edges, Vector3 planeNormal)
        {
            this.points = new List<Vector3>(points);
            this.edges = new List<int>(edges);
            this.triangles = new List<int>();
            this.triangleEdges = new List<int>();
            
            this.planeNormal = planeNormal;
            this.originalEdgeCount = this.edges.Count;
        }
        
        public void Fill(out int[] newEdges, out int[] newTriangles, out int[] newTriangleEdges)
        {
            // Prepare triangulation
            FindLoops();
            
            FindConcavities();
            
            PrepareDuplicateEdges();
            
            // Triangulate loops
            for (int i = 0; i < loops.Count; i++)
            {
                List<int> loop = loops[i];
                List<bool> concavity = concavities[i];
                
                // Triangulate loop
                int index = 0;
                int unsuitableTriangles = 0;
                
                while (loop.Count >= 3)
                {
                    // Evaluate triangle
                    int zero = index == 0 ? loop.Count - 1 : index - 1;
                    int first = index;
                    int second = (index + 1) % loop.Count;
                    int third = (index + 2) % loop.Count;
                    
                    if (concavity[first] || IsTriangleOverlappingLoop(first, second, third, loop, concavity))
                    {
                        // This triangle is not an ear, examine the next one
                        index++;
                        unsuitableTriangles++;
                    }
                    else
                    {
                        // Evaluate loop merge
                        int swallowedLoopIndex;
                        
                        if (MergeLoops(first, second, third, loop, concavity, out swallowedLoopIndex))
                        {
                            // Merge occured, adjust loop index
                            if (swallowedLoopIndex < i)
                            {
                                i--;
                            }
                            
                            //ValidateConcavities();
                        }
                        else
                        {
                            // No merge occured, fill triangle
                            FillTriangle(zero, first, second, third, loop, concavity);
                        }
                        
                        // Suitable triangles may have appeared
                        unsuitableTriangles = 0;
                    }
                    
                    if (unsuitableTriangles >= loop.Count)
                    {
                        // No more suitable triangles in this loop, continue with the next one
                        break;
                    }
                    
                    // Wrap index
                    if (index >= loop.Count)
                    {
                        index = 0;
                        unsuitableTriangles = 0;
                    }
                }
                
                // Is the loop filled?
                if (loop.Count <= 2)
                {
                    // Remove the loop in order to avoid future merges
                    loops.RemoveAt(i);
                    concavities.RemoveAt(i);
                    
                    i--;
                }
            }
            
            // Fill any remaining loops using triangle fans for robustness
            for (int i = 0, n = loops.Count; i < n; i++)
            {
                List<int> loop = loops[i];
                List<bool> concavity = concavities[i];
                
                while (loop.Count >= 3)
                {
                    FillTriangle(0, 1, 2, 3 % loop.Count, loop, concavity);
                }
            }
            
            // Finish triangulation
            RemoveDuplicateEdges();
            
            SetOutput(out newEdges, out newTriangles, out newTriangleEdges);
        }
        
        protected void FindLoops()
        {
            loops = new List<List<int>>();
            
            List<int> loop = new List<int>(edges.Count / 2);
            
            for (int i = 0; i < edges.Count / 2; i++)
            {
                int edge = i * 2;
                
                int startPoint = edges[edge + 0];
                int endPoint = edges[edge + 1];
                
                // Make sure that the current edge is connected with the previous one
                if (loop.Count >= 1)
                {
                    int previousEndPoint = edges[edge - 1];
                    
                    if (startPoint != previousEndPoint)
                    {
                        Debug.LogError("The edges do not form an edge loop!");
                    }
                }
                
                // Add the edge index to the loop
                loop.Add(edge);
                
                // Does the current edge end the loop?
                if (endPoint == edges[loop[0]])
                {
                    loops.Add(loop);
                    
                    loop = new List<int>();
                }
            }
        }
        
        protected void FindConcavities()
        {
            concavities = new List<List<bool>>();
            
            foreach (List<int> loop in loops)
            {
                List<bool> concavity = new List<bool>(loop.Count);
                
                for (int i = 0; i < loop.Count; i++)
                {
                    int point0 = edges[loop[i]];
                    int point1 = edges[loop[(i + 1) % loop.Count]];
                    int point2 = edges[loop[(i + 2) % loop.Count]];
                    
                    Vector3 firstLine = points[point1] - points[point0];
                    Vector3 secondLine = points[point2] - points[point1];
                    
                    concavity.Add(IsLinePairConcave(ref firstLine, ref secondLine));
                }
                
                concavities.Add(concavity);
            }
        }
        
        protected void PrepareDuplicateEdges()
        {
            duplicateEdges = new List<int>();
        }
        
        protected void ValidateConcavities()
        {
            for (int j = 0, nj = loops.Count; j < nj; j++)
            {
                IList<int> loop = loops[j];
                IList<bool> concavity = concavities[j];
                
                for (int i = 0, n = loop.Count; i < n; i++)
                {
                    int point0 = edges[loop[i]];
                    int point1 = edges[loop[(i + 1) % loop.Count]];
                    int point2 = edges[loop[(i + 2) % loop.Count]];
                    
                    Vector3 firstLine = points[point1] - points[point0];
                    Vector3 secondLine = points[point2] - points[point1];
                    
                    if (concavity[i] != IsLinePairConcave(ref firstLine, ref secondLine))
                    {
                        Debug.LogError("Concavity not valid!");
                    }
                }
            }
        }
        
        protected void UpdateConcavity(int index, List<int> loop, List<bool> concavity)
        {
            int firstEdge = loop[index];
            int secondEdge = loop[(index + 1) % loop.Count];
            
            Vector3 firstLine = points[edges[firstEdge + 1]] - points[edges[firstEdge]];
            Vector3 secondLine = points[edges[secondEdge + 1]] - points[edges[secondEdge]];
            
            concavity[index] = IsLinePairConcave(ref firstLine, ref secondLine);
        }
        
        protected bool IsLinePairConcave(ref Vector3 line0, ref Vector3 line1)
        {
            Vector3 lineNormal0 = Vector3.Cross(line0, planeNormal);
            
            // Zero is not considered concave in order to support zero-length lines
            return Vector3.Dot(line1, lineNormal0) > 0.0f;
        }
        
        protected bool IsTriangleOverlappingLoop(int first, int second, int third, List<int> loop, List<bool> concavity)
        {
            int point0 = edges[loop[first]];
            int point1 = edges[loop[second]];
            int point2 = edges[loop[third]];
            
            Vector3 triangle0 = points[point0];
            Vector3 triangle1 = points[point1];
            Vector3 triangle2 = points[point2];
            
            for (int i = 0, n = loop.Count; i < n; i++)
            {
                if (concavity[i])
                {
                    int reflexPoint = edges[loop[i] + 1];
                    
                    // Do not test the reflex point if it is part of the triangle
                    if (reflexPoint != point0 && reflexPoint != point1 && reflexPoint != point2)
                    {
                        Vector3 point = points[reflexPoint];
                        
                        // Does the reflex point lie inside the triangle?
                        if (Tools.IsPointInsideTriangle(ref point, ref triangle0, ref triangle1, ref triangle2, ref planeNormal))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        protected bool MergeLoops(int first, int second, int third, List<int> loop, List<bool> concavity, out int swallowedLoopIndex)
        {
            int otherLoopIndex, otherLoopLocation;
            
            if (FindClosestPointInTriangle(first, second, third, loop, out otherLoopIndex, out otherLoopLocation))
            {
                // Swallow the other loop
                InsertLoop(first, loop, concavity, otherLoopLocation, loops[otherLoopIndex], concavities[otherLoopIndex]);
                
                // Remove the now obsolete other loop
                loops.RemoveAt(otherLoopIndex);
                concavities.RemoveAt(otherLoopIndex);
                
                swallowedLoopIndex = otherLoopIndex;
                
                return true;
            }
            
            swallowedLoopIndex = -1;
            
            return false;
        }
        
        protected bool FindClosestPointInTriangle(int first, int second, int third, List<int> loop, out int loopIndex, out int loopLocation)
        {
            Vector3 triangle0 = points[edges[loop[first]]];
            Vector3 triangle1 = points[edges[loop[second]]];
            Vector3 triangle2 = points[edges[loop[third]]];
            
            Vector3 firstNormal = Vector3.Cross(planeNormal, triangle1 - triangle0);
            
            int closestLoopIndex = -1;
            int closestLoopLocation = 0;
            float closestDistance = 0.0f;
            
            for (int i = 0, n = loops.Count; i < n; i++)
            {
                IList<int> otherLoop = loops[i];
                IList<bool> otherConcavity = concavities[i];
                
                if (otherLoop != loop)
                {
                    for (int j = 0, nj = otherLoop.Count; j < nj; j++)
                    {
                        if (otherConcavity[j])
                        {
                            Vector3 point = points[edges[otherLoop[j] + 1]];
                            
                            if (Tools.IsPointInsideTriangle(ref point, ref triangle0, ref triangle1, ref triangle2, ref planeNormal))
                            {
                                // Calculate the distance from the bottom of the triangle to the point
                                float distance = Vector3.Dot(point - triangle0, firstNormal);
                                
                                if (distance < closestDistance || closestLoopIndex == -1)
                                {
                                    closestLoopIndex = i;
                                    closestLoopLocation = (j + 1) % otherLoop.Count;
                                    closestDistance = distance;
                                }
                            }
                        }
                    }
                }
            }
            
            loopIndex = closestLoopIndex;
            loopLocation = closestLoopLocation;
            
            return closestLoopIndex != -1;
        }
        
        protected void InsertLoop(int insertLocation, List<int> loop, List<bool> concavity, int otherAnchorLocation, List<int> otherLoop, List<bool> otherConcavity)
        {
            int insertPoint = edges[loop[insertLocation]];
            int anchorPoint = edges[otherLoop[otherAnchorLocation]];
            
            // Create bridge edges
            int bridgeFromEdge = edges.Count;
            
            edges.Add(anchorPoint);
            edges.Add(insertPoint);
            
            int bridgeToEdge = edges.Count;
            
            edges.Add(insertPoint);
            edges.Add(anchorPoint);
            
            // Save the last added duplicate edge
            duplicateEdges.Add(bridgeToEdge);
            
            // Insert the other loop into this loop
            int[] insertLoop = new int[otherLoop.Count + 2];
            bool[] insertConcavity = new bool[otherConcavity.Count + 2];
            
            int index = 0;
            
            insertLoop[index] = bridgeToEdge;
            insertConcavity[index++] = false;
            
            for (int i = otherAnchorLocation, n = otherLoop.Count; i < n; i++)
            {
                insertLoop[index] = otherLoop[i];
                insertConcavity[index++] = otherConcavity[i];
            }
            
            for (int i = 0; i < otherAnchorLocation; i++)
            {
                insertLoop[index] = otherLoop[i];
                insertConcavity[index++] = otherConcavity[i];
            }
            
            insertLoop[index] = bridgeFromEdge;
            insertConcavity[index] = false;
            
            loop.InsertRange(insertLocation, insertLoop);
            concavity.InsertRange(insertLocation, insertConcavity);
            
            // Update concavity
            int previousLocation = insertLocation == 0 ? loop.Count - 1 : insertLocation - 1;
            
            UpdateConcavity(previousLocation, loop, concavity);
            
            UpdateConcavity(insertLocation, loop, concavity);
            
            UpdateConcavity(insertLocation + otherLoop.Count, loop, concavity);
            
            UpdateConcavity(insertLocation + otherLoop.Count + 1, loop, concavity);
        }
        
        protected void FillTriangle(int zero, int first, int second, int third, List<int> loop, List<bool> concavity)
        {
            // Find triangle features
            int zeroEdge = loop[zero];
            int firstEdge = loop[first];
            int secondEdge = loop[second];
            int thirdEdge = loop[third];
            
            int firstPoint = edges[firstEdge];
            int secondPoint = edges[secondEdge];
            int thirdPoint = edges[thirdEdge];
            
            int crossEdge;
            
            if (loop.Count != 3)
            {
                // Create the cross edge
                crossEdge = edges.Count;
                
                edges.Add(firstPoint);
                edges.Add(thirdPoint);
            }
            else
            {
                // Use the third edge as the cross edge
                crossEdge = thirdEdge;
            }
            
            // Add new triangle
            triangles.Add(firstPoint);
            triangles.Add(secondPoint);
            triangles.Add(thirdPoint);
            
            triangleEdges.Add(firstEdge);
            triangleEdges.Add(secondEdge);
            triangleEdges.Add(crossEdge);
            
            // Update loop
            loop[second] = crossEdge;
            loop.RemoveAt(first);
            
            // Update concavity; always update in order to support zero-length edges
            Vector3 zeroLine = points[firstPoint] - points[edges[zeroEdge]];
            Vector3 crossLine = points[thirdPoint] - points[firstPoint];
            Vector3 thirdLine = points[edges[thirdEdge + 1]] - points[thirdPoint];
            
            concavity[zero] = IsLinePairConcave(ref zeroLine, ref crossLine);
            
            concavity[second] = IsLinePairConcave(ref crossLine, ref thirdLine);
            concavity.RemoveAt(first);
        }
        
        protected void RemoveDuplicateEdges()
        {
            for (int i = 0, n = duplicateEdges.Count; i < n; i++)
            {
                int edge = duplicateEdges[i];
                
                // Remove the duplicate edge
                edges.RemoveRange(edge, 2);
                
                // Update indices in triangle edges
                for (int j = 0, nj = triangleEdges.Count; j < nj; j++)
                {
                    if (triangleEdges[j] >= edge)
                    {
                        // The corresponding edge lie in front of the duplicate edge
                        triangleEdges[j] -= 2;
                    }
                }
                
                // Update indices in duplicate edges
                for (int j = i + 1, nj = duplicateEdges.Count; j < nj; j++)
                {
                    if (duplicateEdges[j] >= edge)
                    {
                        // The corresponding edge lie in front of the duplicate edge
                        duplicateEdges[j] -= 2;
                    }
                }
            }
        }
        
        protected void SetOutput(out int[] newEdges, out int[] newTriangles, out int[] newTriangleEdges)
        {
            // Set edges
            int newEdgeCount = edges.Count - originalEdgeCount;
            
            if (newEdgeCount > 0)
            {
                newEdges = new int[newEdgeCount];
                
                edges.CopyTo(originalEdgeCount, newEdges, 0, newEdgeCount);
            }
            else
            {
                newEdges = new int[0];
            }
            
            // Set triangles
            newTriangles = triangles.ToArray();
            
            // Set triangle edges
            newTriangleEdges = new int[triangleEdges.Count];
            
            for (int i = 0, n = triangleEdges.Count; i < n; i++)
            {
                newTriangleEdges[i] = triangleEdges[i] / 2;
            }
        }
    }
}