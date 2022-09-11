using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	
	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	private void Start()
	{
		var vertexIndex = 0;
		var vertices = new Vector3[36];
		var triangles = new int[36];
		var uvs = new Vector2[36];
		
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				var triangleIndex = VoxelData.VoxelTris[i, j];
				vertices[vertexIndex] = VoxelData.VoxelVerts[triangleIndex];
				triangles[vertexIndex] = vertexIndex;
				uvs[vertexIndex] = VoxelData.VoxelUvs[j];

				vertexIndex++;
			}
		}

		var mesh = new Mesh
		{
			vertices = vertices,
			triangles = triangles,
			uv = uvs
		};

		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
	}
}