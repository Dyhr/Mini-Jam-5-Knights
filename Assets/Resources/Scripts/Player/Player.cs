using UnityEngine;
using System.Collections;

namespace Character {
	public enum Index {
	    One = 1,
	    Two = 2,
	    Three = 3,
	    Four = 4
	}
	
	[AddComponentMenu("Game/Player")]
	public class Player : MonoBehaviour {
		private const float GRAVITY = 1f;
		
		private Control controller;
		internal Vector3 _velocity;
		private bool _alive;
		private bool _grounded;
		private float _speed;
		private float _fallspeed;
		private Hashtable weapons;
		
	    public int index;
		public Vector3 velocity { get{ return _velocity; } }
		public bool alive { get{ return _alive; } }
        public bool grounded { get { return _grounded; } }
        public bool attacking { get { return Weapon.alive.ContainsValue(weapons["RightHand"]); } }
        public bool defending { get { return Weapon.alive.ContainsValue(weapons["LeftHand"]); } }
		
		private void Start () {
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
                default:
                    renderer.material.color = (Color.white) / 2;
                    break;
	        }
            gameObject.tag = "Player";
			controller = new Control((int)index);
			_alive = true;
			_velocity = Vector3.zero;
			_speed = 4f;
			weapons = new Hashtable();
			foreach(WeaponSlot slot in GetComponentsInChildren<WeaponSlot>()){
				weapons.Add(slot.gameObject.name,slot.GetComponentInChildren<Weapon>());
				slot.Init(this);
			}
            rigidbody.freezeRotation = true;
		}
		
		private void Update () {
			if(!_alive)return;
			
			// Movement:
			Vector3 forward = Camera.main.transform.forward; 
			forward.y = 0;
			forward.Normalize();
			Vector3 input = Vector3.zero;
			if(index > 0){
				input = controller.leftStick.y * forward + controller.leftStick.x * Camera.main.transform.right;
			}
			_fallspeed += GRAVITY;
			if(index > 0){
				if(controller.a && _grounded){
					_fallspeed -= 20;
				}
			}
			
			if(index > 0){
				if(controller.leftStick.magnitude > 0){
					transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(
						controller.leftStick.x * Camera.main.transform.right-controller.leftStick.y * forward),920*Time.deltaTime);
				}
			}
            transform.position = transform.position + (transform.forward * _speed * input.magnitude + _velocity) * Time.deltaTime;
			transform.Translate(Vector3.down*_fallspeed*Time.deltaTime);
            _velocity *= 0.9f;
			
			// Collision:
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Grid")){
				_grounded = g.GetComponent<Grid.Grid>().CollidePlayer(this);
				if(_grounded){
					break;
				}
			}
			if(_grounded && _fallspeed > 0){
				_fallspeed = 0;
			}
			
			// Weapons:
			if(weapons.ContainsKey("RightHand")){
				((Weapon)weapons["RightHand"]).Fire(controller.x);
			}
			if(weapons.ContainsKey("LeftHand")){
				((Weapon)weapons["LeftHand"]).Fire(controller.b);
			}
		}
        private void OnTriggerEnter(Collider other) {
            if (!_alive || index <= 0) {
                return;
            }
            if (other.gameObject.tag == "Magma") {
                _alive = false;
                Destroy(gameObject, 0.2f);
            } else if (other.gameObject.tag == "sword") {
                Player player = other.transform.parent.parent.GetComponent<Player>();
                if (player != this) {
                    if (player.attacking) {
                        Instantiate(Resources.Load("HitParticle"), (transform.position + other.transform.parent.position) / 2, Quaternion.identity);
                        if (!(defending && Vector3.Angle(transform.forward, transform.position - other.transform.parent.position) < 180)) {
                            SoundEffect s = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
                            s.init("Sounds/hit_" + Mathf.CeilToInt(Random.value * 3));
                            Stun((transform.position - other.transform.parent.position).normalized * 8, 0);
                        } else {
                            SoundEffect s = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
                            s.init("Sounds/defend_" + Mathf.CeilToInt(Random.value * 3));
                            Stun((transform.position - other.transform.parent.position).normalized * 2,0);
                            player.Stun(-(transform.position - other.transform.parent.position).normalized * 6,1.5f);
                        }
                    }
                }
            }
        }
        public void Stun(Vector3 force, float pause) {
            ((Weapon)weapons["LeftHand"]).setpause += pause;
            ((Weapon)weapons["RightHand"]).setpause += pause;
            _velocity += force;
        }
	    private void OnDestroy() {
	        _alive = false;
		}
	}
	
	public class Control {
		private int _index;
		
		public Vector2 leftStick { get { 
				return new Vector2(
					Input.GetAxis("joystick "+_index+" axis 0"),
					Input.GetAxis("joystick "+_index+" axis 1")); 
			} }
		public Vector2 rightStick { get { 
				return Vector2.zero; 
			} }
		public Vector2 dpad { get { 
				return Vector2.zero; 
			} }
		public float ltrigger { get { 
				return 0.0f; 
			} }
		public float rtrigger { get { 
				return 0.0f; 
			} }
		public bool a { get { return Input.GetKey("joystick "+_index+" button 0"); } }
		public bool b { get { return Input.GetKey("joystick "+_index+" button 1"); } }
		public bool x { get { return Input.GetKey("joystick "+_index+" button 2"); } }
		public bool y { get { return Input.GetKey("joystick "+_index+" button 3"); } }
		public bool lBumper { get { return Input.GetKey("joystick "+_index+" button 4"); } }
		public bool rBumper { get { return Input.GetKey("joystick "+_index+" button 5"); } }
		public bool back { get { return Input.GetKey("joystick "+_index+" button 6"); } }
		public bool start { get { return Input.GetKey("joystick "+_index+" button 7"); } }
		
		public Control (int joystickIndex) {
			this._index = joystickIndex;
		}
	}
}
