using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk {

	Vector2 position;
	GameObject meshObject;
	Bounds bounds;

	//need a plane to serve as a visual placeholder until the implementation in the mapGenerator
	public TerrainChunk (Vector2 coord, int size, Transform parent)
	{
		position = coord * size;

		Vector3 positionV3 = new Vector3 (position.x, 0, position.y);
		meshObject = GameObject.CreatePrimitive (PrimitiveType.Plane);
		meshObject.transform.position = positionV3;
		meshObject.transform.localScale = new Vector3 (1, 1, 1) * size / 10f; // the plane is by default 10 units, need to divide by 10 to get the correct scale
		meshObject.transform.parent = parent;

		bounds = new Bounds(position, new Vector2(1,1) * size);
		SetVisible (false);
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

	public void SetVisible(bool isVisible){
		meshObject.SetActive (isVisible);
	}

	public bool IsVisible {
		get{ return meshObject.activeSelf; }
	}
}
