using UnityEngine;

[System.Serializable] //to be available in the editor
public struct TerrainType {
	public string Name;
	public float Height;
	public Color TerrainColor;

	public TerrainType (string name, float height, Color color){
		Name = name;
		Height = height;
		TerrainColor = color;
	}
}
