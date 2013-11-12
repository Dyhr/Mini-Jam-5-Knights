using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {
    public static bool[] joined = new bool[10];
    private Transform banner;
    private Transform[] playerMarkers;
    private int[] indices;
    private Hashtable players;

	void Start () {
        players = new Hashtable();
        banner = (Instantiate(Resources.Load("banner0")) as GameObject).transform;
        banner.localScale *= 3;
        playerMarkers = new Transform[4];
        indices = new int[]{1,2,0,3};
        for (int i = 0; i < 4; i++) {
            joined[i] = false;
            playerMarkers[i] = (Instantiate(Resources.Load("DummyPlayer")) as GameObject).transform;
            playerMarkers[i].localScale *= 4;
            playerMarkers[i].renderer.castShadows = false;
            playerMarkers[i].tag = "Untagged";
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
        banner.position = Camera.main.transform.position + Camera.main.transform.up * 8 + Camera.main.transform.forward * 5;
        banner.rotation = Camera.main.transform.rotation;
        banner.Rotate(new Vector3(-90, 0, 0));
        banner.Rotate(new Vector3(0, 180, 0));
        for (int i = 0; i < 4; i++) {
            //if(Input.GetButtonDown("Player" + (i + 1) + "Attack"))
                //joined[i] = !joined[i];
            if (playerMarkers[i] == null) {
                continue;
            }
            playerMarkers[i].position = Camera.main.transform.position + Camera.main.transform.right * (-30 + i*20) - Camera.main.transform.up * 16 + Camera.main.transform.forward * 5;
            playerMarkers[i].rotation = Camera.main.transform.rotation;
			playerMarkers[i].Rotate(0,180,0);
            playerMarkers[i].renderer.enabled = players.ContainsValue(i+1);
			for(int j = 0;j < playerMarkers[indices[i]].childCount; j++){
				if(playerMarkers[indices[i]].GetChild(j).renderer != null)
            		playerMarkers[indices[i]].GetChild(j).renderer.enabled = joined[i];
			}
        }


        for (int i = 1; i <= 4; i++) {
            if (Input.GetKeyDown("joystick " + i + " button 0")) {
                if (!players.ContainsKey(i)) {
                    for (int j = 1; j <= 10; j++) {
                        if (!players.ContainsValue(j)) {
                            players.Add(i, j);
                            break;
                        }
                    }
                } else {
                    players.Remove(i);
                }
            }
            if (Input.GetKeyDown("joystick " + i + " button 7") && players.Keys.Count > 0) {
                int numPlayers = players.Keys.Count;
                if (numPlayers == 0)
                    return;
                Cam.alone = numPlayers < 2;

                for (int j = 0; j < 4; j++) {
                    Destroy(playerMarkers[j].gameObject);
                }

                ArrayList picked = new ArrayList();
                GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");
                if (spawns.Length == 0) {
                    return;
                }
                foreach (int j in players.Keys) {
                    if (picked.Count == spawns.Length) {
                        picked.Clear();
                    }
                    GameObject spawn;
                    do{
                        spawn = spawns[(int)(Random.value*spawns.Length)];
                    }while(picked.Contains(spawn));
                    picked.Add(spawn);

                    Transform pl = (Instantiate(Resources.Load("Prefabs/Player")) as GameObject).transform;
                    pl.position = spawn.transform.position;
                    pl.GetComponent<Character.Player>().index = j;
                }

                banner.renderer.enabled = false;
                enabled = false;
            }
        }
        /*if(Input.GetButton("Start")){
            int numPlayers = 0;
            for (int i = 0; i < 4; i++) {
                if (joined[i])
                    numPlayers++;
            }
            if (numPlayers == 0)
                return;
            Cam.alone = numPlayers < 2;
            for (int i = 0; i < 4; i++) {
                Destroy(playerMarkers[i].gameObject);
            }

            GetComponent<Floor>().init();
            banner.renderer.enabled = false;
            enabled = false;
        }*/
    }
}
