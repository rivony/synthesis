using UnityEngine;

public class MeshGenerator : MonoBehaviour {

	public SquareGrid MySquareGrid;

	public void GenerateMesh(int[,] map, float squareSize){
		MySquareGrid = new SquareGrid (map, squareSize);
	}

	void OnDrawGizmos(){
		if (MySquareGrid == null) {
			return;
		}

		var gridWidth = MySquareGrid.squares.GetLength (0);
		var gridHeight = MySquareGrid.squares.GetLength (1);

		Square currentSquare;
		Vector3 cubeSize;

		for (int x = 0; x < gridWidth; x++) {
			for (int y = 0; y < gridHeight; y++) {

				currentSquare = MySquareGrid.squares [x, y];

				cubeSize = Vector3.one * .4f;

				Gizmos.color = currentSquare.TopLeft.Active ? Color.black : Color.white;
				Gizmos.DrawCube (currentSquare.TopLeft.Position, cubeSize);

				Gizmos.color = currentSquare.TopRight.Active ? Color.black : Color.white;
				Gizmos.DrawCube (currentSquare.TopRight.Position, cubeSize);

				Gizmos.color = currentSquare.BottomLeft.Active ? Color.black : Color.white;
				Gizmos.DrawCube (currentSquare.BottomLeft.Position, cubeSize);

				Gizmos.color = currentSquare.BottomRight.Active ? Color.black : Color.white;
				Gizmos.DrawCube (currentSquare.BottomRight.Position, cubeSize);

				//Check the midset position
				cubeSize = Vector3.one * .15f;

				Gizmos.color = Color.gray;
				Gizmos.DrawCube (currentSquare.CenterTopNode.Position, cubeSize);
				Gizmos.DrawCube (currentSquare.CenterBottomNode.Position, cubeSize);
				Gizmos.DrawCube (currentSquare.CenterRightNode.Position, cubeSize);
				Gizmos.DrawCube (currentSquare.CenterLeftNode.Position, cubeSize);

			}
		}
	}
}
