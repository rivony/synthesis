using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODMesh {

	public Mesh MeshToFetch;
	public bool HasRequestedMesh;
	public bool HasMesh; // if we receive the mesh
	int Lod; //LOD of the current mesh
	System.Action _updateCallback;

	public LODMesh (int lod, System.Action updateCallback)
	{
		Lod = lod;
		_updateCallback = updateCallback;
	}

	void OnMeshDataReceived(MeshData meshData){
		MeshToFetch = meshData.CreateMesh ();
		HasMesh = true;

		//call the update callback
		_updateCallback();
	}

	public void RequestMesh(MapData mapData){
		HasRequestedMesh = true;
		EndlessTerrainBehaviour.MapGenerator.RequestMeshData (mapData, Lod, OnMeshDataReceived);
	}
}
