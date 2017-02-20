using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int Width;
	public int Height;

	public string Seed;
	public bool UseRandomSeed;

	[Range(0,100)]
	public int RandomFillPercent; //how much will be filled initially with holes

	//tile = 0 in the map: empty tile
	//tile = 1 in the map: occupied by a wall
	int[,] map;

	void Start(){
		GenerateMap ();	
	}

	void Update(){
		if (Input.GetMouseButtonDown(0)) {
			GenerateMap ();
		}
	}

	void GenerateMap(){
		map = new int[Width, Height];
		RandomFillMap ();
		int numberOfPermutations = 5;
		for (int i = 0; i < numberOfPermutations; i++) {
			SmoothMap ();
		}
	}

	//to fill the map while random of 0 and 1
	void RandomFillMap(){
		if (UseRandomSeed) {
			Seed = Time.time.ToString ();
		}

		System.Random numberGenerator = new System.Random(Seed.GetHashCode());
		bool isBorderTile = false;
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				//the tiles around of the border of the map are always walls
				isBorderTile = x == 0 || x == Width-1 || y == 0 || y == Height-1;
				if (isBorderTile) {
					map [x, y] = 1; //wall
				}
				else {
					map [x, y] = numberGenerator.Next (0, 100) < RandomFillPercent ? 1 : 0;
				}

			}
		}
	}

	void SmoothMap(){
		int neighbourWallTiles = 0;
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				neighbourWallTiles = GetSurroundingWallCount (x, y);
				if (neighbourWallTiles == 4) {
					continue;
				}
				map [x, y] = neighbourWallTiles > 4? 1: 0;
			}
		}
	}

	//How many neighbours are walls?
	int GetSurroundingWallCount(int gridX, int gridY){
		int wallCount = 0;
		//looping a 3x3 grid center on (gridX, gridY): the 8-neighbourhood
		bool isBorderTile = false;
		for (int neighbourX = gridX - 1; neighbourX < gridX+2; neighbourX++) {
			for (int neighbourY = gridY -1; neighbourY < gridY+2; neighbourY++) {
				isBorderTile = neighbourX <= 0 || neighbourX >= Width || neighbourY <= 0
					|| neighbourY >= Height;
				if (isBorderTile) {
					wallCount++;
				} else {
					wallCount += map [neighbourX, neighbourY];//the value of the map is either 0 or 1 (one for a wall => will increment by 1)
				}
			}
		}
		wallCount -= map [gridX, gridY];//the current tile itself should not be included in the count
		return wallCount;
	}

	void OnDrawGizmos(){
		if (map == null)
			return; 
		float halfWidth = Width * .5f + .5f;
		float halfHeight = Height * .5f + .5f;
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				Gizmos.color = map [x, y] == 1 ? Color.black : Color.white;
				//position = -Width/2 + x + .5f, 0, -Height/2 + y + .5f);
				Vector3 position = new Vector3 (-halfWidth + x, 0, -halfHeight + y);
				Gizmos.DrawCube (position, new Vector3 (1, 1, 1));
			}
		}
	}
}
