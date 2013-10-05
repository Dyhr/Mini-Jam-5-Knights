using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
    public static ArrayList players;
    public static Vector3 up = Vector3.forward;
    public static Vector3 right = Vector3.right;

	// Use this for initialization
	void Start () {
        if(players == null)
            players = new ArrayList(4);
	}
	
	// Update is called once per frame
	void Update () {
        if (players.Count > 0) {
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
            transform.position = Vector3.Lerp(transform.position,average + Vector3.one * 20,0.2f);
            transform.LookAt(transform.position - Vector3.one);

            if(players.Count > 1)
                camera.orthographicSize = dist + 2;
            else
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize,12,0.5f);

            right = transform.right;
            up = transform.forward;
        }
	}
}
