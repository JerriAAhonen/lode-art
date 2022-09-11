namespace Data
{
	public static class TextureData
	{
		public const int AtlasSizeInBlocks = 16;
		
		private static float normalizedBlockTextureSize;
		public static float NormalizedBlockTextureSize
		{
			get
			{
				if (normalizedBlockTextureSize == 0)
					normalizedBlockTextureSize = 1f / AtlasSizeInBlocks;
				return normalizedBlockTextureSize;
			}
		}
	}
}