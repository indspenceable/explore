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
	public AudioClip backgroundMusic;
	public string levelName;

#if UNITY_EDITOR

	public static readonly string[] LAYER_OPTIONS = new string[]{ "BG1", "BG2", "Geometry", "FG1", "FG2", "ActiveObjects" };
	public static readonly string[] SORTING_LAYERS= new string[]{ "Background Tiles", "Background Tiles", "Active Level", "Foreground Tiles", "Foreground Tiles", "Active Level" };
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

	public List<TileLocation> tiles;

	public GameObject TileContainer(int tcid) {
		if (_tcs == null || _tcs.Length != LAYER_OPTIONS.Length) {
			_tcs = new GameObject[LAYER_OPTIONS.Length];
		}
		if (_tcs[tcid] == null) {
			try {
				_tcs[tcid] = transform.FindChild(LAYER_OPTIONS[tcid]).gameObject;
			} catch {
			}
		}
		if (_tcs[tcid] == null) {
			_tcs[tcid] = new GameObject(LAYER_OPTIONS[tcid]);
			_tcs[tcid].transform.parent = transform;
			_tcs[tcid].transform.localPosition = Vector3.zero;
		}
		return _tcs[tcid];
	}

	public bool OnPrefabLayer(int layer) {
		return layer == 2 || layer == 5;
	}

	public GameObject FindOrCreateTileAt(int x, int y, int layer, EditorUtil util) {
		GameObject go = FindTileAt(x, y, layer);
		if (go == null) {
			if (OnPrefabLayer(layer) && false) {
//				go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			} else {
				go = new GameObject("Sprite Tile");
				go.AddComponent<SpriteRenderer>();
				go.GetComponent<SpriteRenderer>().sortingLayerName = SORTING_LAYERS[layer];
				go.GetComponent<SpriteRenderer>().material = util.pixelPerfectSprite;
				go.isStatic = true;
			}

			go.transform.parent = TileContainer(layer).transform;
			go.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f);
			tiles.Add(new TileLocation(x, y, go, layer));
		}
		return go;
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
					tiles.Add(new TileLocation(Mathf.RoundToInt(child.localPosition.x-0.5f), Mathf.RoundToInt(child.localPosition.y-0.5f), child.gameObject, l));
				}
			}
		}
	}

	// in the editor, validate a bunch of things on change. But we rely on editor-specific
	// tools so don't compile it into the playable game.
	[ExecuteInEditMode]
	void OnValidate(){
		mapSize = new Vector2((int) mapSize.x, (int) mapSize.y);
		mapPosition = new Vector2((int) mapPosition.x, (int) mapPosition.y);
		MoveMeToMyPosition();
		gameObject.name = (levelName == null || levelName == "") ? "Level" : levelName;
	}

	public void MoveMeToMyPosition() {
		transform.position = new Vector3(mapPosition.x * GameManager.SCREEN_SIZE.x, mapPosition.y * GameManager.SCREEN_SIZE.y);
	}
#endif
}
