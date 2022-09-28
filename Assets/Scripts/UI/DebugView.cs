using System;
using Data;
using TMPro;
using UnityEngine;
using Util;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class DebugView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI label;
	
		private CanvasGroup cg;

		private string text;
		private int fps;
		private float timer;

		private int halfWorldSizeInVoxels;
		private int halfWorldSizeInChunks;
		
		#region MonoBehaviour
		
		private void Start()
		{
			cg = GetComponent<CanvasGroup>();
			InputManager.I.ToggleDebugScreen += OnToggle;

			halfWorldSizeInVoxels = WorldData.WorldSizeInVoxels / 2;
			halfWorldSizeInChunks = WorldData.WorldSizeInChunks / 2;
			
			Show(false);
		}
		
		private void Update()
		{
			if (timer > 0.5f)
			{
				fps = (int)(1f / Time.unscaledDeltaTime);
				timer = 0f;
			}
			else
				timer += Time.unscaledDeltaTime;
			
			var playerPos = World.I.Player.transform.position;
			var playerX = Mathf.FloorToInt(playerPos.x) - halfWorldSizeInVoxels;
			var playerY = Mathf.FloorToInt(playerPos.y);
			var playerZ = Mathf.FloorToInt(playerPos.z) - halfWorldSizeInVoxels;
			var playerChunk = ChunkCoord.FromWorldVector3(playerPos);
			var playerChunkX = playerChunk.X - halfWorldSizeInChunks;
			var playerChunkZ = playerChunk.Z - halfWorldSizeInChunks;
			
			Clear();
			AddLine($"Fps: {fps}");
			AddLine($"X: {playerX} | Y: {playerY} | Z: {playerZ}");
			AddLine($"Chunk: [{playerChunkX}, {playerChunkZ}]");

			label.text = text;
		}

		private void OnDestroy()
		{
			InputManager.I.ToggleDebugScreen -= OnToggle;
		}

		#endregion
		
		private void OnToggle()
		{
			Show(Mathf.Approximately(cg.alpha, 0));
		}

		private void Show(bool show)
		{
			cg.alpha = show ? 1f : 0f;
		}

		private void AddLine(string line)
		{
			text += line;
			text += "\n";
		}

		private void Clear()
		{
			text = "";
		}
	}
}