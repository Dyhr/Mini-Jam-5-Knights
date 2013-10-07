using UnityEngine;
using System.Collections;

public class PartEffect : MonoBehaviour {

    void Start() {
        if (!GetComponent<ParticleSystem>().isPlaying) {
            GetComponent<ParticleSystem>().Play();
        }
	}
	
	void Update () {
        if (!GetComponent<ParticleSystem>().isPlaying) {
            Destroy(gameObject);
        }
	}
}
