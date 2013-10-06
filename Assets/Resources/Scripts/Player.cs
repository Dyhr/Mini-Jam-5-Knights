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
    public bool ali;
    private Transform sword;
    public bool attacking = false;
    public bool defending = false;
    public Vector3 swordRotOri;
    public Vector3 velocity;
    public bool jumping = false;
    public float defaultWalkSpeed = 5;
    public float defaultTurnSpeed = 0.1f;
    private float walkSpeed;
    private float turnSpeed;
    private float gravity = 18;

	void Start () {
        ali = false;
        velocity = Vector3.zero;
        walkSpeed = defaultWalkSpeed;
        turnSpeed = defaultTurnSpeed;
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

        makeHidden(transform, "Hidden");
	}
    public void makeHidden(Transform t, string layer) {
        t.gameObject.layer = LayerMask.NameToLayer(layer);
        for (int i = 0; i < t.childCount; i++) {
            makeHidden(t.GetChild(i), layer);
        }
    }

    void OnDestroy() {
        ali = false;
        Camera.players.Remove(this);
        if (Camera.players.Count == 0) {
            Application.LoadLevel(0);
        }
    }
    void OnTriggerEnter(Collider other) {
        Debug.Log("Log");
        if (!ali)
            return;
        if (other.tag == "Finish") {
            ali = false;
            Destroy(gameObject, 0.2f);
        } else if (other.tag == "sword") {
            if (other.transform.parent.GetComponent<Player>() != this) {
                if (other.transform.parent.GetComponent<Player>().attacking &&
                    !(defending && Vector3.Angle(transform.forward, transform.position - other.transform.parent.position) < 180)) {
                    velocity = (transform.position - other.transform.parent.position).normalized * 10;
                }
            }
        }
    }
	
	void Update () {
        if (!ali)
            return;
        float h;
        walkSpeed = defaultWalkSpeed;
        turnSpeed = defaultTurnSpeed;
        if (transform.position.x > 0 && transform.position.z > 0 && transform.position.x < Floor.width && transform.position.z < Floor.height) {
            float max = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (!(Mathf.Floor(transform.position.x) + i > 0 && Mathf.Floor(transform.position.z) + j > 0 && Mathf.Floor(transform.position.x) + i < Floor.width && Mathf.Floor(transform.position.z) + j < Floor.height))
                        continue;
                    if (Floor.transforms[Mathf.FloorToInt(transform.position.x) + i, Mathf.FloorToInt(transform.position.z) + j].position.y > max)
                        max = Floor.transforms[Mathf.FloorToInt(transform.position.x) + j, Mathf.FloorToInt(transform.position.z) + j].position.y;
                }
            }
            h = max;
        } else {
            h = -1;
        }
        if (Input.GetAxis("Player" + (int)index + "X") != 0 || Input.GetAxis("Player" + (int)index + "Y") != 0) {
            Vector3 input = Input.GetAxis("Player" + (int)index + "X") * Camera.right + Input.GetAxis("Player" + (int)index + "Y") * Camera.up;
            input.y = 0;
            input.Normalize();
            if (attacking) {
                walkSpeed *= 1.3f;
                turnSpeed *= 0.05f;
            } else if (defending) {
                walkSpeed *= 0.4f;
                turnSpeed *= 0.5f;
            }
            //transform.LookAt(Vector3.Lerp(transform.position+transform.forward,transform.position + input,0.5f));
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(input),turnSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + transform.forward * Time.deltaTime * walkSpeed;
        }
        velocity.y -= gravity * Time.deltaTime;
        if (transform.position.y - 9 * Time.deltaTime < h + 2.5f && velocity.y < 0.001f) {
            jumping = false;
            velocity.y = 0;
        }
        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, Mathf.Max(transform.position.y, h + 2.5f), 0.5f), transform.position.z);
        velocity -= velocity.normalized * Mathf.Min(5,velocity.magnitude) * Time.deltaTime;

        if (Input.GetButton("Player" + (int)index + "Jump") && !jumping) {
            velocity.y = 10;
            jumping = true;
        }
        if (Input.GetButton("Player" + (int)index + "Attack")) {
            sword.localEulerAngles = swordRotOri + new Vector3(0, Mathf.Sin(Time.time * 10) * 90, 0);
            attacking = true;
        } else {
            sword.localEulerAngles = swordRotOri;
            attacking = false;
        }
        if (Input.GetButton("Player" + (int)index + "Defend") && !attacking) {
            sword.localEulerAngles = swordRotOri + new Vector3(-90, 0, Mathf.Sin(Time.time * 20) * 360);
            defending = true;
        } else {
            defending = false;
        }

        if (transform.position.y < -5)
            Destroy(gameObject);
	}
}
