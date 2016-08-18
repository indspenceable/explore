using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class Level : MonoBehaviour {
	public Vector2 mapSize = new Vector2(1, 1);
	public Vector2 mapPosition = new Vector2(0,0);
	public Sprite backgroundImage;

#if UNITY_EDITOR

	public static readonly string[] LAYER_OPTIONS = new string[]{ "Background", "Active", "Foreground" };
	public static readonly string[] SORTING_LAYERS= new string[]{ "Background Tiles", "Active Level", "Foreground Tiles" };

	// TODO - we never switch the gridsize from 1,1 cause it breaks things. Just bake this number in?
	public Vector2 gridSize = new Vector2(1f, 1f);
	public Texture2D importTileSheet;
	public string currentlySelectedTileSheetAssetLocation;
	public Sprite[] sprites;
	public Sprite currentlySelectedSprite;
	public int currentEditLayer = 0;
	private GameObject[] _tcs;

	public SharedLevelEditingStuff shared {
		get {
			return GameObject.Find("GameManager").GetComponent<GameManager>().shared;
		}
	}

	public SharedLevelEditingStuff.SpritePrefabPairing LocateByCurrentSprite() {
		if (shared.spritePrefabPairings == null) {
			shared.spritePrefabPairings = new List<SharedLevelEditingStuff.SpritePrefabPairing>();
		}
		foreach (SharedLevelEditingStuff.SpritePrefabPairing spp in shared.spritePrefabPairings) {
			if (spp.sprite == currentlySelectedSprite) {
				return spp;
			}
		}
		return null;
	}
	public void SetCurrentPrefab(GameObject prefab) {
		SharedLevelEditingStuff.SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			spp.prefab = prefab;
		} else {
			shared.spritePrefabPairings.Add(new SharedLevelEditingStuff.SpritePrefabPairing(currentlySelectedSprite, prefab));
		}
	}

	public GameObject CurrentPrefab() {
		SharedLevelEditingStuff.SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			return spp.prefab;
		} else {
			return shared.defaultTilePrefab;
		}
	}

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
		return FindOrCreateTileAt(x, y, currentEditLayer);
	}
	public GameObject FindOrCreateTileAt(int x, int y, int layer) {
		GameObject go = FindTileAt(x, y);
		if (go == null) {
			if (currentEditLayer == 1) {
				go = PrefabUtility.InstantiatePrefab(CurrentPrefab()) as GameObject;
			} else {
				go = new GameObject("Sprite Tile");
				go.AddComponent<SpriteRenderer>();
				go.GetComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYERS[layer];
				go.GetComponent<SpriteRenderer>().material = shared.pixelPerfectSprite;
				go.isStatic = true;
			}

			go.transform.parent = TileContainer(layer).transform;
			go.transform.localPosition = new Vector3(x + gridSize.x/2, y + gridSize.y/2);
			tiles.Add(new TileLocation(x, y, go, layer));
		}
		return go;
	}

	public GameObject FindTileAt(int x, int y) {
		return FindTileAt(x, y, currentEditLayer);
	}

	public GameObject FindTileAt(int x, int y, int layer) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y && tl.editLayerId == layer) {
				return tl.tile;
			}
		}
		return null;
	}
		
	public void RemoveTileAt(int x, int y) {
		RemoveTileAt(x, y, currentEditLayer);
	}
	public void RemoveTileAt(int x, int y, int layer) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y && tl.editLayerId == layer) {
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
		mapPosition = new Vector2((int) mapPosition.x, (int) mapPosition.y);
		if (importTileSheet) {
			SetCurrentTileSheet(AssetDatabase.GetAssetPath( importTileSheet ));
			importTileSheet = null;
		}
		RebindChildrenToTheirPrefabs();
	}

	public void SetCurrentTileSheet(string target) {
		currentlySelectedTileSheetAssetLocation = target;
		shared.knownTileSheets.Add(currentlySelectedTileSheetAssetLocation);
		shared.knownTileSheets = shared.knownTileSheets.Distinct().ToList();

		sprites = AssetDatabase.LoadAllAssetsAtPath( currentlySelectedTileSheetAssetLocation )
			.OfType<Sprite>().ToArray();
		if (sprites.Length == 0) {
			Debug.LogError("Unable to set to non-multiSprite value.");
			importTileSheet = null;
		}
	}

	public void RemoveCurrentSpritesheet() {
		shared.knownTileSheets.Remove(currentlySelectedTileSheetAssetLocation);
		SetCurrentTileSheet(shared.knownTileSheets[0]);
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
