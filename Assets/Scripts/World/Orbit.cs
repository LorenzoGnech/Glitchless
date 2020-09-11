using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour{
    public Transform planet;
    public float speed;
    public int rotationSpeed;
    public Vector3 axis;
    private Vector3 rotationAxis;
    public float radius;
    public Vector3 desiredPosition;

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

    void Update(){
        OrbitAround();
    }

    void Start(){
        transform.position = planet.position + (radius*axis);
        Vector3 tempAxis = new Vector3(axis.z, 0f, axis.x);
        rotationAxis = Vector3.Cross(axis, tempAxis);
        //rotationAxis.x = axis.y;
        //rotationAxis.y = axis.z;
        //rotationAxis.z = axis.x;
    }

    void OrbitAround(){
        //transform.RotateAround(planet.position, rotationAxis, speed*Time.deltaTime);
        OrbitAround(planet.position, rotationAxis, speed*Time.deltaTime);
    }

    private void OrbitAround(Vector3 center, Vector3 axis, float angle) {
        Vector3 pos = this.transform.position;
        Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
        Vector3 dir = pos - center; // find current direction relative to center
        dir = rot * dir; // rotate the direction
        this.transform.position = center + dir; // define new position
        // rotate object to keep looking at the center:
        //Quaternion myRot = transform.rotation;
        //transform.rotation *= Quaternion.Inverse(myRot) * rot * myRot;
    }
}