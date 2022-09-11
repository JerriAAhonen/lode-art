namespace Data
{
	public static class WorldData
	{
		public const int WorldSizeInChunks = 5;
		
		private static int worldSizeInVoxels;
		public static int WorldSizeInVoxels
		{
			get
			{
				if (worldSizeInVoxels == 0)
					worldSizeInVoxels = WorldSizeInChunks * ChunkData.ChunkWidth;
				return worldSizeInVoxels;
			}
		}
	}
}