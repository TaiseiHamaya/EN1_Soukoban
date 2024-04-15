using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

	public GameObject playerPrefab;

	int[,] map;
	GameObject[,] field;

	Vector2Int GetPlayerIndex() {
		for (int y = 0; y < field.GetLength(0); ++y) {
			for (int x = 0; x < field.GetLength(1); ++x) {
				if (field[x, y].tag == "Player") {
					return new Vector2Int(x, y);
				}
			}
		}
		return new Vector2Int(-1, -1);
	}

	bool MoveNumber(int number, Vector2Int moveFrom, Vector2Int moveTo) {
		if (moveTo.x < 0 || moveTo.x >= field.GetLength(0) || moveTo.y < 0 || moveTo.y >= field.GetLength(1)) {
			return false;
		}
		else if (map[moveTo.x, moveTo.y] == 2) {
			Vector2Int velocity = moveTo - moveFrom;

			bool success = MoveNumber(2, moveTo, moveTo + velocity);
			if (!success) {
				return false;
			}
		}
		field[moveTo.x, moveTo.y].transform.position = new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
		field[moveFrom.x, moveFrom.y].transform.position = new Vector3(moveFrom.x, field.GetLength(0) - moveFrom.y, 0);
		return true;
	}

	// Start is called before the first frame update
	void Start() {

		map = new int[,] {
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 2, 0, 0, 2, 1, 2, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		};
		field = new GameObject[map.GetLength(0), map.GetLength(1)];
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {
				if (map[y, x] == 1) {
					field[y, x] = Instantiate(
						playerPrefab,
						new Vector3(x - map.GetLength(1) / 2, map.GetLength(0) - y, -3),
						Quaternion.identity
						);
				}
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();

			MoveNumber(1, playerIndex, playerIndex + Vector2Int.right);
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Vector2Int playerIndex = GetPlayerIndex();

			MoveNumber(1, playerIndex, playerIndex - Vector2Int.left);
		}
	}
}
