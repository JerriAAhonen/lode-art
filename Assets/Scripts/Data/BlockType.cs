using System;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class BlockType
	{
		#region Unity Editor

		/// <summary>
		/// Used to display a custom title in the Unity inspector
		/// </summary>
		[HideInInspector] public string inspectorTitle;

		#endregion

		[SerializeField] private string name;
		[SerializeField] private bool isSolid;

		[Header("Texture Values")] 
		[SerializeField] private int back;
		[SerializeField] private int front;
		[SerializeField] private int top;
		[SerializeField] private int bottom;
		[SerializeField] private int left;
		[SerializeField] private int right;

		public string Name => name;
		public bool IsSolid => isSolid;

		public int GetTextureId(VoxelData.Face face)
		{
			return face switch
			{
				VoxelData.Face.Back => back,
				VoxelData.Face.Front => front,
				VoxelData.Face.Top => top,
				VoxelData.Face.Bottom => bottom,
				VoxelData.Face.Left => left,
				VoxelData.Face.Right => right,
				_ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
			};
		}
	}
}