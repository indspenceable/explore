using UnityEngine;
using System.Collections;

public class DoorCollider : MonoBehaviour {
	public GameManager.Direction direction;

	public void OnTriggerEnter2D(Collider2D other) {
		if (other && other.gameObject.layer == LayerMask.NameToLayer("Player") ||
		   (other.transform.parent && 
				other.transform.parent.gameObject.layer == LayerMask.NameToLayer("Player"))) {
			GameManager gm = GameManager.instance;
			gm.StartCoroutine(gm.DoorCollision(this));
		}
	}
}
