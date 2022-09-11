using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Util;

public class World : Singleton<World>
{
	[SerializeField] private Material blockAtlasMaterial;
	[SerializeField] private BlockDatabase blockDatabase;
	[SerializeField] private Transform player;
	
	private Vector3 spawnPos;
	private ChunkCoord lastPlayerCC;

	private Chunk[,] chunks = new Chunk[WorldData.WorldSizeInChunks, WorldData.WorldSizeInChunks];
	private List<ChunkCoord> activeChunks = new();

	public Material BlockMaterial => blockAtlasMaterial;
	public List<BlockType> Blocks => blockDatabase.Types;

	private void Start()
	{
		spawnPos = new Vector3(
			WorldData.WorldSizeInVoxels / 2f, 
			ChunkData.ChunkHeight,
			WorldData.WorldSizeInVoxels / 2f);
		spawnPos += Vector3.up * 1.8f;
		lastPlayerCC = ChunkCoord.FromWorldVector3(spawnPos);
		
		GenerateWorld();
	}

	private void Update()
	{
		if (!ChunkCoord.FromWorldVector3(player.position).Equals(lastPlayerCC))
			CheckViewDistance();
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

	private void CheckViewDistance()
	{
		lastPlayerCC = ChunkCoord.FromWorldVector3(player.position);
		var previouslyActiveChunks = new List<ChunkCoord>(activeChunks); // TODO Pooling

		for (int x = lastPlayerCC.X - Settings.ViewDistanceInChunks; x < lastPlayerCC.X + Settings.ViewDistanceInChunks; x++)
		for (int z = lastPlayerCC.Z - Settings.ViewDistanceInChunks; z < lastPlayerCC.Z + Settings.ViewDistanceInChunks; z++)
		{
			var coord = new ChunkCoord(x, z);
			if (IsChunkInWorld(coord))
			{
				if (chunks[x, z] == null)
					CreateChunk(x, z);
				else if (!chunks[x, z].IsActive)
				{
					chunks[x, z].IsActive = true;
					activeChunks.Add(coord);
				}
			}

			for (int i = 0; i < previouslyActiveChunks.Count; i++)
			{
				if (previouslyActiveChunks[i].Equals(coord))
					previouslyActiveChunks.RemoveAt(i);
			}
		}

		foreach (var c in previouslyActiveChunks)
		{
			chunks[c.X, c.Z].IsActive = false;
		}
	}
	
	private void GenerateWorld()
	{
		var center = WorldData.WorldSizeInChunks / 2;
		
		for (int x = center - Settings.ViewDistanceInChunks; x < center + Settings.ViewDistanceInChunks; x++)
		for (int z = center - Settings.ViewDistanceInChunks; z < center + Settings.ViewDistanceInChunks; z++)
		{
			CreateChunk(x, z);
		}

		player.position = spawnPos;
	}

	private void CreateChunk(int x, int z) => CreateChunk(new ChunkCoord(x, z));
	private void CreateChunk(ChunkCoord coord)
	{
		chunks[coord.X, coord.Z] = new Chunk(coord.X, coord.Z);
		activeChunks.Add(coord);
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