using UnityEngine;
using System.Collections;
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInputManager))]
public class MagnetPullMove : MonoBehaviour {
	public LayerMask magnetMask;
	public float magnetDistance = 1f;
	public float weight = 30f;
	public SpriteRenderer sprite;
	public SerailizableGameState currentGameState {
		get {
			return GetComponent<SerailizableGameStateComponent>().state;
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
		if (!currentGameState.enabled(GameStateFlag.MAGNET)) {
			return;
		}
		bool spriteEnabledState = false;
		if (inputManager.GetAxis("MagnetPositive", GameMode.MOVEMENT) > 0
			|| inputManager.GetButton("MagnetPositive", GameMode.MOVEMENT) ) {
			Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
			if (collision != null) {
				GetComponent<PlayerMovement>().ApplyMagnet(collision.transform.position, collision.GetComponent<Magnet>().polarity * 1 * weight);
				spriteEnabledState = true;
			}
		}

		sprite.enabled = spriteEnabledState;
	}
}
