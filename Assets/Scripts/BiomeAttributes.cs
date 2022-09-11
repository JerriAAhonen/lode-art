using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Biomes/Attributes", fileName = "BiomeAttributes")]
public class BiomeAttributes : ScriptableObject
{
	[SerializeField] private string biomeName;
	[SerializeField] private int surfaceHeight;
	[SerializeField] private int terrainHeight;
	[SerializeField] private float terrainScale;
	[SerializeField] private Lode[] lodes;
	
	public string Name => biomeName;
	public int SurfaceHeight => surfaceHeight;
	public int TerrainHeight => terrainHeight;
	public float TerrainScale => terrainScale;
	public Lode[] Lodes => lodes;
}