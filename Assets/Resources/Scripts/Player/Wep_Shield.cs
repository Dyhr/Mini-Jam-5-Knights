using UnityEngine;
using System.Collections;

namespace Character {
	public class Wep_Shield : Weapon {
		private float timer = 0;
		private float pause = 0;
		
		private Vector3 positionOrigin;
		private Vector3 rotationOrigin;
        private Vector3 scaleOrigin;
        private int player;

        override internal float setpause {
            get { return pause; }
            set { pause = value; }
        }
		
		private void Start(){
			positionOrigin = transform.parent.localPosition;
			rotationOrigin = transform.parent.localEulerAngles;
            scaleOrigin = transform.parent.localScale;
            player = transform.parent.parent.GetComponent<Player>().index;
		}
	
		internal override void Fire (bool state) {
			if(state && timer <= 0 && pause <= 0 && !alive.ContainsKey(player)){
	            timer = 0.25f;
	            pause = 0.3f;
				alive.Add(player,this);
			}
			
			Transform shield = transform.parent;
			if(alive.ContainsValue(this)){
				if(pause > 0){
					pause -= Time.deltaTime;
				}
		        shield.localEulerAngles = Vector3.Lerp(shield.localEulerAngles,rotationOrigin + new Vector3(0, 80, 0),0.4f);
		        shield.localScale = Vector3.Lerp(shield.localScale,scaleOrigin * 1.5f,0.4f);
		        shield.localPosition = Vector3.Lerp(shield.localPosition,new Vector3(0.3f, 0, 0.7f),0.4f);
				if(timer > 0){
					timer -= Time.deltaTime;
				} else {
					if(pause <= 0 && !state){
						alive.Remove(player);
					}
				}
			} else {
		        shield.localEulerAngles = Vector3.Lerp(shield.localEulerAngles,rotationOrigin,0.4f);
		        shield.localScale = Vector3.Lerp(shield.localScale,scaleOrigin,0.4f);
		        shield.localPosition = Vector3.Lerp(shield.localPosition,positionOrigin,0.4f);	
			}
		}
	}
}
