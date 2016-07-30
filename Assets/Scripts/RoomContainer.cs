using UnityEngine;
using System.Collections;

public class RoomContainer : GameplayPausable {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float dist = 1f;

	public override void Update() {
		base.Update();
		if (Input.GetKeyDown(KeyCode.T)) {
			if (Time.timeScale == 0f) {
				Time.timeScale = 1f;
			} else {
				Time.timeScale = 0f;
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
