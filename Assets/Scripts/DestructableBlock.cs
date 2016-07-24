using UnityEngine;
using System.Collections;

public class DestructableBlock : MonoBehaviour {
	public void MeleeHit() {
		Destroy(gameObject);
	}
}
