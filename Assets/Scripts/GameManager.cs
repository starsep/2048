using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Sprite[] tileSprites;
	public Camera mainCamera;

	private Grid grid;
	private const int size = 4;

	void Start () {
		float center = size / 2f - 0.5f;
		mainCamera.transform.position = new Vector3 (center, center, -10.0f);
		grid = new Grid (size, tileSprites);
	}

	void InputEvents() {
		if (Input.GetKeyDown ("escape")) {
			Application.Quit ();
		} else if (Input.GetKeyDown ("r")) {
			grid.Reset ();
		}
		if (Input.GetKeyDown ("left")) {
			grid.MakeMove (Direction.Left);
		} else if (Input.GetKeyDown ("right")) {
			grid.MakeMove (Direction.Right);
		}
		if (Input.GetKeyDown ("down")) {
			grid.MakeMove (Direction.Down);
		} else if (Input.GetKeyDown ("up")) {
			grid.MakeMove (Direction.Up);
		}
	}

	void Update() {
		InputEvents ();
		grid.Update ();
	}
}
