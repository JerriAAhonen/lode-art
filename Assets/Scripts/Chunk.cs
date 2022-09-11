using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	private static int totalVertices => 6 * 6 * ChunkData.ChunkHeight * ChunkData.ChunkWidth * ChunkData.ChunkWidth;
	private readonly Vector3[] vertices = new Vector3[totalVertices];
	private readonly int[] triangles = new int[totalVertices];
	private readonly Vector2[] uvs = new Vector2[totalVertices];

	private bool[,,] voxelMap = new bool[ChunkData.ChunkWidth, ChunkData.ChunkHeight, ChunkData.ChunkWidth];

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();
	}

	private bool CheckVoxel(Vector3Int pos)
	{
		int x = pos.x;
		int y = pos.y;
		int z = pos.z;

		if (x < 0 || x > ChunkData.ChunkWidth - 1
		          || y < 0 || y > ChunkData.ChunkHeight - 1
		          || z < 0 || z > ChunkData.ChunkWidth - 1)
			return false;

		return voxelMap[x, y, z];
	}

	private void PopulateVoxelMap()
	{
		for (int y = 0; y < ChunkData.ChunkHeight; y++)
		for (int x = 0; x < ChunkData.ChunkWidth; x++)
		for (int z = 0; z < ChunkData.ChunkWidth; z++)
		{
			voxelMap[x, y, z] = true;
		}
	}

	private void CreateMeshData()
	{
		for (int y = 0; y < ChunkData.ChunkHeight; y++)
		for (int x = 0; x < ChunkData.ChunkWidth; x++)
		for (int z = 0; z < ChunkData.ChunkWidth; z++)
		{
			AddVoxelDataToChunk(new Vector3Int(x, y, z));
		}
	}

	private int vertexIndex;
	private int uvIndex;
	private int triangleIndex;
	
	private void AddVoxelDataToChunk(Vector3Int pos)
	{
		for (int i = 0; i < 6; i++)
		{
			if (CheckVoxel(pos + VoxelData.FaceChecks[i]))
				continue;

			vertices[vertexIndex] = pos + VoxelData.Verts[VoxelData.Tris[i, 0]];
			vertices[vertexIndex + 1] = pos + VoxelData.Verts[VoxelData.Tris[i, 1]];
			vertices[vertexIndex + 2] = pos + VoxelData.Verts[VoxelData.Tris[i, 2]];
			vertices[vertexIndex + 3] = pos + VoxelData.Verts[VoxelData.Tris[i, 3]];

			uvs[uvIndex] = VoxelData.Uvs[0];
			uvs[uvIndex + 1] = VoxelData.Uvs[1];
			uvs[uvIndex + 2] = VoxelData.Uvs[2];
			uvs[uvIndex + 3] = VoxelData.Uvs[3];

			triangles[triangleIndex] = vertexIndex;
			triangles[triangleIndex + 1] = vertexIndex + 1;
			triangles[triangleIndex + 2] = vertexIndex + 2;
			triangles[triangleIndex + 3] = vertexIndex + 2;
			triangles[triangleIndex + 4] = vertexIndex + 1;
			triangles[triangleIndex + 5] = vertexIndex + 3;

			vertexIndex += 4;
			uvIndex += 4;
			triangleIndex += 6;
		}
	}

	private void CreateMesh()
	{
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