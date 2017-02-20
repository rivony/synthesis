using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//2D array of squares
public class SquareGrid {
	public Square[,] squares;

	public SquareGrid (int[,] map, float squareSize)
	{
		int nodeCountX = map.GetLength (0);
		int nodeCountY = map.GetLength (1);
		float mapWidth = nodeCountX * squareSize;
		float mapHeight = nodeCountY * squareSize;

		ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

		var farLeft = -mapWidth * .5f; //-mapWidth/2
		var centered = squareSize * .5f; //divided by 2
		var farTop = -mapHeight * .5f; //-mapHeight/2

		Vector3 position;
		bool isWall = false;

		for (int x = 0; x < nodeCountX; x++) {
			for (int y = 0; y < nodeCountY; y++) {
				//1. Calculate the position of the current controlNode
				position = new Vector3( farLeft + x * squareSize + centered, 0, 
					farTop + y * squareSize + centered);
				isWall = map [x, y] == 1;
				controlNodes [x, y] = new ControlNode (position, isWall, squareSize);
			}
		}

		squares = new Square[nodeCountX - 1, nodeCountY - 1];
		for (int x = 0; x < nodeCountX - 1; x++) {
			for (int y = 0; y < nodeCountY - 1; y++) {
				squares[x, y] = new Square(
					controlNodes[x, y+1], // (x:left, y+1:above) => topleft
					controlNodes[x+1, y+1], // (x+1:right, y+1:above) => topright
					controlNodes[x, y], // (x:left, y:bottom) => bottomLeft
					controlNodes[x+1, y]); // (x+1:right, y:bottom) => bottomRight
			}
		}
	}
}
