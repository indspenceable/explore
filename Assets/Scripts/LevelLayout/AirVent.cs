using UnityEngine;
using System.Collections;

public class AirVent : MonoBehaviour {
	public bool changeYVelocity;
	public float yVelocityFactor;
	public bool changeXVelocity;
	public float xVelocityFactor;

	public void OnTriggerStay2D(Collider2D other) {
		// other should be player
		PlayerMovement player = other.GetComponent<PlayerMovement>();
		if (player != null && !player.currentlyPerformingAirDodge) {
			if (changeYVelocity) {
				player.vert.vy += (player.gravityStrength + yVelocityFactor) * GameManager.instance.ActiveGameDeltaTime;
				player.initiatedJump = false;
			}
			if (changeXVelocity) {
				player.horiz.vx += xVelocityFactor * GameManager.instance.ActiveGameDeltaTime;
			}
		}
	}
}
