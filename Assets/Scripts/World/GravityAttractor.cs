using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = 10f;
    public Vector3 gravitationalCenter;
    public float attractorMass;
    public bool isOrbit;
    public bool isComplex;

    void Start(){
        attractorMass = this.GetComponentInParent<Rigidbody>().mass;
        gravitationalCenter = transform.parent.transform.position;
    }

    void FixedUpdate(){
        if(isOrbit) gravitationalCenter = transform.parent.transform.position;
    }

    void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            GameObject parent = col.gameObject.transform.parent.gameObject;
            if(parent.GetComponent<Player.PlayerController>()){
                 parent.GetComponent<Player.PlayerController>().ChangeAttractor(this);
            }
        }
        if(col.tag == "GravityBody"){
            Debug.Log("Trovato gravityBody");
            col.gameObject.GetComponent<GravityBody>().currentAttractor = this;
            
        }
    }

    void OnTriggerExit(Collider col){
        if(col.tag == "Player"){
            GameObject parent = col.gameObject.transform.parent.gameObject;
            if(parent.GetComponent<Player.PlayerController>()){
                 parent.GetComponent<Player.PlayerController>().ChangeAttractor(null);
            }
        }
    }

}
