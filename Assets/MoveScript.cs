using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class Move : MonoBehaviour {

	public GameObject particlePrefab;

	private GameObject particleSystemObject;

	private const float timeTaken = 0.2f;
	private float timeElapsed;
	private Vector3 destination;
	private Vector3 origin;

	// Start is called before the first frame update
	void Start() {
		if (particlePrefab != null) {
			particleSystemObject = Instantiate(
				particlePrefab, Vector3.zero, Quaternion.identity
				);
		}
		destination = transform.position;
		origin = destination;
	}

	// Update is called once per frame
	void Update() {
		if (origin == destination) {
			return;
		}
		timeElapsed += Time.deltaTime;
		float timeRate = timeElapsed / timeTaken;
		if (timeRate > 1) { timeRate = 1; }
		Vector3 move = Vector3.Lerp(origin, destination, Easing.InOutQuad(timeRate));

		transform.position = move;
	}

	public void MoveTo(Vector3 newDestination) {
		timeElapsed = 0;
		origin = destination;
		transform.position = origin;
		destination = newDestination;
		if (particleSystemObject != null) {
			particleSystemObject.transform.position = origin;
			particleSystemObject.transform.rotation = Quaternion.LookRotation((origin - destination).normalized, new Vector3(0,1,0));
			particleSystemObject.SetActive(true);
			particleSystemObject.GetComponent<ParticleSystem>().Play();
		}
	}
}
