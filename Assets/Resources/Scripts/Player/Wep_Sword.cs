using UnityEngine;
using System.Collections;

namespace Character {
	public class Wep_Sword : Weapon {
		private float timer = 0;
		private float pause = 0;
		
		private Vector3 positionOrigin;
		private Vector3 rotationOrigin;
		private Vector3 scaleOrigin;

        override internal float setpause {
            get { return pause; }
            set { pause = value; }
        }
		
		private void Start(){
			positionOrigin = transform.parent.localPosition;
			rotationOrigin = transform.parent.localEulerAngles;
			scaleOrigin = transform.parent.localScale;
		}
	
		internal override void Fire (bool state) {
	    	if(state && timer <= 0 && pause <= 0 && !alive.ContainsKey(player)){
	            timer = 0.4f;
                pause = 0.6f;
				alive.Add(player,this);
	            SoundEffect sound = (Instantiate(Resources.Load("SoundEff")) as GameObject).GetComponent<SoundEffect>();
	            sound.init("Sounds/swing_"+(Mathf.CeilToInt(Random.value*3)));
			}
			
			if(alive.ContainsValue(this)){
				Transform sword = transform.parent;
				if(pause > 0){
					pause -= Time.deltaTime;
				}
				if(timer > 0){
					timer -= Time.deltaTime;
		            sword.localEulerAngles = 
						Vector3.Lerp(sword.localEulerAngles,rotationOrigin + new Vector3(90, Mathf.Sin((0.4f-timer*0.5f) * 20) * 90, 0),1f);
		            sword.localScale = Vector3.Lerp(sword.localScale,scaleOrigin * 1.1f,0.4f);
		            sword.localPosition = Vector3.Lerp(sword.localPosition,positionOrigin + Vector3.forward * 0.2f,0.4f);
				} else {
		            sword.localEulerAngles = Vector3.Lerp(sword.localEulerAngles,rotationOrigin,0.4f);
		            sword.localScale = Vector3.Lerp(sword.localScale,scaleOrigin,0.4f);
		            sword.localPosition = Vector3.Lerp(sword.localPosition,positionOrigin,0.4f);
					if(pause <= 0 && !state){
                        alive.Remove(player);
					}
				}
			}
		}
	}
}
