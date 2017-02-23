using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

	Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>(); 

	static PoolManager _instance;

	public static PoolManager Instance{
		get{ 
			if (_instance == null) {
				_instance = FindObjectOfType<PoolManager> ();
			}
			return _instance;
		}
	}

	//Create a new pool
	public void CreatePool(GameObject prefab, int poolSize){
		int poolKey = prefab.GetInstanceID ();//unique integer for every GameObject

		if (!_poolDictionary.ContainsKey(poolKey)) {
			_poolDictionary.Add (poolKey, new Queue<GameObject> ());

			for (int i = 0; i < poolSize; i++) {
				GameObject newObject = Instantiate (prefab) as GameObject;
				newObject.SetActive (false);
				_poolDictionary [poolKey].Enqueue (newObject);
			}

		}
	}

	public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation){
		int poolKey = prefab.GetInstanceID ();

		if (_poolDictionary.ContainsKey(poolKey)) {
			GameObject objectToReuse = _poolDictionary [poolKey].Dequeue();
			_poolDictionary [poolKey].Enqueue (objectToReuse);

			objectToReuse.SetActive (true);
			objectToReuse.transform.position = position;
			objectToReuse.transform.rotation = rotation;
		}
	}
}
