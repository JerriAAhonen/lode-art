using UnityEngine;

namespace Data
{
	public class ChunkCoord
	{
		public int X { get; }
		public int Z { get; }

		public Vector3 WorldPos => new(X * ChunkData.ChunkWidth, 0, Z * ChunkData.ChunkWidth);

		public ChunkCoord(Vector2 coords) : this(Mathf.FloorToInt(coords.x), Mathf.FloorToInt(coords.y)) { }
		public ChunkCoord(Vector2Int coords) : this(coords.x, coords.y) { }

		public ChunkCoord(int x, int z)
		{
			X = x;
			Z = z;
		}

		public override string ToString()
		{
			return $"Chunk ({X}, {Z})";
		}

		public static ChunkCoord FromWorldVector3(Vector3 pos)
		{
			var x = Mathf.FloorToInt(pos.x / ChunkData.ChunkWidth);
			var z = Mathf.FloorToInt(pos.z / ChunkData.ChunkWidth);
			return new ChunkCoord(x, z);
		}

		public bool Equals(ChunkCoord other)
		{
			if (other == null)
				return false;

			return X == other.X && Z == other.Z;
		}
	}
}