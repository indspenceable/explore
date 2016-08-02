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
		Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
		if (collision != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + (collision.transform.position - transform.position).normalized);
		}
	}

	void Update () {
		bool shouldEnable = false;
		if (Input.GetAxis("MagnetPositive") > 0) {
			Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
			if (collision != null) {
				
				GetComponent<PlayerMovement>().ApplyMagnet(collision.transform.position, collision.GetComponent<Magnet>().polarity*Input.GetAxis("MagnetPositive") * weight);
				shouldEnable = true;
			}
		} else if (Input.GetAxis("MagnetNegative") > 0) {
			Collider2D collision = Physics2D.OverlapCircle(transform.position, magnetDistance, magnetMask); 
			if (collision != null) {

				GetComponent<PlayerMovement>().ApplyMagnet(collision.transform.position, -1*collision.GetComponent<Magnet>().polarity*Input.GetAxis("MagnetNegative") * weight);
				shouldEnable = true;
			}
		}

		sprite.enabled = shouldEnable;
	}
}
