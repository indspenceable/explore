using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float minDistanceThreshold = 0.02f;
	public GameObject pausedTextContainer;
	public TextContainer dialogues;

	public static bool paused = false;

	[SerializeField]
	public List<Level> levels;

	// SINGLETON
	public static GameManager instance;
	void Start () {
		if (instance != null) {
			Debug.LogError("Can't have multiple Game Managers.");
		}
		instance = this;
		GetComponent<AudioSource>().Play();
		GoToPlayer();
	}

	bool oldEnabled;
	float oldTimeScale;

	public void Update() {
		float px = player.transform.position.x;
		float py = player.transform.position.y;
		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(px, py, myCamera.transform.position.z), Time.deltaTime * lerpWeight);
		if (Mathf.Abs(myCamera.transform.position.x - player.transform.position.x) + Mathf.Abs(myCamera.transform.position.y - player.transform.position.y) < minDistanceThreshold) {
			GoToPlayer();
		}

		if (Input.GetButtonDown("Pause")) {
			if (paused) {
				GetComponent<AudioSource>().UnPause();
				Time.timeScale = oldTimeScale;
				pausedTextContainer.SetActive(false);
				player.enabled = oldEnabled;
				paused = false;
			} else {
				GetComponent<AudioSource>().Pause();
				oldTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				pausedTextContainer.SetActive(true);
				oldEnabled = player.enabled;
				player.enabled = false;
				paused = true;
			}
		}
	}

	public void GoToPlayer() {
		myCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, myCamera.transform.position.z);
	}

	public IEnumerator Read(string text) {
		
		bool oldEnabled = GameManager.instance.player.enabled;
		float oldTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		player.enabled = false;

		yield return null;
		dialogues.DisplayText(text);
		while (!Input.GetButtonDown("Interact")) {
			yield return null;
		}
		dialogues.Hide();

		Time.timeScale = oldTimeScale;
		player.enabled = oldEnabled;
	}


	#if UNITY_EDITOR
	public SharedLevelEditingStuff shared = new SharedLevelEditingStuff();
	#endif
}
