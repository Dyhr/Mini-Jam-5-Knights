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
    //private CharacterMotor motor;
    private Transform sword;
    public bool attacking = false;
    public Vector3 swordRotOri;

	void Start () {
        GameObject[] swords = GameObject.FindGameObjectsWithTag("sword");
        foreach (GameObject sword in swords) {
            if (sword.transform.parent == transform) {
                this.sword = sword.transform;
                this.swordRotOri = this.sword.localEulerAngles;
                break;
            }
        }
        //motor = GetComponent<CharacterMotor>();
        //if (motor == null)
        //    Destroy(gameObject);
        if (Camera.players == null)
            Camera.players = new ArrayList();
        Camera.players.Add(this);
	}

    void OnDestroy() {
        Camera.players.Remove(this);
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Finish") {
            //motor.canControl = false;
            Destroy(gameObject, 1);
        } else if (other.tag == "sword") {
            if (other.transform.parent.GetComponent<Player>() != this) {
                if (other.transform.parent.GetComponent<Player>().attacking) {
                    //motor.SetVelocity((transform.position - other.transform.position).normalized * 15);
                }
            }
        }
    }

    void OnExternalVelocity() {
    }
	
	void Update () {
        //if (!motor.canControl)
            //return;
        float h;
        if (transform.position.x > 0 && transform.position.z > 0 && transform.position.x < 32 && transform.position.z < 32) {
            float average = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    average += Floor.transforms[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)].position.y + 3;
                }
            }
            average /= 9;
            h = average;
        } else
            h = 2;
        if (Input.GetAxis("Player" + (int)index + "X") != 0 || Input.GetAxis("Player" + (int)index + "Y") != 0) {
            Vector3 input = Input.GetAxis("Player" + (int)index + "X") * Camera.right + Input.GetAxis("Player" + (int)index + "Y") * Camera.up;
            input.y = 0;
            input.Normalize();
            transform.LookAt(transform.position + input);
            //if (motor != null && motor.enabled) {
                //motor.inputMoveDirection = input;
           // } else {
                transform.position = new Vector3(transform.position.x, h, transform.position.z) + input*Time.deltaTime * 10;
            //}
        } else {
            //if (motor != null) motor.inputMoveDirection = Vector3.zero;
            transform.position = new Vector3(transform.position.x, h, transform.position.z);
        }
        //motor.inputJump = Input.GetButton("Player" + (int)index + "Jump");

        if (Input.GetButton("Player" + (int)index + "Attack")) {
            sword.localEulerAngles = swordRotOri + new Vector3(-90, Mathf.Sin(Time.time * 10) * 90 - 90, Mathf.Sin(Time.time * 10) * 90);
            attacking = true;
        } else {
            sword.localEulerAngles = swordRotOri;
            attacking = false;
        }

        if (transform.position.y < -5)
            Destroy(gameObject);
	}
}
