using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayBehaviour : MonoBehaviour {

	public Renderer TextureRender;

	public void DrawTexture(Texture2D texture){
		TextureRender.sharedMaterial.mainTexture = texture; // Renderer.material vs Renderer.sharedMaterial (the first at runtime, the second in the editor)
		TextureRender.transform.localScale = new Vector3(texture.width, 1, texture.height); 
	}

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public void DrawMesh(MeshData meshData){
		meshFilter.sharedMesh = meshData.CreateMesh ();//possible generation of the mesh from the outside of the game mode
		//set the mesh size == UniformScale of the terrain
		meshFilter.transform.localScale = new Vector3(1, 1, 1) * 
			FindObjectOfType<MapGeneratorBehaviour>().terrainData.UniformScale;
	}
}
