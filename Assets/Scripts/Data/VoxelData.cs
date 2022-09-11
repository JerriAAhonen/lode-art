using UnityEngine;

namespace Data
{
	public static class VoxelData
	{
		public enum Face 
		{ 
			Back = 0,
			Front = 1,
			Top = 2,
			Bottom = 3,
			Left = 4, 
			Right = 5 
		}
		
		// Local coordinates for each corner of a voxel
		public static readonly Vector3Int[] Verts =
		{
			new(0, 0, 0),
			new(1, 0, 0),
			new(1, 1, 0),
			new(0, 1, 0),
			new(0, 0, 1),
			new(1, 0, 1),
			new(1, 1, 1),
			new(0, 1, 1),
		};

		// Vertex indexes for VoxelVerts 
		public static readonly int[,] Tris =
		{
			{ 0, 3, 1, 2 }, // Back
			{ 5, 6, 4, 7 }, // Front
			{ 3, 7, 2, 6 }, // Top 
			{ 1, 5, 0, 4 }, // Bottom 
			{ 4, 7, 0, 3 }, // Left 
			{ 1, 2, 5, 6 }, // Right 
		};

		public static readonly Vector2Int[] Uvs =
		{
			new(0, 0),
			new(0, 1),
			new(1, 0),
			new(1, 1),
		};

		public static readonly Vector3Int[] FaceChecks =
		{
			new(0, 0, -1),	// Back
			new(0, 0, 1),		// Front
			new(0, 1, 0),		// Top 
			new(0, -1, 0),	// Bottom 
			new(-1, 0, 0),	// Left 
			new(1, 0, 0),		// Right 
		};
	}
}