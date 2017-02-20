using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverBehaviour : MonoBehaviour {

	public GameObject GameOverScreen;
	public Text SecondsSurvivedUI;
	bool isGameOver;

	// Use this for initialization
	void Start () {
		FindObjectOfType<PlayerBehaviour> ().OnPlayerDeath += OnGameOver;
	}
	
	// Update is called once per frame
	void Update () {
		if (isGameOver) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				SceneManager.LoadScene (0); //0: Index of the first scene
			}
		}
	}

	public void OnGameOver(){
		GameOverScreen.SetActive (true);
		SecondsSurvivedUI.text = Mathf.RoundToInt(Time.timeSinceLevelLoad).ToString();
		isGameOver = true;
	}
}
