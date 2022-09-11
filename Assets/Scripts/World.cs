using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Util;

public class World : Singleton<World>
{
	[SerializeField] private Material blockAtlasMaterial;
	[SerializeField] private BlockDatabase blockDatabase;

	private Chunk[,] chunks = new Chunk[WorldData.WorldSizeInChunks, WorldData.WorldSizeInChunks];

	public Material BlockMaterial => blockAtlasMaterial;
	public List<BlockType> Blocks => blockDatabase.Types;

	private void Start()
	{
		GenerateWorld();
	}

	/// <summary>
	/// Get Block id in WORLD COORDS
	/// </summary>
	public byte GetVoxel(int x, int y, int z)
	{
		return GetVoxel(new Vector3Int(x, y, z));
	}
	
	/// <summary>
	/// Get Block id in WORLD COORDS
	/// </summary>
	public byte GetVoxel(Vector3Int pos)
	{
		if (!IsVoxelInWorld(pos))
			return 0;
		
		if (pos.y < 1)
			return 1;
		if (pos.y == ChunkData.ChunkHeight - 1)
			return 3;
		return 2;
	}

	private void GenerateWorld()
	{
		for (int x = 0; x < WorldData.WorldSizeInChunks; x++)
		for (int z = 0; z < WorldData.WorldSizeInChunks; z++)
		{
			CreateChunk(x, z);
		}
	}

	private void CreateChunk(int x, int z)
	{
		chunks[x, z] = new Chunk(x, z);
	}

	private bool IsChunkInWorld(ChunkCoord coord)
	{
		return coord.X is > 0 and < WorldData.WorldSizeInChunks - 1
		       && coord.Z is > 0 and < WorldData.WorldSizeInChunks - 1;
	}

	private bool IsVoxelInWorld(Vector3Int pos)
	{
		return pos.x >= 0 && pos.x < WorldData.WorldSizeInVoxels 
		                 && pos.y is >= 0 and < ChunkData.ChunkHeight
		                 && pos.z >= 0 && pos.z < WorldData.WorldSizeInVoxels;
	}
}