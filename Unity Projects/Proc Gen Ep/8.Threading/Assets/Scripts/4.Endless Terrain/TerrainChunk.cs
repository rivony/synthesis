using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk {

	Vector2 position;
	GameObject meshObject;
	Bounds bounds;

	MapData mapData;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;

	public bool IsVisible {
		get{ return meshObject.activeSelf; }
	}

	//need a plane to serve as a visual placeholder until the implementation in the mapGenerator
	public TerrainChunk (Vector2 coord, int size, Transform parent, Material material)
	{
		position = coord * size;

		Vector3 positionV3 = new Vector3 (position.x, 0, position.y);
		meshObject = new GameObject("Terrain chunk");
		meshObject.transform.position = positionV3;
		meshObject.transform.parent = parent;

		meshRenderer = meshObject.AddComponent<MeshRenderer> ();
		meshRenderer.material = material;
		meshFilter = meshObject.AddComponent<MeshFilter> ();

		bounds = new Bounds(position, new Vector2(1,1) * size);
		SetVisible (false);

		EndlessTerrainBehaviour.MapGenerator.RequestMapData (OnMapDataReceived);
	}

	public void SetVisible(bool isVisible){
		meshObject.SetActive (isVisible);
	}

	//tell the terrain chunk  to update itself: find the point on its perimeters that is the 
	//closest to the viewer's position, and find the distance between that point and the viewer
	//if the distance is less than the maximum value distance then make sure that the meshObject is enabled
	//otherwise if that distance exceed the max, then it will disable the mesh object
	public void UpdateTerrainChunk(){
		Vector2 viewerPosition = EndlessTerrainBehaviour.ViewerPosition;
		float viewerDistanceFromNearestEdge = Mathf.Sqrt(
			bounds.SqrDistance (viewerPosition));
		bool chunkIsVisible = viewerDistanceFromNearestEdge <= EndlessTerrainBehaviour.MAX_VIEW_DIST;
		SetVisible (chunkIsVisible);
	}

	void OnMapDataReceived(MapData mapData){
		EndlessTerrainBehaviour.MapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
	}

	void OnMeshDataReceived(MeshData meshData){
		meshFilter.mesh = meshData.CreateMesh ();
	}

	
}
