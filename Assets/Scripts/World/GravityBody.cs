using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    public GravityAttractor currentAttractor;
    public Rigidbody rb;
    public float smoothRotation = 15;

    void Awake(){
        if(rb == null){
            rb = GetComponent<Rigidbody>();
        }
    }

    void FixedUpdate(){
            ApplyFriction();
            if(currentAttractor != null){
                ApplyGravity();
            }
    }

        private void ApplyFriction() {
            var vel = rb.velocity;
            var friction = 5;
            vel.x = Mathf.Lerp(vel.x, 0f, friction * Time.fixedDeltaTime);
            vel.z = Mathf.Lerp(vel.z, 0f, friction * Time.fixedDeltaTime);
            vel.y = Mathf.Lerp(vel.y, 0f, friction * Time.fixedDeltaTime);
            rb.velocity = vel;
        }

        private void ApplyGravity() {
            Vector3 grav = currentAttractor.gravitationalCenter;
            float distance = Vector3.Distance(currentAttractor.transform.position, transform.position);
            Vector3 surfaceNorm = Vector3.zero;
            RaycastHit hit;
            int layerMask = 1 << 8;
            if(Physics.Raycast(transform.position, grav - transform.position, out hit, distance, layerMask)){
                surfaceNorm = hit.normal;
            }
            Vector3 backup = transform.localRotation.eulerAngles;
            var myRotation = transform.rotation;
            var targetRotation = Quaternion.FromToRotation(transform.up, surfaceNorm)*myRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*smoothRotation);
            Vector3 n = transform.localRotation.eulerAngles;
            float pullForce = 0.0f;
            float divider = Mathf.Pow(Vector3.Distance(grav, transform.position), 2);
            pullForce = currentAttractor.gravity / 11;
            Vector3 pullVec = transform.position - grav;
            var vel = rb.velocity;
            vel.x -= (pullForce*pullVec.normalized).x;
            vel.y -= (pullForce*pullVec.normalized).y;
            vel.z -= (pullForce*pullVec.normalized).z;
            rb.velocity = vel;
        }
}
