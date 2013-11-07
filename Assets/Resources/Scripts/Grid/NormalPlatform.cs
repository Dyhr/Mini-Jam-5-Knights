using UnityEngine;
using System.Collections;
using Grid;

[AddComponentMenu("Game/Platform/Normal")]
public class NormalPlatform : Controller {
		
	// Properties:
	public float radius = 3;
	
	// Init:
	private void Start () {
		radius = Mathf.Abs(transform.lossyScale.y);
	}
	
	internal override float GetInfluence (Vector3 pos){
		Vector3 flat = pos-transform.position;
		return (flat.magnitude <= radius)?1:0;
	}
	
	// Gizmo functions:
	private void OnDrawGizmos() {
		if(transform.localScale != Vector3.one*transform.lossyScale.y){
			transform.localScale = Vector3.one*transform.lossyScale.y;
			radius = Mathf.Abs(transform.lossyScale.y);
		}
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Grid")){
			if(g.transform.position.y != transform.position.y){
				transform.position = new Vector3(transform.position.x,g.transform.position.y,transform.position.z);
			}
		}
		
		Gizmos.color = Color.white * 0.3f + new Color(0,0,0,1);
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
