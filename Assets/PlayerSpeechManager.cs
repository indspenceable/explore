using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSpeechManager : MonoBehaviour {
	PlayerInputManager inputManager;
	string pendingSpeech;
	const int MAX_SPEECH = 10;

	public Text speechUIText;

	public void Start() {
		inputManager = GetComponent<PlayerInputManager>();
	}


	public void Update() {
		if (GameManager.instance.currentGameMode == GameMode.SPEECH) {
			if (Input.inputString == "\n") {
				// We're done here.
				GameManager.instance.currentGameMode = GameMode.MOVEMENT;
				speechUIText.gameObject.SetActive(false);
			} else if (Input.inputString == "\b") {
				if (pendingSpeech.Length > 0) {
					pendingSpeech = pendingSpeech.Substring(0, pendingSpeech.Length-1);
				}
			} else if (pendingSpeech.Length < MAX_SPEECH) {
				pendingSpeech += Input.inputString;
			}
			speechUIText.text = pendingSpeech;
		} else {
			if (inputManager.GetButtonDown("Speech", GameMode.MOVEMENT)) {
				GameManager.instance.currentGameMode = GameMode.SPEECH;
				speechUIText.gameObject.SetActive(true);
				pendingSpeech = "";
				speechUIText.text = "";
			}
		}
	}
}
