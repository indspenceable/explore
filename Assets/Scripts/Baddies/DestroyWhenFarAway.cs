using UnityEngine;
using System.Collections;

public class DestroyWhenFarAway : MonoBehaviour, IActivatableObject {
	private Level currentLevel;
	public void Activate(Level l) {
		currentLevel = l;
	}

	void Update() {
//		Debug.Log(currentLevel.BottomBorder());
//		Debug.Log(transform.position.y);
		if (currentLevel.BottomBorder() > transform.position.y) {
			Destroy(gameObject);
		}
	}
}
