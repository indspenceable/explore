using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashText : MonoBehaviour {
	public float period = 2f;
	public Image image;
	void Start () {
		image = GetComponent<Image>();
		StartCoroutine(Flash());
	}
	public IEnumerator Flash() {
		while (true) {
			float dt = 0f;
			image.enabled = false;
			while (dt < period/2) {
				yield return null;
				dt += Time.deltaTime;
			}
			image.enabled = true;
			dt = 0f;
			while (dt < period/2) {
				yield return null;
				dt += Time.deltaTime;
			}
		}
	}
}
