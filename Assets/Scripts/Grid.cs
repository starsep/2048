using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Grid {
    private const int Empty = -1;
    private readonly System.Random _generator;

    private readonly Sprite[] _tileSprites;
    private readonly SpriteRenderer[,] _renderer;
    private readonly int[,] _value;
    private int[,] _lastValue;
    private readonly int _size;

    private void InitTile(int x, int y) {
        _value[y, x] = Empty;
        var newTile = new GameObject("Tile[" + y + "," + x + "]");
        newTile.transform.position = new Vector3(x, y, 1.0f);
        _renderer[y, x] = newTile.AddComponent<SpriteRenderer>();
        _renderer[y, x].sprite = _tileSprites[0];
        _renderer[y, x].enabled = false;
    }

    public void Update() {
        for (var y = 0; y < _size; y++) {
            for (var x = 0; x < _size; x++) {
                _renderer[y, x].enabled = _value[y, x] != Empty;
                if (_value[y, x] != Empty && _lastValue[y, x] != _value[y, x]) {
                    _renderer[y, x].sprite = _tileSprites[_value[y, x]];
                }
            }
        }
        _lastValue = (int[,]) _value.Clone();
    }

    private bool Full() {
        return _value.Cast<int>().All(x => x != Empty);
    }

    private void AddRandom() {
        if (Full()) {
            throw new UnityException("Cannot add random tile! Grid is full.");
        }
        int x, y;
        do {
            x = _generator.Next() % _size;
            y = _generator.Next() % _size;
        } while (_value[y, x] != Empty);
        _value[y, x] = _generator.Next() % 2;
    }

    public Grid(int size, Sprite[] tileSprites) {
        _generator = new System.Random();
        _tileSprites = tileSprites;
        _size = size;
        _value = new int[size, size];
        _renderer = new SpriteRenderer[size, size];
        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                InitTile(y, x);
            }
        }
        Reset();
    }

    private static bool MoveBuffer([NotNull] IList<int> buffer) {
        if (buffer == null) throw new ArgumentNullException("buffer");
        var result = false;
        var index = 0;
        for (var i = 0; i < buffer.Count; i++) {
            if (buffer[i] == Empty) continue;
            if (index != i) {
                result = true;
                buffer[index] = buffer[i];
                buffer[i] = Empty;
            }
            index++;
        }
        return result;
    }

    private static bool MergeBuffer(IList<int> buffer) {
        var result = false;
        for (var i = 0; i < buffer.Count - 1; i++) {
            if (buffer[i] == Empty || buffer[i] != buffer[i + 1]) continue;
            result = true;
            buffer[i]++;
            buffer[i + 1] = Empty;
        }
        return result;
    }

    private void GetBuffer(int[] buffer, int x, Direction direction) {
        var vertical = direction.Vertical();
        for (var i = 0; i < _size; i++) {
            buffer[i] = vertical ? _value[i, x] : _value[x, i];
        }
        if (direction.Reversed()) {
            Array.Reverse(buffer);
        }
    }

    private void SaveBuffer(int[] buffer, int x, Direction direction) {
        if (direction.Reversed()) {
            Array.Reverse(buffer);
        }
        var vertical = direction.Vertical();
        for (var i = 0; i < _size; i++) {
            if (vertical) {
                _value[i, x] = buffer[i];
            }
            else {
                _value[x, i] = buffer[i];
            }
        }
    }

    private bool Move(Direction direction) {
        var result = false;
        var buffer = new int[_size];
        for (var i = 0; i < _size; i++) {
            GetBuffer(buffer, i, direction);
            result |= MoveBuffer(buffer);
            result |= MergeBuffer(buffer);
            result |= MoveBuffer(buffer);
            SaveBuffer(buffer, i, direction);
        }
        return result;
    }

    public void MakeMove(Direction direction) {
        if (Move(direction)) {
            AddRandom();
        }
    }

    public void Reset() {
        for (var y = 0; y < _size; y++) {
            for (var x = 0; x < _size; x++) {
                _value[y, x] = Empty;
            }
        }
        _lastValue = (int[,]) _value.Clone();
        AddRandom();
        AddRandom();
    }
}
