using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {
    private bool inited = false;

	void Start () {
	}
    public void init(string path) {
        transform.position = Camera.main.transform.position;
        gameObject.AddComponent<AudioSource>();
        GetComponent<AudioSource>().clip = Resources.Load(path) as AudioClip;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
        inited = true;
    }

    void Update() {
        transform.position = Camera.main.transform.position;
        if (!GetComponent<AudioSource>().isPlaying && inited) {
            Destroy(gameObject);
        }
	}
}
