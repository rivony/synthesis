using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGeneratorBehaviour : MonoBehaviour {

	//Constants
	//public const int MAP_CHUNK_SIZE = 241; //241 - 1 = 240, a multiple of 2,4,6,8 for the LOD
	//public const int MAP_CHUNK_SIZE = 95; //because of the border

	public const int MAP_CHUNK_SIZE_FLAT_SHADING = 95;
	public const int MAP_CHUNK_SIZE_NORMAL_SHADING = 239;

	public bool UseFlatShading;

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

	static MapGeneratorBehaviour instance;

	public static int MapChunkSize {
		get{ 
			if (instance == null) {
				instance = FindObjectOfType<MapGeneratorBehaviour> ();
			}

			if (instance.UseFlatShading) {
				return MAP_CHUNK_SIZE_FLAT_SHADING;
			} else {
				return MAP_CHUNK_SIZE_NORMAL_SHADING;
			}
		}
	}

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

	void Awake(){
		_falloffMap = FallofGenerator.GenerateFalloffMap (MapChunkSize);
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
			MeshHeightCurve, lod, UseFlatShading);
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

		float[,] noiseMap = Noise.GenerateNoiseMap (MapChunkSize + 2, MapChunkSize + 2, Seed, NoiseScale,
			OctavesNumber, Persistance, Lacunarity, offset, Normalization);

		//Store the colors in respect of the Map height
		Color[] colorMap = new Color[MapChunkSize * MapChunkSize];

		float currentHeight = 0;
		//Assignation of color to regions (loop through the noiseMap)
		for (int y = 0; y < MapChunkSize; y++) {
			for (int x = 0; x < MapChunkSize; x++) {

				if (UseFalloff) {
					noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - _falloffMap [x, y]);
				}

				currentHeight = noiseMap[x, y];
				for (int i = 0; i < Regions.Length; i++) {
					if (currentHeight >= Regions[i].Height) {
						colorMap [y * MapChunkSize + x] = Regions [i].TerrainColor; //if it is exceeding the regions height
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
			display.DrawTexture (TextureGenerator.TextureFromColorMap (mapData.ColorMap, MapChunkSize,
				MapChunkSize));
		}
		if (Mode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (mapData.HeightMap, MeshHeightMultiplier,
				MeshHeightCurve, EditorPreviewLOD, UseFlatShading), 
				TextureGenerator.TextureFromColorMap (mapData.ColorMap, MapChunkSize, MapChunkSize));
		}
		if (Mode == DrawMode.FalloffMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (
				FallofGenerator.GenerateFalloffMap (MapChunkSize)));
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

		_falloffMap = FallofGenerator.GenerateFalloffMap (MapChunkSize);
	}
}