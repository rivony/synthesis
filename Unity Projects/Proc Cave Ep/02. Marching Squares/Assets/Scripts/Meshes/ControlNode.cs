using UnityEngine;

public class ControlNode : Node {
	public bool Active; //true if it is a wall; false otherwise
	public Node AboveNode;
	public Node RightNode;

	public ControlNode (Vector3 position, bool active, float squareSize): base(position)
	{
		Active = active;
		var halfSquareSize = squareSize * .5f;
		AboveNode = new Node (position + Vector3.forward  * halfSquareSize);
		RightNode = new Node (position + Vector3.right * halfSquareSize);
	}
}
