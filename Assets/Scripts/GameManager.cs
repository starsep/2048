using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Sprite[] tileSprites;
	public Camera mainCamera;

	private System.Random generator;
	private Grid grid;
	private const int size = 4;

	class Grid {
		private const int EMPTY = -1;

		public Sprite[] tileSprites;
		public GameObject[,] tile;
		public SpriteRenderer[,] renderer;
		public int[,] value;
		public readonly int size;

		private void InitTile(int x, int y) {
			value [y, x] = EMPTY;
			GameObject newTile = new GameObject ("Tile[" + y + "," + x + "]");
			newTile.transform.position = new Vector3 (x, y, 1.0f);
			renderer [y, x] =  newTile.AddComponent<SpriteRenderer> ();
			renderer [y, x].sprite = tileSprites [(size * y + x) % 12];
		}

		public void Update() {
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					renderer [y, x].enabled = value [y, x] != EMPTY;
					if (value [y, x] != EMPTY) {
						renderer [y, x].sprite = tileSprites [value [y, x]];
					}
				}
			}
		}

		public Grid(int size, Sprite[] tileSprites) {
			this.tileSprites = tileSprites;
			this.size = size;
			value = new int[size, size];
			tile = new GameObject[size, size];
			renderer = new SpriteRenderer[size, size];
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					InitTile(y, x);
				}
			}
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					value[y, x] = (size * x + y) % 12;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		float center = size / 2f - 0.5f;
		mainCamera.transform.position = new Vector3 (center, center, -10.0f);
		generator = new System.Random ();
		grid = new Grid (size, tileSprites);
	}
	
	// Update is called once per frame
	void Update() {
		int x = generator.Next () % size;
		int y = generator.Next () % size;
		grid.renderer [y, x].enabled = !grid.renderer [y, x].enabled;
 		// grid.Update ();
	}
}
