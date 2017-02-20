using UnityEngine;

public class MeshData {
	Vector3[] Vertices {
		get;
		set;
	}

	int[] Triangles {
		get;
		set;
	}

	Vector2[] UVs {
		get;
		set;
	}

	Vector3[] m_BakedNormals;


	Vector3[] _borderVertices;
	int[] _borderTriangles;

	private int triangleIndex;
	int _borderTriangleIndex;

	bool m_UseFlatShading;

	public MeshData (int verticesPerLine, bool useFlatShading)
	{
		m_UseFlatShading = useFlatShading;

		int squaredVerticesPerLine = verticesPerLine * verticesPerLine;
		Vertices = new Vector3[squaredVerticesPerLine];
		int numberOfTriangles = (verticesPerLine - 1) * (verticesPerLine - 1) * 6;
		Triangles = new int[numberOfTriangles];
		UVs = new Vector2[squaredVerticesPerLine]; // to add texture to the mesh: we need, for each vertex so width * height
		// tell each vertex where it is in relation to the rest of the map as a percentage for both the x and the y axis
		//it is a percentage between 0 and 1

		_borderVertices = new Vector3[verticesPerLine * 4 + 4]; //the four corners and the four sides of the square
		_borderTriangles = new int[24 * verticesPerLine];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex){
		if (vertexIndex < 0) {//ie if it is a border vertex
			_borderVertices[-vertexIndex -1] = vertexPosition;

		} else {
			Vertices [vertexIndex] = vertexPosition;
			UVs [vertexIndex] = uv;
		}
	}

	public void AddTriangle(int a, int b, int c){
		if (a < 0 || b < 0 || c < 0) {
			_borderTriangles [_borderTriangleIndex] = a;
			_borderTriangles [_borderTriangleIndex + 1] = b;
			_borderTriangles [_borderTriangleIndex + 2] = c;
			_borderTriangleIndex += 3;
		} else {
			Triangles [triangleIndex] = a;
			Triangles [triangleIndex + 1] = b;
			Triangles [triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}

	public void ProcessMesh(){
		if (m_UseFlatShading) {
			FlatShading ();
		} else {
			BakeNormals ();
		}
	}

	void BakeNormals(){
		m_BakedNormals = CalculateNormals ();
	}

	void FlatShading(){
		int trianglesLength = Triangles.Length;
		Vector3[] flatShadedVertices = new Vector3[trianglesLength];
		Vector2[] flatShadedUVs = new Vector2[trianglesLength];

		int vertexIndex;
		for (int i = 0; i < trianglesLength; i++) {
			vertexIndex = Triangles [i];
			flatShadedVertices [i] = Vertices [vertexIndex];
			flatShadedUVs [i] = UVs[vertexIndex];
			Triangles [i] = i;
		}

		Vertices = flatShadedVertices;
		UVs = flatShadedUVs;
	}

	//getting the mesh from the mesh data
	public Mesh CreateMesh(){
		Mesh mesh = new Mesh ();
		mesh.vertices = Vertices;
		mesh.triangles = Triangles;
		mesh.uv = UVs;
		if (m_UseFlatShading) {
			mesh.RecalculateNormals ();
		} else {
			mesh.normals = m_BakedNormals;
		}

		return mesh;
	}

	Vector3[] CalculateNormals(){
		Vector3[] vertexNormals = new Vector3[Vertices.Length];

		int triangleCount = Triangles.Length / 3;

		int normalTriangleIndex;
		int vertexIndexA;
		int vertexIndexB;
		int vertexIndexC;

		for (int numberOfTriangle = 0; numberOfTriangle < triangleCount; numberOfTriangle++) {
			normalTriangleIndex = numberOfTriangle * 3;
			vertexIndexA = Triangles [normalTriangleIndex];
			vertexIndexB = Triangles [normalTriangleIndex + 1];
			vertexIndexC = Triangles [normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, 
				vertexIndexC);
			vertexNormals [vertexIndexA] += triangleNormal;
			vertexNormals [vertexIndexB] += triangleNormal;
			vertexNormals [vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = _borderTriangles.Length / 3;

		for (int numberOfTriangle = 0; numberOfTriangle < borderTriangleCount; numberOfTriangle++) {
			normalTriangleIndex = numberOfTriangle * 3;
			vertexIndexA = _borderTriangles [normalTriangleIndex];
			vertexIndexB = _borderTriangles [normalTriangleIndex + 1];
			vertexIndexC = _borderTriangles [normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, 
				vertexIndexC);

			if (vertexIndexA >= 0) {
				vertexNormals [vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0) {
				vertexNormals [vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0) {
				vertexNormals [vertexIndexC] += triangleNormal;
			}

		}


		//normalization
		for (int i = 0; i < vertexNormals.Length; i++) {
			vertexNormals [i].Normalize ();
		}

		return vertexNormals;
	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC){
		Vector3 pointA = (indexA < 0)? _borderVertices[-indexA -1]: Vertices [indexA];
		Vector3 pointB = (indexB < 0)? _borderVertices[-indexB -1]: Vertices [indexB];
		Vector3 pointC = (indexC < 0)? _borderVertices[-indexC -1]: Vertices [indexC];

		//cross product of two vectors AB x AC
		Vector3 ab = pointB - pointA;
		Vector3 ac = pointC - pointA;

		return Vector3.Cross (ab, ac).normalized;
	}

	Vector3 SurfaceNormalFromIndicesImproved(int indexA, int indexB, int indexC){
		Vector3 result = Vector3.Cross (
			Vertices [indexB] - Vertices [indexA], 
			Vertices [indexC] - Vertices [indexA]);
		
		return result.normalized;
	}
}
