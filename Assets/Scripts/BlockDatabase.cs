using System.Collections.Generic;
using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Database", fileName = "Blocks")]
public class BlockDatabase : ScriptableObject
{
	[SerializeField] private List<BlockType> blockTypes;

	public List<BlockType> Types => blockTypes;

	/// <summary>
	/// Used to display the index of the block type in the inspector
	/// </summary>
	private void OnValidate()
	{
		for (int i = 0; i < blockTypes.Count; i++)
			blockTypes[i].inspectorTitle = $"{i} {blockTypes[i].Name}";
	}
}