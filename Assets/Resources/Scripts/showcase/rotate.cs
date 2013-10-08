using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {
	
	public float speed = 10f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}
