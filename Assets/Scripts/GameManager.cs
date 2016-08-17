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
	public SpriteRenderer backgroundImage;

	public static bool paused = false;
	public static readonly Vector2 SCREEN_SIZE = new Vector2(16, 12);

	private GameObject currentActiveObjects;

	[SerializeField]
	public List<Level> levels;


	[System.Serializable]
	public class Door {
		public int x;
		public int y;
	}
	public enum Direction {
		RIGHT,
		LEFT,
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

	public Level FindLevelWithCoord(int x, int y) {
		foreach (Level l in levels) {
			if ((l.mapPosition.x <= x && (l.mapPosition + l.mapSize).x > x) &&
				(l.mapPosition.y <= y && (l.mapPosition + l.mapSize).y > y)) {
				return l;
			}
		}
		return null;
	}

	// SINGLETON
	public static GameManager instance;
	void Start () {
		instance = this;
		GetComponent<AudioSource>().Play();
		GoToTarget(FindTarget());
	}

	public Image fadeOutOverlay;
	public IEnumerator Fade(Color begin, Color end, float time) {
		float dt = 0f;
		fadeOutOverlay.color = begin;
		while (dt < time) {
			yield return null;
			dt += Time.unscaledDeltaTime;
			Color c = Color.Lerp(begin, end, dt/time);
			fadeOutOverlay.color = c;
		}
	}

	GameObject deactivatedActiveObjectsContainer = null;

	public void DealWithActiveObjects(Level targetLevel) {
		// Re-activate their activeObjects container
		if (deactivatedActiveObjectsContainer != null) {
			deactivatedActiveObjectsContainer.SetActive(true);
			deactivatedActiveObjectsContainer = null;
		}
		// Destroy the old room's activated objects
		Destroy(currentActiveObjects);
		currentActiveObjects = null;

		Transform AOTransform = targetLevel.transform.FindChild("ActiveObjects");
		if (AOTransform != null) {
			deactivatedActiveObjectsContainer = AOTransform.gameObject;
			currentActiveObjects = Instantiate(deactivatedActiveObjectsContainer) as GameObject;
			deactivatedActiveObjectsContainer.SetActive(false);
		}





	}

	public void MoveIntoLevel(Level targetLevel, Vector3 playerOffset) {
		targetLevel.transform.position = Vector3.Scale(targetLevel.mapPosition - currentLevel.mapPosition, GameManager.SCREEN_SIZE);
		targetLevel.gameObject.SetActive(true);

		DealWithActiveObjects(targetLevel);

		currentLevel.gameObject.SetActive(false);
		currentLevel = targetLevel;
		player.transform.position -= currentLevel.transform.position;
		player.transform.position += playerOffset;
		myCamera.transform.position -= currentLevel.transform.position;
		currentLevel.transform.position = Vector3.zero;
		backgroundImage.sprite = targetLevel.backgroundImage;
		GoToTarget(FindTarget());
	}

	public IEnumerator DoDoorCollision(int px, int py, Vector3 playerOffset, DoorCollider d) {
		player.enabled = false;
		var oldLayer = d.gameObject.layer;
		d.gameObject.layer = LayerMask.NameToLayer("Default");
		fadeOutOverlay.gameObject.SetActive(true);
		yield return Fade(new Color(0,0,0, 0), Color.black, 0.2f);

		Level targetLevel = FindLevelWithCoord(px, py);
		MoveIntoLevel(targetLevel, playerOffset);

		yield return Fade(Color.black, new Color(0,0,0, 0), 0.2f);
		fadeOutOverlay.gameObject.SetActive(false);

		player.enabled = true;
		d.gameObject.layer = oldLayer;
	}

	public IEnumerator DoorCollision(DoorCollider door) {
		if (door.direction == Direction.RIGHT) {
			int py = Mathf.FloorToInt(player.transform.position.y / GameManager.SCREEN_SIZE.y + currentLevel.mapPosition.y);
			int px = (int)currentLevel.mapPosition.x + (int)currentLevel.mapSize.x-1;

			if (DoorAt(px, py)) {
				// Door going right
				yield return DoDoorCollision(px+1, py, Vector3.right, door);
			}
		}
		//
		if (door.direction == Direction.LEFT) {
			int py = Mathf.FloorToInt(player.transform.position.y / GameManager.SCREEN_SIZE.y + currentLevel.mapPosition.y);
			int px = (int)currentLevel.mapPosition.x-1;
			if (DoorAt(px, py)) {
				// Door going right
				yield return DoDoorCollision(px, py, -Vector3.right, door);
			}
		}
	}


	bool oldEnabled;
	float oldTimeScale;

	Vector2 FindTarget() {
		float halfWidth = SCREEN_SIZE.x / 2;
		float halfHeight = SCREEN_SIZE.y / 2;
		Vector2 scaledMap = Vector2.Scale(currentLevel.mapSize, SCREEN_SIZE);
		float px = Mathf.Clamp(player.transform.position.x, halfWidth, scaledMap.x-halfWidth);
		float py = Mathf.Clamp(player.transform.position.y, halfHeight, scaledMap.y-halfHeight);
		return new Vector2(px, py);
	}

	public void Update() {
		// find the target camera position. This involves keeping the camera within the bounds of the level.
		Vector2 targetP = FindTarget();

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

	public IEnumerator WaitForUnscaledSeconds(float time) {
		float dt = 0;
		while (dt < time)  {
			yield return null;
			dt += Time.unscaledDeltaTime;
		}
	}

	public IEnumerator Read(string text, AudioClip blipSound) {
		
		bool oldEnabled = GameManager.instance.player.enabled;
		float oldTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		player.enabled = false;

		int delay = 0;
		yield return null;
		for (int i = 0; i < text.Length; i += 1) {
			char currentChar = text[i];
			dialogues.DisplayText(text.Substring(0,i+1));
			delay -= 1;
			switch(currentChar) {
			case '\n':
				yield return WaitForUnscaledSeconds(0.5f);
				break;
			case '.':
				yield return WaitForUnscaledSeconds(0.3f);
				break;
			case ' ':
				yield return WaitForUnscaledSeconds(0.01f);
				break;
			default:
				yield return WaitForUnscaledSeconds(0.01f);
				if (blipSound != null && delay <= 0) {
					Time.timeScale = 1f;
					AudioSource.PlayClipAtPoint(blipSound, Vector3.zero);
					Time.timeScale = 0f;
					delay = 3;
				}
				break;
			}
		}

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
