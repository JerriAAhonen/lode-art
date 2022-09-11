using System.Collections.Generic;
using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Database", fileName = "Blocks")]
public class BlockDatabase : ScriptableObject
{
	[SerializeField] private List<BlockType> blockTypes;

	public List<BlockType> Types => blockTypes;
}