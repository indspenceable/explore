using UnityEngine;
using System.Collections;
[RequireComponent(typeof(PlayerMovement))]
public class MagnetPullMove : MonoBehaviour {
	public LayerMask magnetMask;
	public float magnetDistance = 1f;
	public float weight = 30f;

	public SpriteRenderer sprite;
	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, magnetDistance);
	}
	void Update () {
		bool shouldEnable = false;
		if (Input.GetAxis("Magnet") > 0) {
			Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
			if (collision != null) {
				GetComponent<PlayerMovement>().ApplyMagnet(collision.transform.position, Input.GetAxis("Magnet") * weight);
				shouldEnable = true;
			}
		} 
		sprite.enabled = shouldEnable;
	}
}
