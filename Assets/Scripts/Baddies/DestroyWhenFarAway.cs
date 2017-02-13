using UnityEngine;
using System.Collections;

public class DestroyWhenFarAway : MonoBehaviour, IActivatableObject {
	private Level currentLevel;
	public void Activate(Level l) {
		currentLevel = l;
	}

	void Update() {
		if (!currentLevel) {
			return;
		}
		if (currentLevel.BottomBorder() > transform.position.y) {
			Destroy(gameObject);
		}
	}
}
