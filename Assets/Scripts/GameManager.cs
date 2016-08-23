using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

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

	public GameMode currentGameMode;
	public bool paused {
		get {
			return currentGameMode == GameMode.PAUSED;
		}
	}
	public static readonly Vector2 SCREEN_SIZE = new Vector2(16, 12);

	private GameObject currentActiveObjects;

	[SerializeField]
	public List<Level> levels;


	// TODO move this editor crap out of here!
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

	public PlayerInputManager inputManager;

	// SINGLETON
	public static GameManager instance;
	void Start () {
		instance = this;
		GetComponent<AudioSource>().Play();
		player.GetComponent<GameStateFlagsComponent>().state = GetComponent<GameStateFlagsComponent>().state;
//		=GetComponent<GameStateFlagsComponent>();
		GoToTarget(FindTarget());
		inputManager = player.GetComponent<PlayerInputManager>();
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

	public IEnumerator SwapToNewMusic(Level nextLevel) {
		AudioSource audioSource = GetComponent<AudioSource> ();
		if (nextLevel.backgroundMusic != audioSource.clip && nextLevel.backgroundMusic != null) {
			while (audioSource.volume > 0) {
				yield return null;
				audioSource.volume -= Time.deltaTime;
			}
			audioSource.clip = nextLevel.backgroundMusic;
			audioSource.Play();
			while (audioSource.volume < 1) {
				yield return null;
				audioSource.volume += Time.deltaTime;
			}
			audioSource.volume = 1;
		}
	}

	public IEnumerator MoveIntoLevel(Level targetLevel, Vector3 playerOffset) {
		targetLevel.transform.position = Vector3.Scale(targetLevel.mapPosition - currentLevel.mapPosition, GameManager.SCREEN_SIZE);
		targetLevel.gameObject.SetActive(true);

		DealWithActiveObjects(targetLevel);
		yield return SwapToNewMusic(targetLevel);

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
		yield return MoveIntoLevel(targetLevel, playerOffset);

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

		// TODO this should actually live on the player object, probably.
		if (inputManager.GetButtonDown("Pause", GameMode.MOVEMENT)) {
			if (paused) {
				GetComponent<AudioSource>().UnPause();
				Time.timeScale = oldTimeScale;
				pausedTextContainer.SetActive(false);
				player.enabled = oldEnabled;
				currentGameMode = GameMode.MOVEMENT;
			} else {
				GetComponent<AudioSource>().Pause();
				oldTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				pausedTextContainer.SetActive(true);
				oldEnabled = player.enabled;
				player.enabled = false;
				currentGameMode = GameMode.PAUSED;
			}
		}

		if (Input.GetKeyDown(KeyCode.S)) {
			SaveGameState(0);
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			LoadGameState(0);
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
		while (!inputManager.GetButtonDown("Interact", GameMode.MOVEMENT)) {
			yield return null;
		}
		dialogues.Hide();

		Time.timeScale = oldTimeScale;
		player.enabled = oldEnabled;
	}

	void SaveGameState(int slot) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/game_ " + slot + ".gd"); //you can call it anything you want
		bf.Serialize(file, player.currentGameState);
		file.Close();
	}

	void LoadGameState(int slot) {
		if(File.Exists(Application.persistentDataPath + "/game_ " + slot + ".gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/game_ " + slot + ".gd", FileMode.Open);
			player.GetComponent<GameStateFlagsComponent>().state = (GameStateFlags)bf.Deserialize(file);
			file.Close();
		}
	}

	#if UNITY_EDITOR
	public SharedLevelEditingStuff shared = new SharedLevelEditingStuff();
	#endif
}
