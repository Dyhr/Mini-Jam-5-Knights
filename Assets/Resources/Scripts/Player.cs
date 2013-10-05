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
    private CharacterMotor motor;
	void Start () {
        motor = GetComponent<CharacterMotor>();
        if (motor == null)
            Destroy(gameObject);
        if (Camera.players == null)
            Camera.players = new ArrayList();
        Camera.players.Add(this);
	}

    void OnDestroy() {
        Camera.players.Remove(this);
    }
    void OnTriggerEnter(Collider other) {
        motor.canControl = false;
        Destroy(gameObject, 1);
    }
	
	void Update () {
        if (Input.GetAxis("Player" + (int)index + "X") != 0 || Input.GetAxis("Player" + (int)index + "Y") != 0) {
            Vector3 input = Input.GetAxis("Player" + (int)index + "X") * Camera.right + Input.GetAxis("Player" + (int)index + "Y") * Camera.up;
            input.y = 0;
            input.Normalize();
            transform.LookAt(transform.position + input);
            motor.inputMoveDirection = input;
        } else {
            motor.inputMoveDirection = Vector3.zero;
        }
        motor.inputJump = Input.GetButton("Player" + (int)index + "Jump");
        if (transform.position.y < -5)
            Destroy(gameObject);
	}
}
