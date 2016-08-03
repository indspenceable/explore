using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VerticalMovement))]
[RequireComponent(typeof(HorizontalMovement))]
public class Roller : MonoBehaviour {
	private VerticalMovement vert;
	private HorizontalMovement horiz;
	public float gravity = 30f;
	public float fullSpeed = 5f;
	public float drag = 0.95f;
	public int direction = 1;

	// Use this for initialization
	void Start () {
		vert = GetComponent<VerticalMovement>();
		horiz = GetComponent<HorizontalMovement>();
		StartCoroutine(Fall());
	}

	private IEnumerator Fall() {
		vert.vy = 0;
		while (! vert.CheckGrounded()) {
			vert.vy -= gravity * Time.deltaTime;
			vert.RiseOrFall(Time.deltaTime * vert.vy);
			yield return null;
		}
		StartCoroutine(FullSpeedAhead());
	}

	private IEnumerator FullSpeedAhead() {
		horiz.vx = fullSpeed*direction;
		while (true) {
			vert.vy -= gravity * Time.deltaTime;
			vert.RiseOrFall(Time.deltaTime * vert.vy);

			float startingVx = horiz.vx;
			horiz.MoveLeftOrRight(Time.deltaTime * horiz.vx);
			if (horiz.vx == 0f) {
				horiz.vx = -startingVx;
				direction *= -1;
				if (vert.CheckGrounded()) {
					// we bounced;

					StartCoroutine(Bounce());
					yield break;
				}
				// Otherwise we bounced in midair,
			}

			yield return null;
		}
	}

	private IEnumerator Bounce() {
		vert.vy = 5f;
		while (Mathf.Abs(horiz.vx) > 0.1f) {
			// Bounce vertically
			vert.vy -= gravity * Time.deltaTime;
			float startingVy = vert.vy;
			vert.RiseOrFall(Time.deltaTime * vert.vy);
			if (vert.vy == 0f) {
				vert.vy = startingVy/2f;
			}

			float startingVx = horiz.vx;
			horiz.MoveLeftOrRight(Time.deltaTime * horiz.vx);
			if (horiz.vx == 0f) {
				horiz.vx = -startingVx;
			}
			if (vert.CheckGrounded()) {
				horiz.vx *= 0.99f;
			}
			yield return null;
		}
		yield return new WaitForSeconds(3f);
		yield return FullSpeedAhead();
	}


}
