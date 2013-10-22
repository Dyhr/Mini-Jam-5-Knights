using UnityEngine;
using System.Collections;
using Grid;

[AddComponentMenu("Game/Platform/Normal")]
public class NormalPlatform : Controller {
		
	// Properties:
	public float radius = 3;
	
	internal override float GetInfluence (Vector3 pos){
		return Mathf.Max(0,radius - Vector3.Distance(pos,transform.position))/radius; // TODO lock to y axis
	}
	
	// Gizmo functions:
	private void OnDrawGizmos() {
		Gizmos.color = Color.black;
		float inc = (Mathf.PI*2)/180;
		float angle = Mathf.PI*2;
		while(angle >= 0){
			Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
					transform.position + new Vector3(Mathf.Cos(angle-inc),0,Mathf.Sin(angle-inc))*radius);
			angle -= inc;
		}
		angle = Mathf.PI*2;
		while(angle >= 0){
			Gizmos.DrawLine(transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
					transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle-inc),0,Mathf.Sin(angle-inc))*radius);
			angle -= inc;
		}
		inc = (Mathf.PI*2)/8;
		angle = Mathf.PI*2;
		while(angle >= 0){
			Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
					transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius);
			angle -= inc;
		}
	}	
}
