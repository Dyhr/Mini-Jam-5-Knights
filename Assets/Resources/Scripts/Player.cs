using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private float angle;
    CharacterMotor motor;
	void Start () {
        motor = GetComponent<CharacterMotor>();
        if (motor == null)
            Destroy(this);
	}
	
	void Update () {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            transform.LookAt(transform.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            motor.inputMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        } else {
            motor.inputMoveDirection = Vector3.zero;
        }
        //motor.inputMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}
}
