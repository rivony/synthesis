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
	MeshCollider meshCollider;

	LODInfo[] DetailsLevels;
	LODMesh[] LodMeshes;
	LODMesh m_CollisionLodMesh;

	bool MapDataReceived;

	int previousLodIndex = -1;

	public bool IsVisible {
		get{ return meshObject.activeSelf; }
	}

	//need a plane to serve as a visual placeholder until the implementation in the mapGenerator
	public TerrainChunk (Vector2 coord, int size, LODInfo[] detailsLevels, Transform parent, Material material)
	{
		DetailsLevels = detailsLevels;

		position = coord * size;

		Vector3 positionV3 = new Vector3 (position.x, 0, position.y);
		meshObject = new GameObject("Terrain chunk");
		meshObject.transform.position = positionV3 * EndlessTerrainBehaviour.SCALE;
		meshObject.transform.parent = parent;
		meshObject.transform.localScale = new Vector3 (1, 1, 1) * EndlessTerrainBehaviour.SCALE;

		meshRenderer = meshObject.AddComponent<MeshRenderer> ();
		meshRenderer.material = material;
		meshFilter = meshObject.AddComponent<MeshFilter> ();
		meshCollider = meshObject.AddComponent<MeshCollider> ();

		bounds = new Bounds(position, new Vector2(1,1) * size);
		SetVisible (false);

		LodMeshes = new LODMesh[DetailsLevels.Length];
		for (int i = 0; i < DetailsLevels.Length; i++) {
			LodMeshes [i] = new LODMesh (DetailsLevels [i].LevelOfDetail, UpdateTerrainChunk);
			if (DetailsLevels[i].useForCollider) {
				m_CollisionLodMesh = LodMeshes [i];
			}
		}

		EndlessTerrainBehaviour.MapGenerator.RequestMapData (position, OnMapDataReceived);
	}

	public void SetVisible(bool isVisible){
		meshObject.SetActive (isVisible);
	}

	//tell the terrain chunk  to update itself: find the point on its perimeters that is the 
	//closest to the viewer's position, and find the distance between that point and the viewer
	//if the distance is less than the maximum value distance then make sure that the meshObject is enabled
	//otherwise if that distance exceed the max, then it will disable the mesh object
	public void UpdateTerrainChunk(){
		if (! MapDataReceived) {
			return;
		}

		Vector2 viewerPosition = EndlessTerrainBehaviour.ViewerPosition;
		float viewerDistanceFromNearestEdge = Mathf.Sqrt(
			bounds.SqrDistance (viewerPosition));
		bool chunkIsVisible = viewerDistanceFromNearestEdge <= EndlessTerrainBehaviour.MaxViewDist;

		if (chunkIsVisible) {
			int lodIndex = 0;
			for (int i = 0; i < DetailsLevels.Length - 1; i++) {//
				if (viewerDistanceFromNearestEdge > DetailsLevels[i].visibleDistanceThreshold) {
					lodIndex = i + 1;
				} else {
					break;
				}
			}
			if (lodIndex != previousLodIndex) {
				LODMesh lodMesh = LodMeshes [lodIndex];
				if (lodMesh.HasMesh) {
					previousLodIndex = lodIndex;
					meshFilter.mesh = lodMesh.MeshToFetch;
				}
				else if (! lodMesh.HasRequestedMesh) {
					lodMesh.RequestMesh (mapData);
				}
			}

			if (lodIndex.Equals(0)) {
				//if the player is close enough
				if (m_CollisionLodMesh.HasMesh) {
					meshCollider.sharedMesh = m_CollisionLodMesh.MeshToFetch;
				}
				else if (! m_CollisionLodMesh.HasRequestedMesh) {
					m_CollisionLodMesh.RequestMesh (mapData);
				}
			}

			//if it is visible, it will add itself in the chunkList
			EndlessTerrainBehaviour.TerrainChunkVisibleLastUpdate.Add (this);

		}

		SetVisible (chunkIsVisible);
	}

	void OnMapDataReceived(MapData data){
		this.mapData = data;
		MapDataReceived = true;

		//Add texture
		Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, 
			MapGeneratorBehaviour.MAP_CHUNK_SIZE, MapGeneratorBehaviour.MAP_CHUNK_SIZE);
		meshRenderer.material.mainTexture = texture;

		UpdateTerrainChunk ();
	}
}
