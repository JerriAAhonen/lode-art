using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

public class World : Singleton<World>
{
	[SerializeField] private Material blockAtlasMaterial;
	[SerializeField] private BlockDatabase blockDatabase;
	[SerializeField] private BiomeAttributes biome;
	[SerializeField] private Player player;

	private int seed = 1234;
	
	private Vector3 spawnPos;
	private ChunkCoord lastPlayerCC;

	private Chunk[,] chunks = new Chunk[WorldData.WorldSizeInChunks, WorldData.WorldSizeInChunks];
	private List<ChunkCoord> activeChunks = new();
	private List<ChunkCoord> chunksToInit = new();
	private Coroutine createChunksRoutine;

	public Material BlockMaterial => blockAtlasMaterial;
	public List<BlockData> Blocks => blockDatabase.Types;
	public Player Player => player;

	private void Start()
	{
		Random.InitState(seed);
		
		spawnPos = new Vector3(
			WorldData.WorldSizeInVoxels / 2f, 
			ChunkData.ChunkHeight - 20,
			WorldData.WorldSizeInVoxels / 2f);
		spawnPos += Vector3.up * 1.8f;
		lastPlayerCC = ChunkCoord.FromWorldVector3(spawnPos);
		
		GenerateWorld();
		
		player.transform.position = spawnPos;
	}

	private void Update()
	{
		if (!ChunkCoord.FromWorldVector3(player.transform.position).Equals(lastPlayerCC))
			CheckViewDistance();

		if (chunksToInit.Count > 0 && createChunksRoutine == null)
			createChunksRoutine = StartCoroutine(CreateChunks());
	}
	
	/// <param name="pos">World space position</param>
	public byte GetVoxelBlockID(Vector3Int pos)
	{
		var y = pos.y;
		
		// ----------------------------------
		// Immutable pass
		
		if (!IsVoxelInWorld(pos))
			return 0; // Air

		if (y == 0)
			return 1; // Bedrock

		// ----------------------------------
		// Basic terrain pass

		var noise = Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.TerrainScale);
		var terrainHeight = Mathf.FloorToInt(biome.TerrainHeight * noise) + biome.SurfaceHeight;
		byte voxelValue;

		if (y == terrainHeight)
			voxelValue = 4; // Grass
		else if (y < terrainHeight && y > terrainHeight - 4)
			voxelValue = 3; // Dirt
		else if (y > terrainHeight)
			return 0; // Air
		else
			voxelValue = 2; // Stone

		// ----------------------------------
		// Second pass - Lodes

		if (voxelValue == 2)
		{
			foreach (var lode in biome.Lodes)
			{
				if (y >= lode.MinHeight && y <= lode.MaxHeight)
					if (Noise.Get3DPerlin(pos, lode.NoiseOffset, lode.Scale, lode.Threshold))
						voxelValue = lode.BlockId;
			}
		}
		return voxelValue;
	}

	private void CheckViewDistance()
	{
		lastPlayerCC = ChunkCoord.FromWorldVector3(player.transform.position);
		var previouslyActiveChunks = new List<ChunkCoord>(activeChunks); // TODO Pooling

		for (int x = lastPlayerCC.X - Settings.ViewDistanceInChunks; x < lastPlayerCC.X + Settings.ViewDistanceInChunks; x++)
		for (int z = lastPlayerCC.Z - Settings.ViewDistanceInChunks; z < lastPlayerCC.Z + Settings.ViewDistanceInChunks; z++)
		{
			var coord = new ChunkCoord(x, z);
			if (IsChunkInWorld(coord))
			{
				if (chunks[x, z] == null)
				{
					chunks[x, z] = new Chunk(x, z, false);
					chunksToInit.Add(new ChunkCoord(x, z));
				}
				else if (!chunks[x, z].IsActive)
				{
					chunks[x, z].IsActive = true;
				}
				activeChunks.Add(coord);
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
		const int center = WorldData.WorldSizeInChunks / 2;

		for (int x = center - Settings.ViewDistanceInChunks; x < center + Settings.ViewDistanceInChunks; x++)
		for (int z = center - Settings.ViewDistanceInChunks; z < center + Settings.ViewDistanceInChunks; z++)
		{
			var cc = new ChunkCoord(x, z);
			chunks[x, z] = new Chunk(cc, true);
			activeChunks.Add(cc);
		}
	}

	private IEnumerator CreateChunks()
	{
		while (chunksToInit.Count > 0)
		{
			chunks[chunksToInit[0].X, chunksToInit[0].Z].Init();
			chunksToInit.RemoveAt(0);
			yield return null;
		}

		createChunksRoutine = null;
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

	public bool IsVoxelSolid(float x, float y, float z) =>
		IsVoxelSolid(new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z)));
	
	/// <param name="pos">World space position</param>
	public bool IsVoxelSolid(Vector3Int pos)
	{
		var coord = ChunkCoord.FromWorldVector3(pos);

		if (!IsVoxelInWorld(pos))
			return false;

		if (chunks[coord.X, coord.Z] != null && chunks[coord.X, coord.Z].IsVoxelMapPopulated)
			return Blocks[chunks[coord.X, coord.Z].GetVoxelFromWorldV3(pos)].IsSolid;

		return Blocks[GetVoxelBlockID(pos)].IsSolid;
	}
}