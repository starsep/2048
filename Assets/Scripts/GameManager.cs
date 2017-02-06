using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public GameObject[] tiles;

	// Use this for initialization
	void Start () {
		foreach (GameObject tile in tiles) {
			Instantiate (tile);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
