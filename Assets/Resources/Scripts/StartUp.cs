using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {
    public static bool[] joined = new bool[4];
    private Transform banner;
    private Transform[] playerMarkers;
    private int[] indices;

	void Start () {
        banner = (Instantiate(Resources.Load("banner0")) as GameObject).transform;
        banner.localScale *= 3;
        playerMarkers = new Transform[4];
        indices = new int[]{1,2,0,3};
        for (int i = 0; i < 4; i++) {
            joined[i] = false;
            playerMarkers[i] = (GameObject.CreatePrimitive(PrimitiveType.Capsule)).transform;
            playerMarkers[i].localScale *= 4;
            playerMarkers[i].renderer.castShadows = false;
            switch (i) {
                case 0:
                    playerMarkers[i].renderer.material.color = Color.Lerp(Color.white,Color.green,0.6f);
                    break;
                case 1:
                    playerMarkers[i].renderer.material.color = Color.Lerp(Color.white, Color.red, 0.6f);
                    break;
                case 2:
                    playerMarkers[i].renderer.material.color = Color.Lerp(Color.white, Color.blue, 0.6f);
                    break;
                case 3:
                    playerMarkers[i].renderer.material.color = Color.Lerp(Color.white, Color.yellow, 0.6f);
                    break;
            }
        }
	}

    void Update() {
        banner.position = Camera.main.transform.position + Camera.main.transform.up * 9 + Camera.main.transform.forward * 5;
        banner.rotation = Camera.main.transform.rotation;
        banner.Rotate(new Vector3(-90, 0, 0));
        banner.Rotate(new Vector3(0, 180, 0));
        for (int i = 0; i < 4; i++) {
            if(Input.GetButtonDown("Player" + (i + 1) + "Attack"))
                joined[i] = !joined[i];
            playerMarkers[i].position = Camera.main.transform.position + Camera.main.transform.right * (-30 + i*20) - Camera.main.transform.up * 16 + Camera.main.transform.forward * 5;
            playerMarkers[i].rotation = Camera.main.transform.rotation;
            playerMarkers[indices[i]].renderer.enabled = joined[i];
        }
        if(Input.GetButton("Player1Jump")){
            int numPlayers = 0;
            for (int i = 0; i < 4; i++) {
                if (joined[i])
                    numPlayers++;
            }
            if (numPlayers == 0)
                return;
            Cam.alone = numPlayers < 2;
            for (int i = 0; i < 4; i++) {
                playerMarkers[indices[i]].renderer.enabled = false;
            }

            GetComponent<Floor>().init();
            banner.renderer.enabled = false;
            enabled = false;
        }
	}
}
