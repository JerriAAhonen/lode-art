using Data;
using Extensions;
using UnityEngine;
using Util;

public class Chunk
{
	private GameObject chunkGO;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private ChunkCoord chunkCoord;

	private static int totalVertices => 6 * 6 * ChunkData.ChunkHeight * ChunkData.ChunkWidth * ChunkData.ChunkWidth;
	private readonly Vector3[] vertices = new Vector3[totalVertices];
	private readonly int[] triangles = new int[totalVertices];
	private readonly Vector2[] uvs = new Vector2[totalVertices];
	private bool isActive;

	private byte[,,] voxelMap = new byte[ChunkData.ChunkWidth, ChunkData.ChunkHeight, ChunkData.ChunkWidth];
	public byte[,,] VoxelMap => voxelMap;
	public bool IsVoxelMapPopulated { get; private set; }

	public bool IsActive
	{
		get => isActive;
		set
		{
			isActive = value;
			if (chunkGO != null)
				chunkGO.SetActive(value);
		}
	}

	public Vector3Int Position => chunkGO.transform.position.ToVector3Int();

	public Chunk(int x, int z, bool generateOnLoad) : this(new ChunkCoord(x, z), generateOnLoad) { }
	public Chunk(ChunkCoord chunkCoord, bool generateOnLoad)
	{
		this.chunkCoord = chunkCoord;
		isActive = true;
		if (generateOnLoad)
			Init();
	}

	public void Init()
	{
		chunkGO = new GameObject();
		chunkGO.transform.SetParent(World.I.transform);
		chunkGO.transform.position = chunkCoord.WorldPos;
		chunkGO.name = chunkCoord.ToString();
		
		meshFilter = chunkGO.AddComponent<MeshFilter>();
		meshRenderer = chunkGO.AddComponent<MeshRenderer>();
		meshRenderer.material = World.I.BlockMaterial;

		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();
	}

	private void PopulateVoxelMap()
	{
		for (int y = 0; y < ChunkData.ChunkHeight; y++)
		for (int x = 0; x < ChunkData.ChunkWidth; x++)
		for (int z = 0; z < ChunkData.ChunkWidth; z++)
		{
			voxelMap[x, y, z] = World.I.GetVoxelBlockID(new Vector3Int(x, y, z) + Position);
		}

		IsVoxelMapPopulated = true;
	}

	private void CreateMeshData()
	{
		for (int y = 0; y < ChunkData.ChunkHeight; y++)
		for (int x = 0; x < ChunkData.ChunkWidth; x++)
		for (int z = 0; z < ChunkData.ChunkWidth; z++)
		{
			if (World.I.Blocks[voxelMap[x, y, z]].IsSolid)
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

			var blockId = voxelMap[pos.x, pos.y, pos.z];
			var textureId = World.I.Blocks[blockId].GetTextureId((VoxelData.Face)i);
			AddTexture(textureId, uvIndex);

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
	
	/// <summary>
	/// IsVoxelSolid
	/// </summary>
	/// <param name="pos">LOCAL position of the Voxel inside the chunk</param>
	private bool CheckVoxel(Vector3Int pos)
	{
		int x = pos.x;
		int y = pos.y;
		int z = pos.z;

		// Get information of voxels in neighbouring chunks form World class
		if (!IsVoxelInChunk(x, y, z))
			return World.I.IsVoxelSolid(pos + Position);
		
		return World.I.Blocks[voxelMap[x, y, z]].IsSolid;
	}

	public byte GetVoxelFromWorldV3(Vector3Int pos)
	{
		var x = pos.x - Position.x;
		var y = pos.y;
		var z = pos.z - Position.z;

		return voxelMap[x, y, z];
	}
	
	private static bool IsVoxelInChunk(int x, int y, int z)
	{
		return x is >= 0 and <= ChunkData.ChunkWidth - 1 
		       && y is >= 0 and <= ChunkData.ChunkHeight - 1 
		       && z is >= 0 and <= ChunkData.ChunkWidth - 1;
	}

	private void AddTexture(int textureId, int index)
	{
		var normalized = TextureData.NormalizedBlockTextureSize;
		
		var row = textureId / TextureData.AtlasSizeInBlocks;
		// Invert the row because we want row 0 to be at the top, instead of the bottom of the texture.
		var inverted_row = TextureData.AtlasSizeInBlocks - 1 - row;
		// Column number is the remainder after dividing by the number of columns. E.g. 15 % 4 = 3
		var col = textureId % TextureData.AtlasSizeInBlocks;

		// Normalise the row/col numbers to get values between 0f and 1f.
		var x = col * normalized;
		var y = inverted_row * normalized;

		uvs[index] = new Vector2(x, y);
		uvs[index + 1] = new Vector2(x, y + normalized);
		uvs[index + 2] = new Vector2(x + normalized, y);
		uvs[index + 3] = new Vector2(x + normalized, y + normalized);
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