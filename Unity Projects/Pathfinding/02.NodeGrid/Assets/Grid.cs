using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	
	Node[,] _grid;
	float _nodeDiameter;
	int _gridSizeX;
	int _gridSizeY;

	public Vector2 GridWorldSize; // area in the world coordinate that the grid is going to cover
	public float NodeRadius; // how much spaces each individual are covered
	public LayerMask UnwalkableMask;
	//public Transform Player;
	 
	void Start(){
		//how many nodes can we fit into our grid?
		_nodeDiameter = NodeRadius * 2;
		_gridSizeX = Mathf.RoundToInt (GridWorldSize.x / _nodeDiameter); //the nearest integer
		_gridSizeY = Mathf.RoundToInt (GridWorldSize.y / _nodeDiameter);
		CreateGrid ();

	}

	void CreateGrid ()
	{
		_grid = new Node[_gridSizeX, _gridSizeY];

		Vector3 worldBottomLeft = transform.position 
			- Vector3.right * GridWorldSize.x * .5f //left edge of the world
			- Vector3.forward * GridWorldSize.y * .5f; //bottom left corner

		for (int x = 0; x < _gridSizeX; x++) {
			for (int y = 0; y < _gridSizeY; y++) {
				//want to get the worldposition for each of the collision checks
				Vector3 worldPoint = worldBottomLeft 
					+ Vector3.right * (x * _nodeDiameter + NodeRadius) 
					+ Vector3.forward * (y * _nodeDiameter + NodeRadius);
				//Physics.CheckSphere returns true in case of collision
				bool walkable = ! Physics.CheckSphere (worldPoint, NodeRadius, UnwalkableMask); 
				_grid [x, y] = new Node (walkable, worldPoint);
			}
		}
	}

	/*
	* how far along the grid it is? convert the position into a percentage
	* for the x axis, 
	* if it is on the far left: percentage == 0, 
	* if it is on the middle: percentage == .5, 
	* if it is on the far right: percentage == 1
	*/
	public Node NodeFromWorldPoint(Vector3 worldPosition){
		//how far along the grid it is? convert the position into a percentage: 
		float percentX = (worldPosition.x + GridWorldSize.x * .5f) / GridWorldSize.x; //eg. if worldPosition.x == 0,  then  (0 + GridWorldSize.x * .5f) / GridWorldSize.x == .5f
		float percentY = (worldPosition.z + GridWorldSize.y * .5f) / GridWorldSize.y; //eg. if worldPosition.y == -GridWorldSize.y * .5f (far top) then 0 / GridWorldSize.y == 0; 
		//Clamp01: if the character is outside of the grid, no invalid index is got
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01 (percentY);

		var xFloat = (_gridSizeX - 1) * percentX; // if percentX==1 (far right of the grid) and there is not -1, then x = _gridSizeX, need to substract 1 to not be outside of the array  
		var yFloat = (_gridSizeY - 1) * percentY;

		int x = Mathf.RoundToInt (xFloat);
		int y = Mathf.RoundToInt (yFloat);

		return _grid [x, y];
	}

	void OnDrawGizmos(){
		Vector3 gridWorldSize3 = new Vector3 (GridWorldSize.x, 1, GridWorldSize.y);
		Gizmos.DrawWireCube (transform.position, gridWorldSize3);

		if (_grid != null) {
			//Node playerNode = NodeFromWorldPoint (Player.position);
			foreach (var node in _grid) {
				Gizmos.color = node.Walkable ? Color.white : Color.blue;
				/*
				  if (playerNode == node) {
					Gizmos.color = Color.red;
				}
				//*/
				Vector3 cubeSize = Vector3.one * (_nodeDiameter - .1f); //.1f for the space of outline around each node
				Gizmos.DrawCube (node.WorldPosition, cubeSize);
			}
		}
	}

}
