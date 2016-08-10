using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class LevelBuilder : MonoBehaviour {
#if UNITY_EDITOR

	public static readonly string[] LAYER_OPTIONS = new string[]{ "Background", "Active", "Foreground" };
	public static readonly string[] SORTING_LAYERS= new string[]{ "Background Tiles", "Active Level", "Foreground Tiles" };

	public static Vector2 SCREEN_SIZE = new Vector2(15, 12);
	public Vector2 mapSize = new Vector2(6, 8);
	public Vector2 gridSize = new Vector2(1f, 1f);
	public Texture2D importTileSheet;
	public List<string> knownTileSheets;
	public string currentlySelectedTileSheetAssetLocation;
	public Sprite[] sprites;
	public GameObject defaultTilePrefab;
	public Sprite currentlySelectedSprite;

	public int currentEditLayer = 0;

	private GameObject[] _tcs;

	public struct TileLocation {
		public TileLocation(int _x, int _y, GameObject _tile, int _editLayerId) {
			x = _x;
			y = _y;
			tile = _tile;
			editLayerId = _editLayerId;
		}
		public int x;
		public int y;
		public GameObject tile;
		public int editLayerId;
	}
	[System.Serializable]
	public class SpritePrefabPairing {
		public SpritePrefabPairing(Sprite sprite, GameObject prefab) {
			this.sprite = sprite;
			this.prefab = prefab;
		}
		public Sprite sprite;
		public GameObject prefab;
	}

	public SpritePrefabPairing LocateByCurrentSprite() {
		if (spritePrefabPairings == null) {
			spritePrefabPairings = new List<SpritePrefabPairing>();
		}
		foreach (SpritePrefabPairing spp in spritePrefabPairings) {
			if (spp.sprite == currentlySelectedSprite) {
				return spp;
			}
		}
		return null;
	}
	public GameObject CurrentPrefab() {
		SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			return spp.prefab;
		} else {
			return defaultTilePrefab;
		}
	}
	public void SetCurrentPrefab(GameObject prefab) {
		SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			spp.prefab = prefab;
		} else {
			spritePrefabPairings.Add(new SpritePrefabPairing(currentlySelectedSprite, prefab));
		}
	}

	[SerializeField]
	public List<SpritePrefabPairing> spritePrefabPairings;

	public List<TileLocation> tiles;

	public GameObject TileContainer(int tcid) {
		if (_tcs == null || _tcs.Length != LAYER_OPTIONS.Length) {
			_tcs = new GameObject[4];
		}
		if (_tcs[tcid] == null) {
			try {
				_tcs[tcid] = transform.FindChild("TileContainer_" + tcid).gameObject;
			} catch {
			}
		}
		if (_tcs[tcid] == null) {
			_tcs[tcid] = new GameObject("TileContainer_" + tcid);
			_tcs[tcid].transform.parent = transform;
		}
		return _tcs[tcid];
	}
	public GameObject FindOrCreateTileAt(int x, int y) {
		GameObject go = FindTileAt(x, y);
		if (go == null) {
			if (currentEditLayer == 1) {
				go = PrefabUtility.InstantiatePrefab(CurrentPrefab()) as GameObject;
			} else {
				go = new GameObject("Sprite Tile");
			}
			go.AddComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYERS[currentEditLayer];

			go.transform.parent = TileContainer(currentEditLayer).transform;
			go.transform.localPosition = new Vector3(x + gridSize.x/2, y + gridSize.y/2);
			tiles.Add(new TileLocation(x, y, go, currentEditLayer));
		}
		return go;
	}

	public GameObject FindTileAt(int x, int y) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y && tl.editLayerId == currentEditLayer) {
				return tl.tile;
			}
		}
		return null;
	}
		
	public void RemoveTileAt(int x, int y) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y && tl.editLayerId == currentEditLayer) {
				Undo.DestroyObjectImmediate(tl.tile);
				tiles.Remove(tl);
				return;
			}
		}
	}

	private void EnsureTileLocationListIsSetup() {
		if (tiles == null) {
			tiles = new List<TileLocation>();
			for (int l = 0; l < LAYER_OPTIONS.Length; l +=1) {
				for (int i = 0; i < TileContainer(l).transform.childCount; i+=1) {
					Transform child = TileContainer(l).transform.GetChild(i);
					tiles.Add(new TileLocation(Mathf.RoundToInt(child.localPosition.x-gridSize.x/2), Mathf.RoundToInt(child.localPosition.y-gridSize.y/2), child.gameObject, l));
				}
			}
		}
	}

	public void RebindChildrenToTheirPrefabs() {
		Sprite css = currentlySelectedSprite;
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			currentlySelectedSprite = tl.tile.GetComponent<SpriteRenderer>().sprite;
		}
		currentlySelectedSprite = css;
	}

	// in the editor, validate a bunch of things on change. But we rely on editor-specific
	// tools so don't compile it into the playable game.
	[ExecuteInEditMode]
	void OnValidate(){
		mapSize = new Vector2((int) mapSize.x, (int) mapSize.y);
		if (importTileSheet) {
			SetCurrentTileSheet(AssetDatabase.GetAssetPath( importTileSheet ));
			importTileSheet = null;
		}
		RebindChildrenToTheirPrefabs();
	}

	public void SetCurrentTileSheet(string target) {
		currentlySelectedTileSheetAssetLocation = target;
		knownTileSheets.Add(currentlySelectedTileSheetAssetLocation);
		knownTileSheets = knownTileSheets.Distinct().ToList();

		sprites = AssetDatabase.LoadAllAssetsAtPath( currentlySelectedTileSheetAssetLocation )
			.OfType<Sprite>().ToArray();
		if (sprites.Length == 0) {
			Debug.LogError("Unable to set to non-multiSprite value.");
			importTileSheet = null;
		}
	}

	public void RemoveCurrentSpritesheet() {
		knownTileSheets.Remove(currentlySelectedTileSheetAssetLocation);
		SetCurrentTileSheet(knownTileSheets[0]);
	}

	public void ReInstantiateTiles() {
	}

	public void RebindTileList() {
		tiles = null;
		_tcs = null;
		EnsureTileLocationListIsSetup();
	}
#endif
}
