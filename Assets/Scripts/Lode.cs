using UnityEngine;

[System.Serializable]
public class Lode
{
	[SerializeField] private string name;
	[SerializeField] private byte blockId;
	[SerializeField, Tooltip("Inclusive")] private int minHeight;
	[SerializeField, Tooltip("Inclusive")] private int maxHeight;
	[SerializeField] private float scale;
	[SerializeField] private float threshold;
	[SerializeField] private float noiseOffset;

	public string Name => name;
	public byte BlockId => blockId;
	public int MinHeight => minHeight;
	public int MaxHeight => maxHeight;
	public float Scale => scale;
	public float Threshold => threshold;
	public float NoiseOffset => noiseOffset;
}