using UnityEngine;

public class MeshData {
	public Vector3[] Vertices {
		get;
		set;
	}

	public int[] Triangles {
		get;
		set;
	}

	public Vector2[] UVs {
		get;
		set;
	}

	private int triangleIndex;

	public MeshData (int meshWidth, int meshHeight)
	{
		//Vertices
		Vertices = new Vector3[meshWidth * meshHeight];
		//Triangles
		int numberOfTriangles = (meshWidth - 1) * (meshHeight - 1) * 6;
		Triangles = new int[numberOfTriangles];
		//UVs
		UVs = new Vector2[meshWidth * meshHeight]; // to add texture to the mesh: we need, for each vertex so width * height
		// tell each vertex where it is in relation to the rest of the map as a percentage for both the x and the y axis
		//it is a percentage between 0 and 1
	}

	public void AddTriangle(int a, int b, int c){
		Triangles [triangleIndex] = a;
		Triangles [triangleIndex + 1] = b;
		Triangles [triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	//getting the mesh from the mesh data
	public Mesh CreateMesh(){
		Mesh mesh = new Mesh ();
		mesh.vertices = Vertices;
		mesh.triangles = Triangles;
		mesh.uv = UVs;
		mesh.RecalculateNormals ();//for the lighting to work out
		return mesh;
	}
}
