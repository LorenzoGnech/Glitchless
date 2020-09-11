using System;
using System.Diagnostics;
using Player;
using ProInput.Scripts;
using Rope;
using Unity.Collections;
using UnityEngine;

namespace Carriable.Carriables {
    public class GrapplingHook : MonoBehaviour {
        [Header("Grappling")]
        public GrapplingRope grapplingRope;
        public PlayerController player;
        public Transform grappleTip;
        public Transform grappleHolder;
        public int whatToGrapple;
        public float maxDistance;
        public float minDistance;
        public float rotationSmooth;

        [Header("Raycasts")]
        public float raycastRadius;
        public int raycastCount;

        [Header("Physics")]
        public float pullForce;
        public float pushForce;
        public float yMultiplier;
        public float multiplier;
        public float minPhysicsDistance;
        public float maxPhysicsDistance;
        private bool isMoving;
        private Transform hitPoint;
        private Rigidbody hitBody;
        
        private Vector3 _hit;


        private void Update(){
            if(isMoving && grapplingRope.Grappling){
                 if (ProMouse.RightButton.IsPressed && grapplingRope.Grappling){
                    //grapplingRope.Grapple(grappleTip.position, hitPoint.position);
                    grapplingRope.UpdateEnd(hitPoint.position);
                    _hit = hitPoint.position;
                 }
            }
        }

        private void FixedUpdate() {
            if (ProMouse.RightButton.IsPressed && grapplingRope.Grappling) {
                grappleHolder.rotation = Quaternion.Lerp(grappleHolder.rotation, Quaternion.LookRotation(-(grappleHolder.position - _hit)), rotationSmooth * Time.fixedDeltaTime);
                var distance = Vector3.Distance(player.transform.position, _hit);
                if (!(distance >= minPhysicsDistance) || !(distance <= maxPhysicsDistance)) return;
                player.playerRigidBody.velocity += pullForce * Time.fixedDeltaTime * yMultiplier * Mathf.Abs(_hit.y - player.transform.position.y) * (_hit - player.transform.position).normalized;
                player.playerRigidBody.velocity += pushForce * Time.fixedDeltaTime * player.transform.forward;
                //if(isMoving) player.playerRigidBody.velocity += hitBody.velocity * Time.fixedDeltaTime;
                if(distance > maxDistance) {
                    var heading = _hit - player.transform.position;
                    var dist = heading.magnitude;
                    var direction = heading / distance;
                    Vector3 newVelocity = direction * multiplier;
                    UnityEngine.Debug.Log("Qui entro " + newVelocity + " - " + player.playerRigidBody.velocity);
                    player.playerRigidBody.velocity += newVelocity;
                    grapplingRope.UnGrapple();
                    grapplingRope.UpdateGrapple();
                }
            }
            else {
                grappleHolder.localRotation = Quaternion.Lerp(grappleHolder.localRotation, Quaternion.Euler(0, 0, 0), rotationSmooth * Time.fixedDeltaTime);
            }
        }

        private void LateUpdate() {
            if (ProMouse.RightButton.IsDown && RaycastAll(out var hitInfo)) {
                grapplingRope.Grapple(grappleTip.position, hitInfo.point); 
                _hit = hitInfo.point;
            }

            if (ProMouse.RightButton.IsUp) {
                grapplingRope.UnGrapple();
            }

            if (ProMouse.RightButton.IsPressed && grapplingRope.Grappling) {
                grapplingRope.UpdateStart(grappleTip.position);
            }

            grapplingRope.UpdateGrapple();
        }

        private bool RaycastAll(out RaycastHit hit) {
            isMoving = false;
            var divided = raycastRadius / 2f;
            var possible = new NativeList<RaycastHit>(raycastCount * raycastCount, Allocator.Temp);
            var cam = player.playerCamera.transform;
            for (var x = 0; x < raycastCount; x++) {
                for (var y = 0; y < raycastCount; y++) {
                    var pos = new Vector2(
                        Mathf.Lerp(-divided, divided, x / (float)(raycastCount - 1)),
                        Mathf.Lerp(-divided, divided, y / (float)(raycastCount - 1))
                    );
                    int layerMask = 1 << 9;
                    //UnityEngine.Debug.Log("Tot = " + (cam.position + cam.right * pos.x + cam.up * pos.y) + " - cam.position = " + cam.position + " - cam.right = " + cam.right + " - cam.up = " + cam.up + " - pos.x = " + pos.x + " - pos.y = " + pos.y);
                    if (!Physics.Raycast(cam.position + cam.right * pos.x + cam.up * pos.y, cam.forward, out var hitInfo, maxDistance, layerMask)) continue;
                    
                    var distance = Vector3.Distance(cam.position, hitInfo.point);
                    if (hitInfo.transform.gameObject.layer != whatToGrapple) continue; 
                    if (distance < minDistance) continue;
                    if (distance > maxDistance) continue;
                    possible.Add(hitInfo);
                }
            }

            var arr = possible.ToArray();
            possible.Dispose();
            if (arr.Length > 0) {
                var closest = new RaycastHit();
                var distance = 0f;
                var set = false;

                foreach (var hitInfo in arr) {
                    var hitDistance = DistanceFromCenter(hitInfo.point);

                    if (!set) {
                        set = true;
                        distance = hitDistance;
                        closest = hitInfo;
                    }
                    else if (hitDistance < distance) {
                        distance = hitDistance;
                        closest = hitInfo;
                    }
                }
                
                hit = closest;
                if(hit.transform.tag == "Meteor") {
                    isMoving = true;
                    hitBody = hit.rigidbody;
                    hitPoint = hit.transform.GetChild(0);
                    hitPoint.position = hit.point;
                }
                return true;
            }
            
            hit = new RaycastHit();
            return false;
        }
        
        private float DistanceFromCenter(Vector3 point) {
            return Vector2.Distance(player.playerCamera.WorldToViewportPoint(point),
                new Vector2(0.5f, 0.5f));
        }
    }
}