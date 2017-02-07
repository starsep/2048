using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Sprite[] tileSprites;
	public Camera mainCamera;

	private Vector2 touchStartPosition = Vector2.zero;
	private Grid grid;

	private const int size = 4;
	private const float minSwipeDistance = 10.0f;


	void Start () {
		float center = size / 2f - 0.5f;
		mainCamera.transform.position = new Vector3 (center, center, -10.0f);
		grid = new Grid (size, tileSprites);
	}

	void InputEvents () {
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

	void TouchEvents () {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			touchStartPosition = Input.GetTouch (0).position;
		}
		if (Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 swipeDelta = (Input.GetTouch (0).position - touchStartPosition);
			if (swipeDelta.magnitude < minSwipeDistance) {
				return;
			}
			swipeDelta.Normalize ();
			if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
				grid.MakeMove (Direction.Up);
			} else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
				grid.MakeMove (Direction.Down);
			} else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {
				grid.MakeMove (Direction.Right);
			} else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {
				grid.MakeMove (Direction.Left);
			}
		}
	}

	void Update () {
		InputEvents ();
		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
		TouchEvents ();
		#endif
		grid.Update ();
	}
}
