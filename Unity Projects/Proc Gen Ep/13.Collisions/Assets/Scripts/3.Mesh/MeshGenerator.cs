using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh (float[,] heightMap, float heightMultiplier,
		AnimationCurve heightCurve, int levelOfDetail) {

		//cf meshSize = borderedSize - 2
		int borderedSize = heightMap.GetLength (0);

		int meshSimplificationIncrement = levelOfDetail.Equals(0)? 1: levelOfDetail * 2;
		int meshSize = borderedSize - 2 * meshSimplificationIncrement;
		int meshSizeUnsimplified = borderedSize - 2; //use to calculate topleftX and topLeftZ


		int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

		MeshData meshData = new MeshData (verticesPerLine);

		//To center the vertices at the center
		float topLeftX = (meshSizeUnsimplified -1)/ -2f; //do not forget the -1 and the f for -2f to yield a float result
		float topLeftZ = (meshSizeUnsimplified -1)/ 2f; // positive 2f because the z goes up (-1 0 1 => x = (width - 1)/(-2))

		float floatMeshSize = (float)meshSize;

		float vertexX = 0f;
		float vertexY = 0f;
		float vertexZ = 0f;

		AnimationCurve aHeightCurve = new AnimationCurve (heightCurve.keys); //a great improvisation over the generation

		int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = - 1;

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement) {
				bool isBorderVertex = (y == 0 || y == borderedSize -1 || x == 0 || x == borderedSize - 1);
				if (isBorderVertex) {
					vertexIndicesMap [x, y] = borderVertexIndex;
					borderVertexIndex--;
				} else {
					vertexIndicesMap [x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}


		int vertexIndex;
		Vector3 vertexPosition;
		Vector2 percent;
		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement) {

				vertexIndex = vertexIndicesMap[x, y];
				// tell each vertex where it is in relation to the rest of the map as a percentage for both the x and the y axis
				//it is a percentage between 0 and 1
				percent = new Vector2(
					(x - meshSimplificationIncrement)/floatMeshSize, 
					(y - meshSimplificationIncrement)/floatMeshSize);

				vertexX = topLeftX + percent.x * meshSizeUnsimplified;
				vertexY = aHeightCurve.Evaluate(heightMap [x, y]) * heightMultiplier;
				vertexZ = topLeftZ - percent.y * meshSizeUnsimplified; //we don't want to move down from topLeftZ, we move down the y value
				vertexPosition = new Vector3 (vertexX, vertexY, vertexZ);

				meshData.AddVertex (vertexPosition, percent, vertexIndex);

				if (x < borderedSize - 1 && y < borderedSize - 1) {// we ignore the vertices at the most rigth and the most bottom
					int a = vertexIndicesMap [x, y];
					int b = vertexIndicesMap [x + meshSimplificationIncrement, y];
					int c = vertexIndicesMap [x, y + meshSimplificationIncrement];
					int d = vertexIndicesMap [x + meshSimplificationIncrement, y + meshSimplificationIncrement];

					meshData.AddTriangle (a, d, c); // first triangle (clock wise on a square)
					meshData.AddTriangle (d, a, b); // second triangle 
				}
				vertexIndex++;
			}
		}

		meshData.BakeNormals ();

		return meshData;//useful to threading instead of returning the mesh, we return the meshData
	}
}
