using Data;
using UnityEngine;

public static class Noise
{
	public static float Get2DPerlin(Vector2 pos, float offset, float scale)
	{
		return Mathf.PerlinNoise(
			(pos.x + 0.1f) / ChunkData.ChunkWidth * scale + offset,
			(pos.y + 0.1f) / ChunkData.ChunkWidth * scale + offset);
	}

	public static bool Get3DPerlin(Vector3 pos, float offset, float scale, float threshold)
	{
		var x = (pos.x + offset + 0.1f) * scale;
		var y = (pos.y + offset + 0.1f) * scale;
		var z = (pos.z + offset + 0.1f) * scale;

		var ab = Mathf.PerlinNoise(x, y);
		var bc = Mathf.PerlinNoise(y, z);
		var ac = Mathf.PerlinNoise(x, z);
		
		var ba = Mathf.PerlinNoise(y, x);
		var cb = Mathf.PerlinNoise(z, y);
		var ca = Mathf.PerlinNoise(z, x);

		var abc = ab + bc + ac + ba + cb + ca;
		return abc / 6f > threshold;
	}
}