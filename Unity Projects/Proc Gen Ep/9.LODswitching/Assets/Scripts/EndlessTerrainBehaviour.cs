using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrainBehaviour : MonoBehaviour {

	//to not update the visible chunck
	const float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 25f;
	const float SQUARED_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 
		VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;
	Vector2 oldViewerPosition;

	//how far the viewer can see
	public static float MaxViewDist;
	//ref to the viewer's Transform: to figure out his position
	public Transform Viewer; 

	public static Vector2 ViewerPosition;//static to a easier access later on

	int chunkSize;
	//Only chuncks that is visible, depends on chuncksize and the MAX_VIEW_DIST (the number of chuncks)
	int chunksVisibleInViewDistance;

	//Used to check for duplicate chunks
	Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

	public static MapGeneratorBehaviour MapGenerator;

	public Material MapMaterial;

	public LODInfo[] DetailLevels;

	// Use this for initialization
	void Start () {
		MapGenerator = FindObjectOfType<MapGeneratorBehaviour> ();

		MaxViewDist = DetailLevels [DetailLevels.Length - 1].visibleDistanceThreshold;

		//fetch the chuncksize from the MapGenerator 
		chunkSize = MapGeneratorBehaviour.MAP_CHUNK_SIZE - 1;// 241 but the actual size is 240 x 240
		chunksVisibleInViewDistance = Mathf.RoundToInt( MaxViewDist/ chunkSize ); //the number of chuncks is an integer
		UpdateVisibleChunks ();	//since the condition(2) may not be evaluate to true at the start
	}

	void Update(){
		ViewerPosition = new Vector2 (Viewer.position.x, Viewer.position.z);

		if ((oldViewerPosition - ViewerPosition).sqrMagnitude > 
			SQUARED_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE) { //condition(2)
			oldViewerPosition = ViewerPosition;
			UpdateVisibleChunks ();	
		}
	}

	void UpdateVisibleChunks(){

		foreach (var item in terrainChunkVisibleLastUpdate) {
			item.SetVisible (false);
		}
		terrainChunkVisibleLastUpdate.Clear ();

		//get the coordinate of the chunk where the viewer is standing on
		int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / chunkSize);// (240;-240) : position (1,-1)
		int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / chunkSize);

		//loop throught all the surrounding chunks
		for (int yOffset = - chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
			for (int xOffset = - chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset,
					                           currentChunkCoordY + yOffset);

				if (terrainChunkDict.ContainsKey(viewedChunkCoord)) {
					terrainChunkDict [viewedChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDict[viewedChunkCoord].IsVisible) {
						terrainChunkVisibleLastUpdate.Add (terrainChunkDict [viewedChunkCoord]);
					}
				} else {
					//Instantiate new terrain chunk
					terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize,
						DetailLevels, transform, MapMaterial));
				}
			}
		}
	}
}
