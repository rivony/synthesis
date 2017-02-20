using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdableData), true)]
public class UpdatableDataEditor : Editor {

	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		var updatabledata = (UpdableData)target;
		if (GUILayout.Button("Update")) {
			updatabledata.NotifyOfUpdatedValues ();
		}
	}
}
