using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VerticalMovement))]
[RequireComponent(typeof(HorizontalMovement))]
public class HopperControls : MonoBehaviour, IActivatableObject {
	private VerticalMovement vert;
	private HorizontalMovement horiz;
	public float gravity = 30f;
	public float waitTime = 2f;
	public float jumpStrength = 10f;
	public float jumpVelocity = 5f;

	// Use this for initialization
	public void Activate() {
		vert = GetComponent<VerticalMovement>();
		horiz = GetComponent<HorizontalMovement>();
		horiz.vx = jumpStrength;
		StartCoroutine(Fall());
	}

	private IEnumerator Fall() {
		vert.vy = 0;
		while (! vert.CheckGrounded()) {
			vert.vy -= gravity * GameManager.instance.ActiveGameDeltaTime;
			vert.RiseOrFall(GameManager.instance.ActiveGameDeltaTime * vert.vy);
			yield return null;
		}
		StartCoroutine(Jump());
	}

	private IEnumerator Jump() {
		while (true) {
			if (vert.CheckGrounded()) {
				float dt = 0;

				while (dt < waitTime) {
					dt += GameManager.instance.ActiveGameDeltaTime;
					yield return null;
					if (!vert.CheckGrounded()) {
						StartCoroutine(Fall());
						yield break;
					}
				}
				vert.vy = jumpStrength;

			} else {
				vert.vy -= gravity * GameManager.instance.ActiveGameDeltaTime;
				vert.RiseOrFall(GameManager.instance.ActiveGameDeltaTime * vert.vy);
				float startingVx = horiz.vx;
				horiz.MoveLeftOrRight(GameManager.instance.ActiveGameDeltaTime * horiz.vx);
				if (horiz.vx == 0f) {
					horiz.vx = -startingVx;
				}
			}
			yield return null;
		}
	}
}
