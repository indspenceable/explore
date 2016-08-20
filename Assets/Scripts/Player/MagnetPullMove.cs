using UnityEngine;
using System.Collections;
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInputManager))]
public class MagnetPullMove : MonoBehaviour {
	public LayerMask magnetMask;
	public float magnetDistance = 1f;
	public float weight = 30f;
	public SpriteRenderer sprite;
	public GameStateFlags currentGameState {
		get {
			return GetComponent<GameStateFlagsComponent>().state;
		}
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, magnetDistance);
		Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
		if (collision != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + (collision.transform.position - transform.position).normalized);
		}
	}

	private PlayerInputManager inputManager;

	void Start() {
		inputManager = GetComponent<PlayerInputManager>();
	}

	void Update () {
		if (!currentGameState.magnetEnabled) {
			return;
		}
		bool spriteEnabledState = false;
		if (inputManager.GetAxis("MagnetPositive") > 0) {
			Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
			if (collision != null) {
				
				GetComponent<PlayerMovement>().ApplyMagnet(collision.transform.position, collision.GetComponent<Magnet>().polarity*inputManager.GetAxis("MagnetPositive") * weight);
				spriteEnabledState = true;
			}
		}

		sprite.enabled = spriteEnabledState;
	}
}
