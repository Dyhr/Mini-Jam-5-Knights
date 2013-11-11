using UnityEngine;
using System.Collections;

namespace Character {
	public class WeaponSlot : MonoBehaviour {
		internal void Init(Player player){
			GetComponentInChildren<Weapon>().player = player;
		}
		private void OnDrawGizmos() {
			Gizmos.DrawSphere(transform.position,0.05f);
		}
		private void OnDrawGizmosSelected() {
			Gizmos.DrawSphere(transform.position,0.1f);
		}
	}
	public abstract class Weapon : MonoBehaviour {
		protected static Weapon alive;
		internal Player player;
		internal abstract void Fire(bool state);
	}
}
