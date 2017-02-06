using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	private const int EMPTY = -1;
	private System.Random generator;

	public Sprite[] tileSprites;
	public GameObject[,] tile;
	public SpriteRenderer[,] renderer;
	public int[,] value;
	private int[,] lastValue;
	public readonly int size;

	private void InitTile (int x, int y) {
		value [y, x] = EMPTY;
		GameObject newTile = new GameObject ("Tile[" + y + "," + x + "]");
		newTile.transform.position = new Vector3 (x, y, 1.0f);
		renderer [y, x] = newTile.AddComponent<SpriteRenderer> ();
		renderer [y, x].sprite = tileSprites [0];
		renderer [y, x].enabled = false;
	}

	public void Update () {
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				renderer [y, x].enabled = value [y, x] != EMPTY;
				if (value [y, x] != EMPTY && lastValue [y, x] != value [y, x]) {
					renderer [y, x].sprite = tileSprites [value [y, x]];
				}
			}
		}
		lastValue = (int[,])value.Clone ();
	}

	public bool Full () {
		foreach (int x in value) {
			if (x == EMPTY) {
				return false;
			}
		}
		return true;
	}

	public void AddRandom () {
		if (Full ()) {
			throw new UnityException ("Cannot add random tile! Grid is full.");
		}
		int x, y;
		do {
			x = generator.Next () % size;
			y = generator.Next () % size;
		} while (value [y, x] != EMPTY);
		value [y, x] = generator.Next () % 2;
	}

	public Grid (int size, Sprite[] tileSprites) {
		this.generator = new System.Random ();
		this.tileSprites = tileSprites;
		this.size = size;
		value = new int[size, size];
		tile = new GameObject[size, size];
		renderer = new SpriteRenderer[size, size];
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				InitTile (y, x);
			}
		}
		Reset ();
	}

	private bool MoveBuffer (int[] buffer) {
		bool result = false;
		int index = 0;
		for (int i = 0; i < buffer.Length; i++) {
			if (buffer [i] != EMPTY) {
				if (index != i) {
					result = true;
					buffer [index] = buffer [i];
					buffer [i] = EMPTY;
				}
				index++;
			}
		}
		return result;
	}

	private bool MergeBuffer (int[] buffer) {
		bool result = false;
		for (int i = 0; i < buffer.Length - 1; i++) {
			if (buffer [i] != EMPTY && buffer [i] == buffer [i + 1]) {
				result = true;
				buffer [i]++;
				buffer [i + 1] = EMPTY;
			}
		}
		return result;
	}

	private void GetBuffer(int[] buffer, int x, Direction direction) {
		bool vertical = direction.Vertical ();
		for (int i = 0; i < size; i++) {
			buffer [i] = vertical ? value [i, x] : value [x, i];
		}
		if (direction.Reversed ()) {
			System.Array.Reverse (buffer);
		}
	}

	private void SaveBuffer(int[] buffer, int x, Direction direction) {
		if (direction.Reversed ()) {
			System.Array.Reverse (buffer);
		}
		bool vertical = direction.Vertical ();
		for (int i = 0; i < size; i++) {
			if (vertical) {
				value [i, x] = buffer [i];
			} else {
				value [x, i] = buffer [i];
			}
		}

	}
		
	private bool Move (Direction direction) {
		bool result = false;
		int[] buffer = new int[size];
		for (int i = 0; i < size; i++) {
			GetBuffer (buffer, i, direction);
			result |= MoveBuffer (buffer);
			result |= MergeBuffer (buffer);
			result |= MoveBuffer (buffer);
			SaveBuffer (buffer, i, direction);
		}
		return result;
	}

	public void MakeMove(Direction direction) {
		if (Move (direction)) {
			AddRandom ();
		}
	}

	public void Reset() {
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				value [y, x] = EMPTY;
			}
		}
		lastValue = (int[,])value.Clone ();
		AddRandom ();
		AddRandom ();
	}
}
