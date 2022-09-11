using System.Collections.Generic;
using Data;
using UnityEngine;
using Util;

public class World : Singleton<World>
{
	[SerializeField] private Material blockAtlasMaterial;
	[SerializeField] private BlockDatabase blockDatabase;

	public List<BlockType> Blocks => blockDatabase.Types;
}