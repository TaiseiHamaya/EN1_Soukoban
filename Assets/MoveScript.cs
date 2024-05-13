using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Move : MonoBehaviour {

	private const float timeTaken = 0.2f;
	private float timeElapsed;
	private Vector3 destination;
	private Vector3 origin;
	// Start is called before the first frame update
	void Start() {
		destination = transform.position;
		origin = destination;
	}

	// Update is called once per frame
	void Update() {
		if(origin == destination) {
			return;
		}
		timeElapsed += Time.deltaTime;
		float timeRate = timeElapsed / timeTaken;
		if(timeRate > 1) { timeRate = 1; }
		transform.position = Vector3.Lerp(origin, destination, Easing.InOutQuad(timeRate));
	}

	public void MoveTo(Vector3 newDestination) {
		timeElapsed = 0;
		origin = destination;
		transform.position = origin;
		destination = newDestination;
	}
}
