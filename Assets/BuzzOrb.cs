using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzOrb : MonoBehaviour, IActivatableObject {
	public float TimeOff=4f;
	public float TimeOn=1f;
	public GameObject spike;
	public void Activate(Level l){
		StartCoroutine(ChangeSize());
	}

	private IEnumerator ChangeSize() {
		float dt;
		while (true){
			dt = 0;
			while (dt < TimeOff) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.transform.localScale = Vector3.one * 2;
			yield return null;
			dt = 0;
			while (dt < TimeOn) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.transform.localScale = Vector3.one;
			yield return null;
		}
	}
}
