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
	public LevelContainer levels;
	private int GameIsActiveState = 0;
	public GameObject titleScreen;
	public AudioClip titleScreenMusic;

	public float ActiveGameDeltaTime {
		get {
			if (currentGameMode == GameMode.MOVEMENT) {
				return Time.deltaTime;
			} else {
				return 0f;
			}
		}
	}

	public GameMode currentGameMode;
	public bool paused {
		get {
			return currentGameMode == GameMode.PAUSED;
		}
	}
	public static readonly Vector2 SCREEN_SIZE = new Vector2(16, 12);

	public GameObject currentActiveObjects;

	[SerializeField]
	public DoorMap doors;

	public PlayerInputManager inputManager;

	// SINGLETON
	public static GameManager instance;

	public void Start() {
		instance = this;
		levels.BuildCache(true);
		inputManager = player.GetComponent<PlayerInputManager>();
		StartCoroutine(FadeInMusic(titleScreenMusic, null, true));
	}

	void StartGame () {
		currentLevel = levels.FindLevelByWorldCoords(player.transform.position);
		if (currentLevel == null) {
			Debug.LogError("Player is starting outside of a room!");
		}

		GetComponent<AudioSource>().Play();

		DealWithActiveObjects(currentLevel);
		InstallAndPlayMusic(GetComponent<AudioSource> (), currentLevel.currentMusic);
		backgroundImage.sprite = currentLevel.backgroundImage;
		foreach (Level l in levels.levels) {
			l.gameObject.SetActive(l == currentLevel);
		}
		MoveCameraToCameraTargetInstantly(FindTarget());
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

	public void InstantDestroyActiveObjects() {
		Destroy(currentActiveObjects);
	}
	public void DealWithActiveObjects(Level targetLevel) {
		// Re-activate their activeObjects container
		if (deactivatedActiveObjectsContainer != null) {
			deactivatedActiveObjectsContainer.SetActive(true);
			deactivatedActiveObjectsContainer = null;
		}
		// Destroy the old room's activated objects
		InstantDestroyActiveObjects();
		currentActiveObjects = null;

		Transform AOTransform = targetLevel.transform.FindChild("ActiveObjects");
		if (AOTransform != null) {
			deactivatedActiveObjectsContainer = AOTransform.gameObject;
			currentActiveObjects = Instantiate(deactivatedActiveObjectsContainer) as GameObject;
			currentActiveObjects.transform.position = deactivatedActiveObjectsContainer.transform.position;
			deactivatedActiveObjectsContainer.SetActive(false);
			foreach (IActivatableObject o in currentActiveObjects.GetComponentsInChildren<IActivatableObject>()) {
				o.Activate(targetLevel);
			}
		}
	}

	void InstallAndPlayMusic(AudioSource audioSource, AudioClip backgroundMusic)
	{
		audioSource.clip = backgroundMusic;
		audioSource.Play ();
	}

	public IEnumerator FadeOutMusic(AudioSource audioSource=null) {
		if (audioSource == null ) {
			audioSource = GetComponent<AudioSource> ();
		}
		while (audioSource.volume > 0) {
			yield return null;
			audioSource.volume -= GameManager.instance.ActiveGameDeltaTime;
		}
		audioSource.volume = 0f;
	}

	public IEnumerator FadeInMusic(AudioClip music, AudioSource audioSource=null, bool instant=false) {
		if (audioSource == null ) {
			audioSource = GetComponent<AudioSource> ();
		}
		InstallAndPlayMusic(audioSource, music);
		if (instant) {
			audioSource.volume = 1;
		}
		while (audioSource.volume < 1) {
			yield return null;
			audioSource.volume += GameManager.instance.ActiveGameDeltaTime;
		}
		audioSource.volume = 1f;
	}

	public IEnumerator SwapToNewMusicByLevel(Level nextLevel) {
		AudioSource audioSource = GetComponent<AudioSource> ();
		if (nextLevel.currentMusic != audioSource.clip && nextLevel.currentMusic != null) {
			yield return FadeOutMusic(audioSource);

			yield return FadeInMusic(nextLevel.currentMusic, audioSource, true);
		}
	}

	public IEnumerator MoveIntoLevel(Level targetLevel, Vector3 playerOffset) {
		if (currentLevel == targetLevel) {
			Debug.Log("moving into same level.");
			yield break;
		} else {
			Debug.Log("Moving into another level.");
		}
//		targetLevel.transform.position = Vector3.Scale(targetLevel.mapPosition - currentLevel.mapPosition, GameManager.SCREEN_SIZE);
		targetLevel.gameObject.SetActive(true);

		DealWithActiveObjects(targetLevel);
		yield return SwapToNewMusicByLevel(targetLevel);
		currentLevel.gameObject.SetActive(false);

		currentLevel = targetLevel;
//		player.transform.position -= currentLevel.transform.position;
		player.transform.position += playerOffset;
		player.VisitCurrentLocation ();
//		myCamera.transform.position -= currentLevel.transform.position;
//		currentLevel.transform.position = Vector3.zero;
		backgroundImage.sprite = targetLevel.backgroundImage;
		MoveCameraToCameraTargetInstantly(FindTarget());

		if (! currentLevel.gameObject.activeSelf) {
			Debug.LogError("Current level is not active?");
		}
	}


	public IEnumerator DoDoorCollision(Vector3 playerDestination, DoorCollider d) {
		// While we transition, change the layer to default.
		var oldLayer = d.gameObject.layer;
		d.gameObject.layer = LayerMask.NameToLayer("Default");
		d.gameObject.SetActive(false);

		yield return DoRoomTransitionFull(playerDestination);

		d.gameObject.layer = oldLayer;
		d.gameObject.SetActive(true);
	}

	public IEnumerator DoRoomTransitionFull(Vector3 playerDestination) {
		player.controlsAreEnabled = false;
		player.enabled = false;

		// End the airdodge early, if needed
		if (player.currentlyPerformingAirDodge) {
			player.StopCoroutine (player.airDodgeCoroutine);
			player.FinishAirDodge ();
		}


		fadeOutOverlay.gameObject.SetActive(true);
		yield return Fade(new Color(0,0,0, 0), Color.black, 0.2f);

		Level targetLevel = levels.FindLevelByWorldCoords(playerDestination);
		Debug.Log("offset is: " + (playerDestination-player.transform.position));
		yield return MoveIntoLevel(targetLevel, playerDestination-player.transform.position);

		yield return Fade(Color.black, new Color(0,0,0, 0), 0.2f);
		fadeOutOverlay.gameObject.SetActive(false);

		player.enabled = true;
		player.controlsAreEnabled = true;
		player.GetComponent<Collider2D>().enabled = true;
	}

	public IEnumerator DoorCollision(DoorCollider door) {
		if (door.direction == DoorMap.Direction.RIGHT) {
//			int py = Mathf.FloorToInt((player.transform.position.y - currentLevel.transform.position.y) / GameManager.SCREEN_SIZE.y + currentLevel.mapPosition.y);
//			int px = (int)currentLevel.mapPosition.x + (int)currentLevel.mapSize.x-1;

//			if (doors.DoorAt(px, py)) {
			if (true) {
				// Door going right
				yield return DoDoorCollision(player.transform.position+Vector3.right, door);
			}
		}
		//
		if (door.direction == DoorMap.Direction.LEFT) {
//			int py = Mathf.FloorToInt((player.transform.position.y - currentLevel.transform.position.y) / GameManager.SCREEN_SIZE.y + currentLevel.mapPosition.y);
//			int px = (int)currentLevel.mapPosition.x-1;
//			if (doors.DoorAt(px, py)) {
			if (true) {
				// Door going left
				yield return DoDoorCollision(player.transform.position-Vector3.right, door);
			}
		}
		if (door.direction == DoorMap.Direction.UP) {
//			int px = Mathf.FloorToInt((player.transform.position.x - currentLevel.transform.position.x) / GameManager.SCREEN_SIZE.x + currentLevel.mapPosition.x);
//			int py = (int)currentLevel.mapPosition.y + (int)currentLevel.mapSize.y-1;
			//			if (doors.DoorAt(px, py)) {
			if (true) {
				// Door going right
				player.vert.vy = 0f;
				yield return DoDoorCollision(player.transform.position+(Vector3.up*2), door);
			}
		}
		if (door.direction == DoorMap.Direction.DOWN) {
//			int px = Mathf.FloorToInt((player.transform.position.x - currentLevel.transform.position.x) / GameManager.SCREEN_SIZE.x + currentLevel.mapPosition.x);
//			int py = (int)currentLevel.mapPosition.y-1;
			//			if (doors.DoorAt(px, py)) {
			if (true) {
				// Door going right
				yield return DoDoorCollision(player.transform.position-Vector3.up, door);
			}
		}
	}


	bool oldEnabled;

	Vector2 FindTarget() {
		float halfWidth = SCREEN_SIZE.x / 2;
		float halfHeight = SCREEN_SIZE.y / 2;
		Vector2 scaledMap = Vector2.Scale(currentLevel.mapSize, SCREEN_SIZE);
		float px = Mathf.Clamp(player.transform.position.x, currentLevel.transform.position.x + halfWidth, currentLevel.transform.position.x + scaledMap.x-halfWidth);
		float py = Mathf.Clamp(player.transform.position.y, currentLevel.transform.position.y + halfHeight, currentLevel.transform.position.y + scaledMap.y-halfHeight);
		return new Vector2(px, py);
	}

	public void Update() {
		if (GameIsActiveState == 0) {
			if (inputManager.GetButtonDown("Melee", GameMode.MOVEMENT)) {
				GameIsActiveState = 1;
				StartGame();
				titleScreen.SetActive(false);
				player.gameObject.SetActive(true);
			}
			return;
		}

		// New frame! no sounds have played yet!
		SoundsThatHavePlayedThisFrame.Clear();
		// find the target camera position. This involves keeping the camera within the bounds of the level.
		Vector2 targetP = FindTarget();

		// Can the camera go there?
		myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, new Vector3(targetP.x, targetP.y, myCamera.transform.position.z), GameManager.instance.ActiveGameDeltaTime * lerpWeight);
		if (Mathf.Abs(myCamera.transform.position.x - targetP.x) + Mathf.Abs(myCamera.transform.position.y - targetP.y) < minDistanceThreshold) {
			MoveCameraToCameraTargetInstantly(targetP);
		}

		// TODO this should actually live on the player object, probably.
		if (inputManager.GetButtonDown("Pause", GameMode.MOVEMENT)) {
			GetComponent<AudioSource>().Pause();
			pausedTextContainer.SetActive(true);
			oldEnabled = player.enabled;
			player.enabled = false;
			currentGameMode = GameMode.PAUSED;
		} else if (inputManager.GetButtonDown("Pause", GameMode.PAUSED)) {
			GetComponent<AudioSource>().UnPause();
			pausedTextContainer.SetActive(false);
			player.enabled = oldEnabled;
			currentGameMode = GameMode.MOVEMENT;
		}

//		if (Input.GetKeyDown(KeyCode.S)) {
//			SaveGameState(0);
//		}
//		if (Input.GetKeyDown(KeyCode.L)) {
//			LoadGameState(0);
//		}
	}

	public void MoveCameraToCameraTargetInstantly(Vector2 p) {
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
		GameMode oldGameMode = currentGameMode; // This should always be Movement


		player.enabled = false;
		currentGameMode = GameMode.TEXT;

		int delay = 0;
		yield return null;
		for (int i = 0; i < text.Length; i += 1) {
			char currentChar = text[i];
			dialogues.DisplayText(text.Substring(0,i+1));
			delay -= 1;
			switch(currentChar) {
			case '\n':
				yield return new WaitForSeconds(0.5f);
				break;
			case '.':
				yield return new WaitForSeconds(0.3f);
				break;
			case ' ':
				yield return new WaitForSecondsRealtime(0.00001f);
				break;
			default:
				yield return new WaitForSecondsRealtime(0.00001f);
				if (blipSound != null && delay <= 0) {
					GameManager.instance.PlaySound(blipSound);
					delay = 2;
				}
				break;
			}
		}

		dialogues.DisplayText(text);
		while (!inputManager.GetButtonDown("Interact", GameMode.TEXT)) {
			yield return null;
		}
		dialogues.Hide();

		player.enabled = oldEnabled;
		currentGameMode = oldGameMode;
	}

	public void SaveGameState(int slot, Vector3 pos=new Vector3()) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/game_ " + slot + ".gd"); //you can call it anything you want
		SerailizableGameState gs = player.currentGameState;
		if (pos != Vector3.zero) {
			gs.pos = pos;
		}
		bf.Serialize(file, gs);
		file.Close();
	}

	public void LoadGameState(int slot) {
		if(File.Exists(Application.persistentDataPath + "/game_ " + slot + ".gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/game_ " + slot + ".gd", FileMode.Open);
			SerailizableGameState gs = player.GetComponent<SerailizableGameStateComponent>().state;
			gs = (SerailizableGameState)bf.Deserialize(file);
			file.Close();
			// Hackity hack. Disable stuff here just to make sure it gets disabled before next frame.
			player.controlsAreEnabled = false;
			player.enabled = false;
			player.GetComponent<Collider2D>().enabled = false;
			StartCoroutine(DoRoomTransitionFull(gs.pos));
		}
	}

	private HashSet<AudioClip> SoundsThatHavePlayedThisFrame = new HashSet<AudioClip>();

	public void PlaySound(AudioClip soundEffect) {
		if (soundEffect == null || SoundsThatHavePlayedThisFrame.Contains(soundEffect)) {
			return;
		}
		AudioSource.PlayClipAtPoint (soundEffect, Vector3.zero);
		SoundsThatHavePlayedThisFrame.Add(soundEffect);
	}
}
