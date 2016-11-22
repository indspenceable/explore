using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VerticalMovement))]
public class StationaryHopperController : MonoBehaviour, IActivatableObject {
	private VerticalMovement vert;
	public float gravity = 30f;
	public float waitTime = 2f;
	public float jumpStrength = 10f;
	public float jumpVelocity = 5f;

	// Use this for initialization
	public void Activate() {
		vert = GetComponent<VerticalMovement>();
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
			}
			yield return null;
		}
	}
}
