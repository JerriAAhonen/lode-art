using UnityEngine;

namespace Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3Int ToVector3Int(this Vector3 v3)
		{
			var x = Mathf.FloorToInt(v3.x);
			var y = Mathf.FloorToInt(v3.y);
			var z = Mathf.FloorToInt(v3.z);

			return new Vector3Int(x, y, z);
		}
	}
}