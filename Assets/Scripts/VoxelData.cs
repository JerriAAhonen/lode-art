using UnityEngine;

public static class VoxelData
{
	public static readonly Vector3Int[] VoxelVerts = new Vector3Int[8]
	{
		new(0, 0, 0),
		new(1, 0, 0),
		new(1, 1, 0),
		new(0, 1, 0),
		new(0, 0, 1),
		new(1, 0, 1),
		new(1, 1, 1),
		new(0, 1, 1)
	};

	// Vertex indexes from VoxelVerts to form 2 triangles needed to draw a square
	public static readonly int[,] VoxelTris = new int[6, 6]
	{
		{ 0, 3, 1, 1, 3, 2 }, // Back
		{ 5, 6, 4, 4, 6, 7 }, // Front
		{ 3, 7, 2, 2, 7, 6 }, // Top 
		{ 1, 5, 0, 0, 5, 4 }, // Bottom 
		{ 4, 7, 0, 0, 7, 3 }, // Left 
		{ 1, 2, 5, 5, 2, 6 }, // Right 
	};

	public static readonly Vector2Int[] VoxelUvs = new Vector2Int[6]
	{
		new(0, 0),
		new(0, 1),
		new(1, 0),
		new(1, 0),
		new(0, 1),
		new(1, 1)
	};
}