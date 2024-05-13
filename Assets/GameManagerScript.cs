using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject BoxPrefab;
	public GameObject clearText;
	public GameObject goalPrefab;

	int[,] map;
	GameObject[,] field;
	readonly List<GameObject> goalsObj = new();
	readonly List<Vector2Int> goals = new();
	

	Vector2Int GetPlayerIndex() {
		for (int y = 0; y < field.GetLength(0); ++y) {
			for (int x = 0; x < field.GetLength(1); ++x) {
				if (!ReferenceEquals(field[y, x], null) && field[y, x].CompareTag("Player")) {
					return new Vector2Int(y, x);
				}
			}
		}
		return new Vector2Int(-1, -1);
	}

	Vector3 MakePositionFromIndex(Vector2Int index) {
		return new Vector3Int(index.y - field.GetLength(1) / 2, field.GetLength(0) - index.x - field.GetLength(0) / 2, 0);
	}

	bool MoveNumber(string tag, Vector2Int moveFromIndex, Vector2Int moveToIndex) {
		if (moveToIndex.x < 0 || moveToIndex.x >= field.GetLength(0) || moveToIndex.y < 0 || moveToIndex.y >= field.GetLength(1)) {
			return false;
		}
		else if (!ReferenceEquals(field[moveToIndex.x, moveToIndex.y], null) && field[moveToIndex.x, moveToIndex.y].CompareTag("Box")) {
			Vector2Int velocity = moveToIndex - moveFromIndex;

			bool success = MoveNumber(tag, moveToIndex, moveToIndex + velocity);
			if (!success) {
				return false;
			}
		}

		(field[moveToIndex.x, moveToIndex.y], field[moveFromIndex.x, moveFromIndex.y]) = (field[moveFromIndex.x, moveFromIndex.y], field[moveToIndex.x, moveToIndex.y]);

		if (!ReferenceEquals(field[moveToIndex.x, moveToIndex.y], null))
			field[moveToIndex.x, moveToIndex.y].GetComponent<Move>().MoveTo(MakePositionFromIndex(moveToIndex));
		if (!ReferenceEquals(field[moveFromIndex.x, moveFromIndex.y], null))
			field[moveFromIndex.x, moveFromIndex.y].GetComponent<Move>().MoveTo(MakePositionFromIndex(moveFromIndex));

		return true;
	}

	bool IsClear() {
		for(int i = 0; i < goals.Count; ++i) {
			GameObject obj = field[goals[i].x, goals[i].y];
			if(obj == null || !obj.CompareTag("Box")) {
				return false;
			}
		}
		return true;
	}

	// Start is called before the first frame update
	void Start() {
		map = new int[,] {
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 2, 0, 3, 3, 0, 0, 0 },
			{ 0, 0, 0, 2, 3, 1, 0, 2, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		};
		field = new GameObject[map.GetLength(0), map.GetLength(1)];
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {
				switch (map[y, x]) {
					case 0: // air
						break;
					case 1: // player
						field[y, x] = Instantiate(
							playerPrefab,
							MakePositionFromIndex(new Vector2Int(y, x)),
							Quaternion.identity
							);
						break;
					case 2: // box
						field[y, x] = Instantiate(
							BoxPrefab,
							MakePositionFromIndex(new Vector2Int(y, x)),
							Quaternion.identity
							);
						break;
					case 3:
						goals.Add(new Vector2Int(y, x));
						Vector3 tempPos = MakePositionFromIndex(new Vector2Int(y, x));
						tempPos.z = 0.01f;
						goalsObj.Add(Instantiate(
							goalPrefab,
							tempPos, 
							Quaternion.identity
							));
						break;
				}
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();
			MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
		}
		if (IsClear()) {
			clearText.SetActive(true);
		}
	}
}
