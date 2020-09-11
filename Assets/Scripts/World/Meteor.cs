using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour{
    public float speed;
    public Vector3 axis;
    public Rigidbody rb;

    void OnCollisionEnter(Collision other) {
         if (other.transform.tag == "Player") {
            other.transform.parent = transform;
         }
     }
 
     private void OnCollisionExit(Collision other) {
         if (other.transform.tag == "Player") {
             other.transform.parent = null;
         }
     }

    void FixedUpdate(){
        rb.velocity = axis*speed;
    }

    void Start(){
        transform.rotation = Quaternion.Euler(axis);
    }
}