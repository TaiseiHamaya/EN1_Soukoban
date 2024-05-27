using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject boxPrefab;
	public GameObject wallPrefab;
	public GameObject backgroundPrefab;
	public GameObject clearText;
	public GameObject clearParticle1;
	public GameObject clearParticle2;
	public GameObject goalPrefab;

	int[,] map;
	GameObject[,] field;
	readonly List<GameObject> goalsObj = new();
	GameObject[,] backgroundObjects;
	readonly List<Vector2Int> goals = new();
	GameObject playerObject;


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
		else if (!ReferenceEquals(field[moveToIndex.x, moveToIndex.y], null) && field[moveToIndex.x, moveToIndex.y].CompareTag("Wall")) {
			return false;
		}

		(field[moveToIndex.x, moveToIndex.y], field[moveFromIndex.x, moveFromIndex.y]) = (field[moveFromIndex.x, moveFromIndex.y], field[moveToIndex.x, moveToIndex.y]);

		if (!ReferenceEquals(field[moveToIndex.x, moveToIndex.y], null)) {
			if (field[moveToIndex.x, moveToIndex.y].CompareTag("Player")) {
				Vector3 nextPos = MakePositionFromIndex(moveToIndex);
				nextPos.z = 0.5f;
				field[moveToIndex.x, moveToIndex.y].GetComponent<Move>().MoveTo(nextPos);
			}
			else {
				field[moveToIndex.x, moveToIndex.y].GetComponent<Move>().MoveTo(MakePositionFromIndex(moveToIndex));
			}
		}
		if (!ReferenceEquals(field[moveFromIndex.x, moveFromIndex.y], null)) {
			if (field[moveFromIndex.x, moveFromIndex.y].CompareTag("Player")) {
				Vector3 nextPos = MakePositionFromIndex(moveToIndex);
				nextPos.z = 0.5f;
				field[moveFromIndex.x, moveFromIndex.y].GetComponent<Move>().MoveTo(nextPos);
			}
			else {
				field[moveFromIndex.x, moveFromIndex.y].GetComponent<Move>().MoveTo(MakePositionFromIndex(moveToIndex));
			}
		}

		return true;
	}

	bool IsClear() {
		for (int i = 0; i < goals.Count; ++i) {
			GameObject obj = field[goals[i].x, goals[i].y];
			if (obj == null || !obj.CompareTag("Box")) {
				return false;
			}
		}
		return true;
	}

	// Start is called before the first frame update
	void Start() {

		Screen.SetResolution(1280, 720, false);

		map = new int[,] {
			{ 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
			{ 4, 0, 0, 3, 0, 0, 2, 0, 0, 0, 4 },
			{ 4, 0, 0, 0, 0, 0, 4, 0, 0, 0, 4 },
			{ 4, 4, 4, 4, 2, 1, 4, 0, 0, 0, 4 },
			{ 4, 0, 0, 0, 0, 0, 4, 4, 2, 4, 4 },
			{ 4, 0, 0, 0, 4, 0, 0, 0, 0, 3, 4 },
			{ 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
		};

		field = new GameObject[map.GetLength(0), map.GetLength(1)];
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {
				switch (map[y, x]) {
					case 0: // air
						break;
					case 1: // player
						{
							Vector3 tempPos = MakePositionFromIndex(new Vector2Int(y, x));
							tempPos.z = 0.5f;
							playerObject = Instantiate(
								playerPrefab,
								tempPos,
								Quaternion.Euler(-90, 0, 0)
								);
							field[y, x] = playerObject;
							break;
						}
					case 2: // box
						field[y, x] = Instantiate(
							boxPrefab,
							MakePositionFromIndex(new Vector2Int(y, x)),
							Quaternion.identity
							);
						break;
					case 3: // goal
						{
							goals.Add(new Vector2Int(y, x));
							Vector3 tempPos = MakePositionFromIndex(new Vector2Int(y, x));
							tempPos.z = 0.47f;
							goalsObj.Add(Instantiate(
								goalPrefab,
								tempPos,
								Quaternion.identity
								));
							break;
						}
					case 4:// wall
						field[y, x] = Instantiate(
							wallPrefab,
							MakePositionFromIndex(new Vector2Int(y, x)),
							Quaternion.identity
							);
						break;
				}
			}
		}

		backgroundObjects = new GameObject[map.GetLength(0), map.GetLength(1)];
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {
				Vector3 temppos = MakePositionFromIndex(new Vector2Int(y, x));
				temppos.z = 0.49f;
				backgroundObjects[y, x] = Instantiate(
							backgroundPrefab,
							temppos,
							Quaternion.Euler(-90, 0, 0)
							);
				backgroundObjects[y, x].transform.rotation *= Quaternion.Euler(0, 90 * UnityEngine.Random.Range(0, 3), 0);
				backgroundObjects[y, x].transform.localScale = new Vector3(0.75f, 1, 0.75f);
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
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(0);
		}
		if (IsClear()) {
			clearParticle1.SetActive(true);
			clearParticle2.SetActive(true);
			clearText.SetActive(true);
		}
	}
}
