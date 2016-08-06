using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextContainer : MonoBehaviour {
	public Text dialogueText;

	public void DisplayText(string text) {
		gameObject.SetActive(true);
		dialogueText.text = text;
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
}
