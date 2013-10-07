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

    public bool ali;
    public bool attacking = false;
    public bool defending = false;
    public bool jumping = false;

    private float attackTimer = 0;
    private float attackPause = 0;
    private float defendTimer = 0;

    private Transform sword;
    private Transform shield;
    private Vector3 swordRotOri;
    private Vector3 swordPosOri;
    private Vector3 swordScaOri;
    private Vector3 shieldRotOri;
    private Vector3 shieldPosOri;
    private Vector3 shieldScaOri;

    public Vector3 velocity;

    public float defaultWalkSpeed = 5;
    public float defaultTurnSpeed = 0.2f;
    private float walkSpeed;
    private float turnSpeed;
    private float gravity = 18;
	
	public float victory = 0;

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
                this.swordPosOri = this.sword.localPosition;
                this.swordScaOri = this.sword.localScale;
                break;
            }
        }
        GameObject[] shields = GameObject.FindGameObjectsWithTag("shield");
        foreach (GameObject shield in shields) {
            if (shield.transform.parent == transform) {
                this.shield = shield.transform;
                this.shieldRotOri = this.shield.localEulerAngles;
                this.shieldPosOri = this.shield.localPosition;
                this.shieldScaOri = this.shield.localScale;
                break;
            }
        }
        if (Cam.players == null)
            Cam.players = new ArrayList();
        Cam.players.Add(this);

        switch ((int)index) {
            case 1:
                renderer.material.color = Color.Lerp(Color.white, Color.red, 0.6f);
                break;
            case 2:
                renderer.material.color = Color.Lerp(Color.white, Color.blue, 0.6f);
                break;
            case 3:;
                renderer.material.color = Color.Lerp(Color.white, Color.green, 0.6f);
                break;
            case 4:
                renderer.material.color = Color.Lerp(Color.white, Color.yellow, 0.6f);
                break;
        }

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
        Cam.players.Remove(this);
        if ((Cam.players.Count == 0 && Cam.alone) || (Cam.players.Count == 1 && !Cam.alone)) {
            Cam.destroy = 0;
			foreach(Player p in Cam.players){
				p.victory = 1;
			}
        }
    }
    void OnTriggerEnter(Collider other) {
        if (!ali)
            return;
        if (other.tag == "Finish") {
            ali = false;
            Destroy(gameObject, 0.2f);
        } else if (other.tag == "sword") {
            if (other.transform.parent.GetComponent<Player>() != this) {
                if (other.transform.parent.GetComponent<Player>().attacking){
                    Instantiate(Resources.Load("HitParticle"), (transform.position + other.transform.parent.position) / 2, Quaternion.identity);
                    if (!(defending && Vector3.Angle(transform.forward, transform.position - other.transform.parent.position) < 180)) {
                        SoundEffect s = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
                        s.init("Sounds/hit_" + Mathf.CeilToInt(Random.value * 3));
                        velocity = (transform.position - other.transform.parent.position).normalized * 8;
                    } else {
                        SoundEffect s = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
                        s.init("Sounds/defend_" + Mathf.CeilToInt(Random.value * 3));
                        velocity = (transform.position - other.transform.parent.position).normalized * 2;
						other.transform.parent.GetComponent<Player>().attackPause = 1.5f;
                        other.transform.parent.GetComponent<Player>().velocity = -(transform.position - other.transform.parent.position).normalized * 6;
                    }
                }
            }
        }
    }
	
	void Update () {
        if (!ali)
            return;
		if(victory > 0){
			transform.Rotate(new Vector3(0,60*victory*Time.deltaTime,0));
			transform.Translate(new Vector3(0,0.5f*victory*Time.deltaTime,0));
			victory += Time.deltaTime*10;
			return;
		}
		Transform tile;
        float h;
        walkSpeed = defaultWalkSpeed;
        turnSpeed = defaultTurnSpeed;
        if (transform.position.x > 0 && transform.position.z > 0 && transform.position.x < Floor.width && transform.position.z < Floor.height) {
            float max = 0;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (!(Mathf.Floor(transform.position.x) + i > 0 && Mathf.Floor(transform.position.z) + j > 0 && Mathf.Floor(transform.position.x) + i < Floor.width && Mathf.Floor(transform.position.z) + j < Floor.height))
                        continue;
					tile = Floor.transforms[Mathf.FloorToInt(transform.position.x) + i, Mathf.FloorToInt(transform.position.z) + j];
                    if (tile.position.y > max && 
						Vector3.Distance(tile.position-Vector3.up*tile.position.y,transform.position-Vector3.up*transform.position.y) < 1){
                  		max = Floor.transforms[Mathf.FloorToInt(transform.position.x) + i, Mathf.FloorToInt(transform.position.z) + j].position.y;
					}
                }
            }
            h = max;
        } else {
            h = -1;
        }
        if (Input.GetAxis("Player" + (int)index + "X") != 0 || Input.GetAxis("Player" + (int)index + "Y") != 0) {
            Vector3 input = Input.GetAxis("Player" + (int)index + "X") * Cam.right + Input.GetAxis("Player" + (int)index + "Y") * Cam.up;
            input.y = 0;
            input.Normalize();
            if (attacking) {
                walkSpeed *= 1.2f;
                turnSpeed *= 0.15f;
            } else if (defending) {
                walkSpeed *= 0.3f;
                turnSpeed *= 0.3f;
            }
			
			if(input.magnitude>0)
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
            velocity.y = 9;
            jumping = true;
        }
        attackPause -= Time.deltaTime;
        if (Input.GetButtonDown("Player" + (int)index + "Attack") && attackTimer <= 0 && attackPause <= 0 && defendTimer <= 0) {
            attackTimer = 0.4f;
            attackPause = 0.6f;
            attacking = true;
            SoundEffect s = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
            s.init("Sounds/swing_"+(Mathf.CeilToInt(Random.value*3)));
			
        }
        if (attacking || attackTimer > 0) {
            attackTimer -= Time.deltaTime;
            sword.localEulerAngles = 
				Vector3.Lerp(sword.localEulerAngles,swordRotOri + new Vector3(89, Mathf.Sin((0.4f-attackTimer*0.5f) * 20) * 90, 0),1f);
            //sword.localEulerAngles = swordRotOri + new Vector3(Mathf.Sin((0.4f-attackTimer) * 10) * 90,-90+ Mathf.Sin((0.4f-attackTimer) * 10) * 90, 0);
            sword.localScale = Vector3.Lerp(sword.localScale,swordScaOri * 1.1f,0.4f);
            sword.localPosition = Vector3.Lerp(sword.localPosition,swordPosOri + Vector3.forward * 0.2f,0.4f);
            if (attackTimer <= 0) {
                attackTimer = 0;
                attacking = false;
            }
        } else {
            sword.localEulerAngles = Vector3.Lerp(sword.localEulerAngles,swordRotOri,0.4f);
            sword.localScale = Vector3.Lerp(sword.localScale,swordScaOri,0.4f);
            sword.localPosition = Vector3.Lerp(sword.localPosition,swordPosOri,0.4f);
        }
        if (Input.GetButton("Player" + (int)index + "Defend") && !attacking && attackPause <= 0) {
            defendTimer = 0.25f;
            defending = true;
        } else {
            defending = false;
        }
        if (defending || defendTimer > 0 && attackPause <= 0) {
            defending = true;
            defendTimer -= Time.deltaTime;
            shield.localEulerAngles = Vector3.Lerp(shield.localEulerAngles,shieldRotOri + new Vector3(0, 80, 0),0.4f);
            shield.localScale = Vector3.Lerp(shield.localScale,shieldScaOri * 1.8f,0.4f);
            shield.localPosition = Vector3.Lerp(shield.localPosition,new Vector3(0, 0, 0.5f),0.4f);
            sword.localPosition = Vector3.Lerp(sword.localPosition,swordPosOri + 
				Vector3.right * 0.3f + Vector3.down * 0.2f + Vector3.back * 0.1f,0.4f);
            if (defendTimer <= 0) {
                defendTimer = 0;
                defending = false;
            }
        } else {
            shield.localEulerAngles = Vector3.Lerp(shield.localEulerAngles,shieldRotOri,0.4f);
            shield.localScale = Vector3.Lerp(shield.localScale,shieldScaOri,0.4f);
            shield.localPosition = Vector3.Lerp(shield.localPosition,shieldPosOri,0.4f);
        }

        if (transform.position.y < -5)
            Destroy(gameObject);
	}
}
