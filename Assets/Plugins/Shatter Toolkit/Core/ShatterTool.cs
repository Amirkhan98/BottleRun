// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

namespace ShatterToolkit
{
	[RequireComponent(typeof(MeshFilter))]
	public class ShatterTool : MonoBehaviour
	{
		[Tooltip("If true, then original object will be destroyed.")]
		public bool destroyOriginal = true;

		[HideInInspector]
		public int generation = 1;
		
		[SerializeField]
		[Range(1, 30)]
		[Tooltip("Gets or sets the generation limit of this ShatterTool instance. This value restricts how many times a game object may be shattered using ShatterTool.Shatter(). A game object will only be able to shatter if ShatterTool.Generation is less than ShatterTool.GenerationLimit.")]
		public int generationLimit = 3;
		
		[Range(1, 25)]
		[Tooltip("Gets or sets the number of times the game object will be cut when ShatterTool.Shatter() occurs.")]
		public int cuts = 2;
		
		[Tooltip("Gets or sets whether the cut region should be triangulated. If true, the connected UvMapper component will control the vertex properties of the filled area. When the ShatterTool is used on double-sided meshes with zero thickness, such as planes, this value should be false.")]
		public bool fillCut = true;
		
		[Tooltip("Gets or sets the type of the internal hull used to shatter the mesh. The FastHull implementation is roughly 20-50% faster than the LegacyHull implementation and requires no time to startup. The LegacyHull implementation is more robust in extreme cases and is provided for backwards compatibility. This setting can't be changed during runtime.")]
		public HullType internalHullType = HullType.FastHull;

		public delegate void PreSplitDelegate(ref Plane[] aPlanes);
		public delegate void PostSplitDelegate(ref GameObject[] aNewGameObjects);
		public event PreSplitDelegate EventPreSplit;
		public event PostSplitDelegate EventPostSplit;
		
		protected bool _isIntact = true;
		protected IHull _hull;
		protected Vector3 _center;
		
		/// <summary>
		/// Determines whether this game object is of the first generation. (Generation == 1)
		/// </summary>
		public bool IsFirstGeneration
		{
			get => generation == 1;
		}
		
		/// <summary>
		/// Determines whether this game object is of the last generation. (Generation >= GenerationLimit)
		/// </summary>
		public bool IsLastGeneration
		{
			get => generation >= generationLimit;
		}
		
		/// <summary>
		/// Gets the worldspace center of the game object. Only works during runtime.
		/// </summary>
		public Vector3 Center
		{
			get => transform.TransformPoint(_center);
		}
		
		protected void CalculateCenter()
		{
			// Get the localspace center of the mesh bounds
			_center = GetComponent<MeshFilter>().sharedMesh.bounds.center;
		}
		
		public void Start()
		{
			Mesh sharedMesh = GetComponent<MeshFilter>().sharedMesh;
			
			// Initialize the first generation hull
			if (_hull == null)
			{
				if (internalHullType == HullType.FastHull)
				{
					_hull = new FastHull(sharedMesh);
				}
				else if (internalHullType == HullType.LegacyHull)
				{
					_hull = new LegacyHull(sharedMesh);
				}
			}
			
			// Update properties
			CalculateCenter();
		}
		
		/// <summary>
		/// Shatters the game object at a point, instantiating the pieces as new
		/// game objects (clones of the original) and destroying the original game object when finished.
		/// If the game object has reached the generation limit, nothing will happen.
		/// Apart from taking the generation into account, this is equivalent to calling
		/// ShatterTool.Split() using randomly generated planes passing through the point.
		/// </summary>
		/// <param name="point">
		/// The world-space point.
		/// </param>
		public void Shatter(Vector3 point)
		{
			if (!IsLastGeneration)
			{
				// Increase generation
				generation++;
				
				// Split the hull using randomly generated planes passing through the point
				var planes = new Plane[cuts];
				
				for (int i = 0, n = planes.Length; i < n; i++)
				{
					planes[i] = new Plane(Random.onUnitSphere, point);
				}
				
				Split(ref planes);
			}
		}
		
		/// <summary>
		/// Splits the game object using an array of planes, instantiating the pieces as new
		/// game objects (clones of the original) and destroying the original game object when finished.
		/// </summary>
		/// <param name="planes">
		/// An array of world-space planes with unit-length normals.
		/// </param>
		public void Split(ref Plane[] aPlanes)
		{
			if (aPlanes != null && aPlanes.Length > 0 && _isIntact && _hull != null && !_hull.IsEmpty)
			{
				UvMapper uvMapper = GetComponent<UvMapper>();
				ColorMapper colorMapper = GetComponent<ColorMapper>();
				
				EventPreSplit?.Invoke(ref aPlanes);
				
				Vector3[] points, normals;
				ConvertPlanesToLocalspace(ref aPlanes, out points, out normals);
				
				IList<IHull> newHulls;
				CreateNewHulls(uvMapper, colorMapper, points, normals, out newHulls);
				
				GameObject[] newGameObjects;
				CreateNewGameObjects(newHulls, out newGameObjects);
				
				EventPostSplit?.Invoke(ref newGameObjects);
				
				if (destroyOriginal)
				{
					Destroy(gameObject);
				}
				else
				{
					gameObject.SetActive(false);
				}
				
				_isIntact = false;
			}
		}
		
		protected void ConvertPlanesToLocalspace(ref Plane[] planes, out Vector3[] points, out Vector3[] normals)
		{
			points = new Vector3[planes.Length];
			normals = new Vector3[planes.Length];
			
			for (int i = 0, n = planes.Length; i < n; i++)
			{
				var plane = planes[i];
				
				Vector3 localPoint = transform.InverseTransformPoint(plane.normal * -plane.distance);
				Vector3 localNormal = transform.InverseTransformDirection(plane.normal);
				
				localNormal.Scale(transform.localScale);
				localNormal.Normalize();
				
				points[i] = localPoint;
				normals[i] = localNormal;
			}
		}
		
		protected void CreateNewHulls(UvMapper uvMapper, ColorMapper colorMapper, Vector3[] points, Vector3[] normals, out IList<IHull> newHulls)
		{
			newHulls = new List<IHull>();
			
			// Add the starting hull
			newHulls.Add(_hull);
			
			for (int j = 0, n = points.Length; j < n; j++)
			{
				int previousHullCount = newHulls.Count;
				
				for (int i = 0; i < previousHullCount; i++)
				{
					IHull previousHull = newHulls[0];
					
					// Split the previous hull
					IHull a, b;
					
					previousHull.Split(points[j], normals[j], fillCut, uvMapper, colorMapper, out a, out b);
					
					// Update the list
					newHulls.Remove(previousHull);
					
					if (!a.IsEmpty)
					{
						newHulls.Add(a);
					}
					
					if (!b.IsEmpty)
					{
						newHulls.Add(b);
					}
				}
			}
		}
		
		protected void CreateNewGameObjects(IList<IHull> newHulls, out GameObject[] newGameObjects)
		{
			// Get new meshes
			Mesh[] newMeshes = new Mesh[newHulls.Count];
			float[] newVolumes = new float[newHulls.Count];
			float totalVolume = 0.0f;
			
			for (int i = 0, n = newHulls.Count; i < n; i++)
			{
				Mesh mesh = newHulls[i].GetMesh();
				Vector3 size = mesh.bounds.size;
				float volume = size.x * size.y * size.z;
				
				newMeshes[i] = mesh;
				newVolumes[i] = volume;
				
				totalVolume += volume;
			}

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			MeshCollider meshCollider = GetComponent<MeshCollider>();
			Rigidbody rigidbody = GetComponent<Rigidbody>();

			// Remove mesh references to speed up instantiation
			meshFilter.sharedMesh = null;

			if (meshCollider != null)
			{
				meshCollider.sharedMesh = null;
			}
			
			// Create new game objects
			newGameObjects = new GameObject[newHulls.Count];
			
			for (int i = 0, n = newHulls.Count; i < n; i++)
			{
				IHull newHull = newHulls[i];
				Mesh newMesh = newMeshes[i];
				float volume = newVolumes[i];
				
				GameObject newGameObject = (GameObject)Instantiate(gameObject);
				
				// Set shatter tool
				ShatterTool newShatterTool = newGameObject.GetComponent<ShatterTool>();
				
				if (newShatterTool != null)
				{
					newShatterTool._hull = newHull;
				}
				
				// Set mesh filter
				MeshFilter newMeshFilter = newGameObject.GetComponent<MeshFilter>();
				
				if (newMeshFilter != null)
				{
					newMeshFilter.sharedMesh = newMesh;
				}
				
				// Set mesh collider
				MeshCollider newMeshCollider = newGameObject.GetComponent<MeshCollider>();
				
				if (newMeshCollider != null)
				{
					newMeshCollider.sharedMesh = newMesh;
				}
				
				// Set rigidbody
				Rigidbody newRigidbody = newGameObject.GetComponent<Rigidbody>();
				
				if (rigidbody != null && newRigidbody != null)
				{
					newRigidbody.mass = rigidbody.mass * (volume / totalVolume);
					
					if (!newRigidbody.isKinematic)
					{
						newRigidbody.velocity = rigidbody.GetPointVelocity(newRigidbody.worldCenterOfMass);
						
						newRigidbody.angularVelocity = rigidbody.angularVelocity;
					}
				}
				
				// Update properties
				newShatterTool.CalculateCenter();
				
				newGameObjects[i] = newGameObject;
			}
		}
	}
}