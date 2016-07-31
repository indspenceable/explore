using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class RoomContainer : GameplayPausable {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float dist = 1f;
	public GameObject pausedTextContainer;

	void Start () {
		GetComponent<AudioSource>().Play();
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

	// Update is called once per frame
	public override void UnpausedUpdate () {
		float px = player.transform.position.x + (player.facingLeft ? -dist : dist);
		float py = player.transform.position.y;
		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(px, py, myCamera.transform.position.z), Time.deltaTime * lerpWeight);
	}
}
