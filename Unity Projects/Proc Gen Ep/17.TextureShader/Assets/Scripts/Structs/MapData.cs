using UnityEngine;

public struct MapData {
	public readonly float[,] HeightMap;

	public MapData (float[,] heightMap)
	{
		HeightMap = heightMap;

	}
}
