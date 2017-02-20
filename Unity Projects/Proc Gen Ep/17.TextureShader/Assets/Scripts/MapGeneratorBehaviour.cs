using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGeneratorBehaviour : MonoBehaviour {

	//Constants
	//public const int MAP_CHUNK_SIZE = 241; //241 - 1 = 240, a multiple of 2,4,6,8 for the LOD
	//public const int MAP_CHUNK_SIZE = 95; //because of the border

	public NoiseData noiseData;
	public TerrainData terrainData;
	public TextureData textureData;

	public Material TerrainMaterial;

	public const int MAP_CHUNK_SIZE_FLAT_SHADING = 95;
	public const int MAP_CHUNK_SIZE_NORMAL_SHADING = 239;

	//public members
	[Range(0,6)]
	public int EditorPreviewLOD; // level of simplification: increment by 1

//	public int MapWidth;
//	public int MapHeight;

	public DrawMode Mode;

	public bool AutoUpdate;

	float[,] _falloffMap;

	public int MapChunkSize {
		get{ 
			if (terrainData.UseFlatShading) {
				return MAP_CHUNK_SIZE_FLAT_SHADING;
			} else {
				return MAP_CHUNK_SIZE_NORMAL_SHADING;
			}
		}
	}

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

	void OnValuesUpdated(){
		if (!Application.isPlaying) {//we are in the editor, so ie application is not playing
			DrawMapInEditor();
		}
	}

	void OnTextureValuesUpdated(){
		textureData.ApplyToMaterial (TerrainMaterial);
	}

	public void RequestMapData(Vector2 center, Action<MapData> callback){
		ThreadStart threadStart = delegate {
			MapDataThread (center, callback);
		};
		new Thread (threadStart).Start ();
	}

	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback){
		ThreadStart threadStart = delegate {
			MeshDataThread (mapData, lod, callback);
		};
		new Thread (threadStart).Start ();
	}

	void MapDataThread(Vector2 center, Action<MapData> callback){
		MapData mapData = GenerateMapData (center);
		lock (mapDataThreadInfoQueue) {// when one thread reaches this point, while it executes this code, no other thread can execute the same, need to wait
			mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));
		}
	}

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback){
		MeshData meshData = MeshGenerator.GenerateTerrainMesh (mapData.HeightMap, 
			terrainData.MeshHeightMultiplier, terrainData.MeshHeightCurve, lod, terrainData.UseFlatShading);
		lock (meshDataThreadInfoQueue) {
			meshDataThreadInfoQueue.Enqueue (new MapThreadInfo<MeshData> (callback, meshData));
		}
	}

	void Update(){
		if (mapDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue ();//Dequeue= the next thing in a queue
				threadInfo.Callback (threadInfo.Parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue ();
				threadInfo.Callback (threadInfo.Parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 center){
		var offset = new Float2{
			x = noiseData.Offset.x + center.x, 
			y = noiseData.Offset.y + center.y 
		};

		float[,] noiseMap = Noise.GenerateNoiseMap (MapChunkSize + 2, MapChunkSize + 2, noiseData.Seed, 
			noiseData.NoiseScale, noiseData.OctavesNumber, noiseData.Persistance, noiseData.Lacunarity, 
			offset, noiseData.NormalizeMode);

		if (terrainData.UseFalloff) {

			if (_falloffMap == null) {
				_falloffMap = FallofGenerator.GenerateFalloffMap (MapChunkSize  + 2);
			}

			//Assignation of color to regions (loop through the noiseMap)
			for (int y = 0; y < MapChunkSize + 2; y++) {
				for (int x = 0; x < MapChunkSize + 2; x++) {

					if (terrainData.UseFalloff) {
						noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - _falloffMap [x, y]);
					}


				}
			}
		}

		textureData.UpdateMeshHeight (TerrainMaterial, terrainData.MinHeight, terrainData.MaxHeight);

		return new MapData(noiseMap);
	}

	public void DrawMapInEditor ()
	{
		MapData mapData = GenerateMapData (new Vector2(0,0));//pass the position zero

		MapDisplayBehaviour display = FindObjectOfType<MapDisplayBehaviour> ();
		if (Mode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.HeightMap));
		}
		if (Mode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.HeightMap, 
				terrainData.MeshHeightMultiplier, terrainData.MeshHeightCurve, EditorPreviewLOD, 
				terrainData.UseFlatShading));
		}
		if (Mode == DrawMode.FalloffMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (
				FallofGenerator.GenerateFalloffMap (MapChunkSize)));
		}
	}

	//Automatically called whenever one of the script var has been changed by the inspector
	void OnValidate(){

		if (terrainData != null) {
			terrainData.OnValuesUpdated -= OnValuesUpdated;//to unsubscribe : keep the subscription number at one
			terrainData.OnValuesUpdated += OnValuesUpdated;
		}
		if (noiseData != null) {
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null) {
			textureData.OnValuesUpdated -= OnValuesUpdated;
			textureData.OnValuesUpdated += OnValuesUpdated;
		}
	}
}