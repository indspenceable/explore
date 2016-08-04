using UnityEngine;
using System.Collections;

public class FallingHazard : MonoBehaviour {
	public float width = 2;
	public float maxDistance = 5f;
	public LayerMask playerMask;
	public LayerMask levelMask;
	public float gravity = 30f;

	private bool startedFalling = false;

	void Update() {
		if (!startedFalling && CheckForPlayer()) {
			startedFalling = true;
			StartCoroutine(Fall());
		}
	}

	private bool CheckForPlayer() {
		LayerMask mask = playerMask.value | levelMask.value;
		RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(width, 0.25f), 0, new Vector2(0, -1), maxDistance, playerMask);
		if (hit) {
			if (hit.collider.GetComponent<PlayerMovement>() != null) {
				return true;
			}
		}
		return false;
	}

	private bool CheckForHitGround() {
		return Physics2D.BoxCast(transform.position + Vector3.down*0.25f, new Vector2(0.75f, 0.25f), 0, Vector2.down, 0.1f, levelMask);
	}

	public IEnumerator Fall() {
		float vy = -5;
		while (!CheckForHitGround()) {
			yield return null;
			vy -= gravity * Time.deltaTime;
			transform.Translate(Vector3.up * vy * Time.deltaTime, Space.World);
		}

		// TODO shatter!
		ScoredHit();
	}

	public void ScoredHit() {
		Destroy(gameObject);
	}
}
