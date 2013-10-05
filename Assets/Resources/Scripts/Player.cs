using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public enum Index {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
    public Index index;
    private float angle;
    private bool ali;
    private Transform sword;
    public bool attacking = false;
    public bool defending = false;
    public Vector3 swordRotOri;
    public Vector3 velocity;
    public bool jumping = false;

	void Start () {
        ali = true;
        velocity = Vector3.zero;
        GameObject[] swords = GameObject.FindGameObjectsWithTag("sword");
        foreach (GameObject sword in swords) {
            if (sword.transform.parent == transform) {
                this.sword = sword.transform;
                this.swordRotOri = this.sword.localEulerAngles;
                break;
            }
        }
        if (Camera.players == null)
            Camera.players = new ArrayList();
        Camera.players.Add(this);
	}

    void OnDestroy() {
        Camera.players.Remove(this);
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Finish") {
            ali = false;
            Destroy(gameObject, 0.2f);
        } else if (other.tag == "sword") {
            if (other.transform.parent.GetComponent<Player>() != this) {
                if (other.transform.parent.GetComponent<Player>().attacking && !defending) {
                    velocity = (transform.position - other.transform.position).normalized * 10;
                }
            }
        }
    }

    void OnExternalVelocity() {
    }
	
	void Update () {
        if (!ali)
            return;
        float h;
        if (transform.position.x > 1 && transform.position.z > 1 && transform.position.x < 31 && transform.position.z < 31) {
            float max = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (Floor.transforms[Mathf.FloorToInt(transform.position.x)+i, Mathf.FloorToInt(transform.position.z)+j].position.y > max)
                        max = Floor.transforms[Mathf.FloorToInt(transform.position.x)+j, Mathf.FloorToInt(transform.position.z)+j].position.y;
                }
            }
            h = max;
        } else
            h = 2;
        if (Input.GetAxis("Player" + (int)index + "X") != 0 || Input.GetAxis("Player" + (int)index + "Y") != 0) {
            Vector3 input = Input.GetAxis("Player" + (int)index + "X") * Camera.right + Input.GetAxis("Player" + (int)index + "Y") * Camera.up;
            input.y = 0;
            input.Normalize();
            if (defending || attacking)
                input *= 0.5f;
            transform.LookAt(Vector3.Lerp(transform.position+transform.forward,transform.position + input,0.5f));
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + input * Time.deltaTime * 4;
        }
        if (transform.position.y - 9 * Time.deltaTime < h + 3 && velocity.y - 5 < 0.001f) {
            jumping = false;
            velocity.y = 0;
        }
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y,Mathf.Max(transform.position.y-5*Time.deltaTime,h+3),0.5f), transform.position.z);
        transform.position += velocity* Time.deltaTime;
        velocity -= velocity.normalized * Mathf.Min(5,velocity.magnitude) * Time.deltaTime;

        if (Input.GetButton("Player" + (int)index + "Jump") && !jumping) {
            velocity.y = 10;
            jumping = true;
        }
        if (Input.GetButton("Player" + (int)index + "Attack")) {
            sword.localEulerAngles = swordRotOri + new Vector3(-90, Mathf.Sin(Time.time * 10) * 90 - 90, Mathf.Sin(Time.time * 10) * 90);
            attacking = true;
        } else {
            sword.localEulerAngles = swordRotOri;
            attacking = false;
        }
        if (Input.GetButton("Player" + (int)index + "Defend") && !attacking) {
            sword.localEulerAngles = swordRotOri + new Vector3(-90+Mathf.Sin(Time.time * 10) * 90, Mathf.Sin(Time.time * 10) * 90 - 90, 0);
            defending = true;
        } else {
            defending = false;
        }

        if (transform.position.y < -5)
            Destroy(gameObject);
	}
}
