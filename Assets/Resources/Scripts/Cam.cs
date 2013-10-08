using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {
    public static ArrayList players;
    public static bool alone = false;
    public static float destroy = -1;
    public static Vector3 up = Vector3.forward;
    public static Vector3 right = Vector3.right;
    private AudioSource music;
    public AudioClip[] clips;

	void Start () {
        destroy = -1;
        if (players == null)
            players = new ArrayList(4);
        music = gameObject.AddComponent<AudioSource>();
	}

    void Update() {
        if (destroy >= 0)
            destroy += Time.deltaTime;
        if(destroy > 2.25f)
            Application.LoadLevel(0);
        if (players.Count > 0) {
            if (!music.isPlaying && clips.Length > 0) {
                music.clip = clips[Mathf.FloorToInt(Random.value * clips.Length)];
                music.loop = true;
                music.volume = 0.3f;
                music.Play();
            }
            Vector3 average = Vector3.zero;
            float dist = 0;
            foreach (Player p in players) {
                average += p.transform.position;
                foreach (Player p2 in players) {
                    if (p != p2) {
                        if (Vector3.Distance(p.transform.position, p2.transform.position) > dist)
                            dist = Vector3.Distance(p.transform.position, p2.transform.position);
                    }
                }
            }
            average /= players.Count;
            transform.position = Vector3.Lerp(transform.position,average + (Vector3.one + Vector3.up) * 8,0.2f);
            transform.LookAt(transform.position - (Vector3.one + Vector3.up));

            if(players.Count > 1)
                camera.orthographicSize = dist + 3;
            else
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize,12,0.5f);

            right = transform.right;
            up = transform.forward;
        }
	}
}
