using UnityEngine;
using System.Collections;

public class DoorCollider : MonoBehaviour {
	public DoorMap.Direction direction;

	public void OnTriggerEnter2D(Collider2D other) {
		if (!gameObject.activeSelf) {
			return;
		}
		if (other && other.gameObject.layer == LayerMask.NameToLayer("Player") ||
		   (other.transform.parent && 
				other.transform.parent.gameObject.layer == LayerMask.NameToLayer("Player"))) {
			GameManager gm = GameManager.instance;

			gameObject.SetActive(false);
			gm.StartCoroutine(gm.DoorCollision(this));
		}
	}
}
