using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Sprite[] TileSprites;
    public Camera MainCamera;
    public Canvas GridCanvas;

    private Vector2 _touchStartPosition = Vector2.zero;
    private Grid _grid;

    private const int Size = 4;
    private const float MinSwipeDistance = 10.0f;


    private void Start() {
        const float center = Size / 2f - 0.5f;
        MainCamera.transform.position = new Vector3(center, center, -10.0f);
        _grid = new Grid(Size, TileSprites, GridCanvas);
    }

    private void InputEvents() {
        if (Input.GetKeyDown("escape")) {
            Application.Quit();
        }
        else if (Input.GetKeyDown("r")) {
            _grid.Reset();
        }
        if (Input.GetKeyDown("left")) {
            _grid.MakeMove(Direction.Left);
        }
        else if (Input.GetKeyDown("right")) {
            _grid.MakeMove(Direction.Right);
        }
        if (Input.GetKeyDown("down")) {
            _grid.MakeMove(Direction.Down);
        }
        else if (Input.GetKeyDown("up")) {
            _grid.MakeMove(Direction.Up);
        }
    }

    private void TouchEvents() {
        if (Input.touchCount == 0) {
            return;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began) {
            _touchStartPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(0).phase != TouchPhase.Ended) return;
        var swipeDelta = (Input.GetTouch(0).position - _touchStartPosition);
        if (swipeDelta.magnitude < MinSwipeDistance) {
            return;
        }
        swipeDelta.Normalize();
        if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
            _grid.MakeMove(Direction.Up);
        }
        else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f) {
            _grid.MakeMove(Direction.Down);
        }
        else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {
            _grid.MakeMove(Direction.Right);
        }
        else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f) {
            _grid.MakeMove(Direction.Left);
        }
    }

    private void Update() {
        InputEvents();
        TouchEvents();
        _grid.Update();
    }

    [UsedImplicitly]
    public void ResetLevel() {
        _grid.Reset();
    }
}
