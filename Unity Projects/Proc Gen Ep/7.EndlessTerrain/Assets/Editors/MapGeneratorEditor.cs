using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneratorBehaviour))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI(){
		MapGeneratorBehaviour mapGen = (MapGeneratorBehaviour)target;

		if(DrawDefaultInspector ()){
			if (mapGen.AutoUpdate) {
				mapGen.GenerateMap ();
			}
		}

		if (GUILayout.Button("Generate")) {
			mapGen.GenerateMap ();
		}
	}
}
