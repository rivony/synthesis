using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh (float[,] heightMap) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		MeshData meshData = new MeshData (width, height);
		int vertexIndex = 0; //to keep track where we are in our 1D array Vertices

		//To center the vertices at the center
		float topLeftX = (width -1)/ -2f; //do not forget the -1 and the f for -2f to yield a float result
		float topLeftZ = (height -1)/ 2f; // positive 2f because the z is positive when we go up (-1 0 1 => x = (width - 1)/(-2))


		float floatWidth = (float)width;
		float floatHeigth = (float)height;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				//Vertices
				meshData.Vertices [vertexIndex] = new Vector3 (x + topLeftX, heightMap [x, y], topLeftZ - y);
				//we don't want to move down from topLeftZ, we move down the y value

				//Triangles
				if (x < width -1 && y < height - 1) {// we ignore the vertices at the most rigth and the most bottom
					meshData.AddTriangle(vertexIndex, 
						vertexIndex + width + 1, 
						vertexIndex + width); // first triangle (i, i+w+1, i+w)
					
					meshData.AddTriangle (vertexIndex + width + 1, 
						vertexIndex, 
						vertexIndex + 1); // second triangle (i+w+1, i, i+1
				}

				// tell each vertex where it is in relation to the rest of the map as a percentage for both the x and the y axis
				//it is a percentage between 0 and 1
				meshData.UVs[vertexIndex] = new Vector2(x/floatWidth, y/floatHeigth);

				vertexIndex++;
			}
		}
		return meshData;//useful to threading instead of returning the mesh, we return the meshData
		//threading: cannot create meshes inside a thread
	}
}
