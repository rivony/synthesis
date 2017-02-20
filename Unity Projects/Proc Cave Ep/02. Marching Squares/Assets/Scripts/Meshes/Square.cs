using UnityEngine;

public class Square {
	public ControlNode TopLeft;
	public ControlNode TopRight;
	public ControlNode BottomLeft;
	public ControlNode BottomRight;

	public Node CenterTopNode;
	public Node CenterBottomNode;
	public Node CenterLeftNode;
	public Node CenterRightNode;

	public Square (ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
	{
		TopLeft = topLeft;
		TopRight = topRight;
		BottomLeft = bottomLeft;
		BottomRight = bottomRight;

		CenterTopNode = topLeft.RightNode;
		CenterBottomNode = bottomLeft.RightNode;
		CenterLeftNode = bottomLeft.AboveNode;
		CenterRightNode = bottomRight.AboveNode;
	}
}
