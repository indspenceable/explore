using UnityEngine;
using System.Collections;

public class RoomContainer : MonoBehaviour {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float dist = 1f;

	// Update is called once per frame
	void Update () {
		float px = player.transform.position.x + (player.facingLeft ? -dist : dist);
		float py = player.transform.position.y;
		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(px, py, myCamera.transform.position.z), Time.deltaTime * lerpWeight);
		if (Input.GetKeyDown(KeyCode.T)) {
			GameplayPausable.paused = !GameplayPausable.paused;
		}
	}
}
