using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGeneratorBehaviour : MonoBehaviour {

	//Constants
	//public const int MAP_CHUNK_SIZE = 241; //241 - 1 = 240, a multiple of 2,4,6,8 for the LOD
	public const int MAP_CHUNK_SIZE = 239; //because of the border

	//public members
	[Range(0,6)]
	public int EditorPreviewLOD; // level of simplification: increment by 1

//	public int MapWidth;
//	public int MapHeight;

	public float NoiseScale;

	public int OctavesNumber;

	[Range(0,1)] //turn the Persistance UI into a Slider
	public float Persistance;
	public float Lacunarity;

	public int Seed;
	public Float2 Offset;

	public TerrainType[] Regions;
	public DrawMode Mode;

	public float MeshHeightMultiplier; //to elevate the heights
	public AnimationCurve MeshHeightCurve; //to flatten the water

	public NormalizedMode Normalization;

	public bool UseFalloff;
	public bool AutoUpdate;

	float[,] _falloffMap;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

	void Awake(){
		_falloffMap = FallofGenerator.GenerateFalloffMap (MAP_CHUNK_SIZE);
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
		MeshData meshData = MeshGenerator.GenerateTerrainMesh (mapData.HeightMap, MeshHeightMultiplier, 
			MeshHeightCurve, lod);
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
			x = Offset.x + center.x, 
			y = Offset.y + center.y 
		};

		float[,] noiseMap = Noise.GenerateNoiseMap (MAP_CHUNK_SIZE + 2, MAP_CHUNK_SIZE + 2, Seed, NoiseScale,
			OctavesNumber, Persistance, Lacunarity, offset, Normalization);

		//Store the colors in respect of the Map height
		Color[] colorMap = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];

		float currentHeight = 0;
		//Assignation of color to regions (loop through the noiseMap)
		for (int y = 0; y < MAP_CHUNK_SIZE; y++) {
			for (int x = 0; x < MAP_CHUNK_SIZE; x++) {

				if (UseFalloff) {
					noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - _falloffMap [x, y]);
				}

				currentHeight = noiseMap[x, y];
				for (int i = 0; i < Regions.Length; i++) {
					if (currentHeight >= Regions[i].Height) {
						colorMap [y * MAP_CHUNK_SIZE + x] = Regions [i].TerrainColor; //if it is exceeding the regions height
					} else {
						break;//no need to 
					}
				}
			}
		}
		return new MapData(noiseMap, colorMap);
	}

	public void DrawMapInEditor ()
	{
		MapData mapData = GenerateMapData (new Vector2(0,0));//pass the position zero

		MapDisplayBehaviour display = FindObjectOfType<MapDisplayBehaviour> ();
		if (Mode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.HeightMap));
		}
		if (Mode == DrawMode.ColorMap) {
			display.DrawTexture (TextureGenerator.TextureFromColorMap (mapData.ColorMap, MAP_CHUNK_SIZE,
				MAP_CHUNK_SIZE));
		}
		if (Mode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.HeightMap, MeshHeightMultiplier,
				MeshHeightCurve, EditorPreviewLOD), 
				TextureGenerator.TextureFromColorMap (mapData.ColorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
		}
		if (Mode == DrawMode.FalloffMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (
				FallofGenerator.GenerateFalloffMap (MAP_CHUNK_SIZE)));
		}
	}

	//Automatically called whenever one of the script var has been changed by the inspector
	void OnValidate(){
		if (Lacunarity < 1) {
			Lacunarity = 1;
		}

		if (OctavesNumber < 0) {
			OctavesNumber = 0;
		}

		_falloffMap = FallofGenerator.GenerateFalloffMap (MAP_CHUNK_SIZE);
	}
}