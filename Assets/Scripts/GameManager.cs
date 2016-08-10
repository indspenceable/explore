using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {
	public PlayerMovement player;
	public Camera myCamera;
	public float lerpWeight = 5f;
	public float minDistanceThreshold = 0.02f;
	public GameObject pausedTextContainer;
	public TextContainer dialogues;
	public Level currentLevel;

	public static bool paused = false;
	public static readonly Vector2 SCREEN_SIZE = new Vector2(16, 12);

	[SerializeField]
	public List<Level> levels;


	[System.Serializable]
	public class Door {
		public int x;
		public int y;
	}
	[SerializeField]
	public List<Door> HorizontalDoors = new List<Door>();
	private Door FindDoor(int x, int y) {
		foreach (var door in HorizontalDoors) {
			if (door.x == x && door.y == y) {
				return door;
			}
		}
		return null; 
	}
	public bool DoorAt(int x, int y) {
		return (FindDoor(x,y) != null);
	}
	public void SetDoor(int x, int y, bool shouldThereBe) {
		Door d = FindDoor(x, y);
		if (d == null && shouldThereBe) {
			d = new Door();
			d.x = x;
			d.y = y;
			HorizontalDoors.Add(d);
		} else if (d != null && !shouldThereBe) {
			HorizontalDoors.Remove(d);
		}
	}


	// SINGLETON
	public static GameManager instance;
	void Start () {
		instance = this;
		GetComponent<AudioSource>().Play();
		GoToTarget(findTargets());
	}

	bool oldEnabled;
	float oldTimeScale;

	Vector2 findTargets() {
		float halfWidth = SCREEN_SIZE.x / 2;
		float halfHeight = SCREEN_SIZE.y / 2;
		Vector2 scaledMap = Vector2.Scale(currentLevel.mapSize, SCREEN_SIZE);
		float px = Mathf.Clamp(player.transform.position.x, halfWidth, scaledMap.x-halfWidth);
		float py = Mathf.Clamp(player.transform.position.y, halfHeight, scaledMap.y-halfHeight);
		return new Vector2(px, py);
	}

	public void Update() {
		// find the target camera position. This involves keeping the camera within the bounds of the level.
		Vector2 targetP = findTargets();

		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(targetP.x, targetP.y, myCamera.transform.position.z), Time.deltaTime * lerpWeight);
		if (Mathf.Abs(myCamera.transform.position.x - targetP.x) + Mathf.Abs(myCamera.transform.position.y - targetP.y) < minDistanceThreshold) {
			GoToTarget(targetP);
		}

		if (Input.GetButtonDown("Pause")) {
			if (paused) {
				GetComponent<AudioSource>().UnPause();
				Time.timeScale = oldTimeScale;
				pausedTextContainer.SetActive(false);
				player.enabled = oldEnabled;
				paused = false;
			} else {
				GetComponent<AudioSource>().Pause();
				oldTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				pausedTextContainer.SetActive(true);
				oldEnabled = player.enabled;
				player.enabled = false;
				paused = true;
			}
		}
	}

	public void GoToTarget(Vector2 p) {
		myCamera.transform.position = new Vector3(p.x, p.y, myCamera.transform.position.z);
	}

	public IEnumerator Read(string text) {
		
		bool oldEnabled = GameManager.instance.player.enabled;
		float oldTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		player.enabled = false;

		yield return null;
		dialogues.DisplayText(text);
		while (!Input.GetButtonDown("Interact")) {
			yield return null;
		}
		dialogues.Hide();

		Time.timeScale = oldTimeScale;
		player.enabled = oldEnabled;
	}


	#if UNITY_EDITOR
	public SharedLevelEditingStuff shared = new SharedLevelEditingStuff();
	#endif
}
