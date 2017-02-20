using UnityEngine;

[CreateAssetMenu()]
public class UpdableData : ScriptableObject {

	public event System.Action OnValuesUpdated;
	public bool AutoUpdate;

	protected virtual void OnValidate(){
		if (AutoUpdate) {
			UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
		}
	}

	public void NotifyOfUpdatedValues(){
		UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
		if (OnValuesUpdated != null) {
			OnValuesUpdated ();
		}
	}

}
