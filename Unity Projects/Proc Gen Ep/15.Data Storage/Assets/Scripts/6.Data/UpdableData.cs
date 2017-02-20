using UnityEngine;

[CreateAssetMenu()]
public class UpdableData : ScriptableObject {

	public event System.Action OnValuesUpdated;
	public bool AutoUpdate;

	public void NotifyOfUpdatedValues(){
		if (OnValuesUpdated != null) {
			OnValuesUpdated ();
		}
	}

	protected virtual void OnValidate(){
		if (AutoUpdate) {
			NotifyOfUpdatedValues ();
		}
	}
}
