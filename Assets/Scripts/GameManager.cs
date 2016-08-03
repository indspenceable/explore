using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : GameplayPausable {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float dist = 1f;
	public GameObject pausedTextContainer;
	public float minDistanceThreshold = 0.02f;

	void Start () {
		GetComponent<AudioSource>().Play();
		GoToPlayer();
	}

	public override void Update() {
		base.Update();
		if (Input.GetButtonDown("Pause")) {
			if (Time.timeScale == 0f) {
				GetComponent<AudioSource>().UnPause();
				Time.timeScale = 1f;
				pausedTextContainer.SetActive(false);
			} else {
				GetComponent<AudioSource>().Pause();
				Time.timeScale = 0f;
				pausedTextContainer.SetActive(true);
			}
		}
	}

	public void GoToPlayer() {
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, myCamera.transform.position.z);
	}

	// Update is called once per frame
	public override void UnpausedUpdate () {
		float px = player.transform.position.x + (player.facingLeft ? -dist : dist);
		float py = player.transform.position.y;
		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(px, py, myCamera.transform.position.z), Time.deltaTime * lerpWeight);
		if (Mathf.Abs(myCamera.transform.position.x - player.transform.position.x) + Mathf.Abs(myCamera.transform.position.y - player.transform.position.y) < minDistanceThreshold) {
			GoToPlayer();
		}
	}
}
