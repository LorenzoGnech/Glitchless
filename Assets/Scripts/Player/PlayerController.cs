﻿using System;
using ProInput.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Player {
    public class PlayerController : MonoBehaviour {
        [Header("Movement")]
        public float movementSpeed;
        public float airMovementSpeed;
        public float maxSpeed;
        public float runningMultiplier;

        [Header("Friction")]
        public float friction;
        public float airFriction;

        [Header("Rotation")]
        public float rotationSensitivity;
        public float rotationBounds;

        [Header("Gravity")]
        public float extraGravity;
        public GravityAttractor currentAttractor;
        public GravityAttractor newAttractor;
        public float smoothRotation;

        [Header("Ground Detection")]
        public LayerMask whatIsGround;
        public float checkYOffset;
        public float checkRadius;
        public float groundTimer;

        [Header("Jumping")]
        public float jumpForce;
        public float jumpCooldown;

        [Header("Data")]
        public Camera playerCamera;
        public Transform playerCameraHolder;
        public Rigidbody playerRigidBody;
        public GameObject gfx;
        public Transform gravityRaycastStartingPoint;

        [Header("Surface")]
        public float surfaceAttractionForce;
        public float maxSurfaceDistance;

        private InputObject _forwardsInput;
        private InputObject _backwardsInput;
        private InputObject _leftInput;
        private InputObject _rightInput;
        private InputObject _jumpInput;
        private KeyCode _run;
        
        private float _xRotation;
        private float _yRotation;
        private float _grounded;
        private bool _realGrounded;
        private float _jumpCooldown;
        private float rotateY;
        private bool changingAttractor = false;
        private bool isRunning = false;
        private bool isOnSurface = false;
        

        private void Start() {
            _forwardsInput = new InputObject("Forwards", Key.W);
            _backwardsInput = new InputObject("Backwards", Key.S);
            _leftInput = new InputObject("Left", Key.A);
            _rightInput = new InputObject("Right", Key.D);
            _jumpInput = new InputObject("Space", Key.Space);
            _run = KeyCode.LeftShift;
        }

        private void FixedUpdate() {
            GroundCheck();
            ApplyMovement();
            ApplyFriction();
            if(!changingAttractor && currentAttractor != null && !currentAttractor.isComplex){
                ApplyGravity();
            } else if(currentAttractor != null && currentAttractor.isComplex){
                ApplyComplexGravity();
            }
            Jumping();
        }

        private void Update() {
            Rotation();
            if(Input.GetKeyDown(_run)){
                isRunning = true;
            }
            if(Input.GetKeyUp(_run)){
                isRunning = false;
            }
        }

        private void GroundCheck() {
            _grounded -= Time.fixedDeltaTime;
            var colliderList = new Collider[100];
            int block = 1 << LayerMask.NameToLayer("Block");
            int surface = 1 << LayerMask.NameToLayer("Surface");
            int mask = block | surface;
            var size = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, checkYOffset, 0), checkRadius, colliderList, mask);
            _realGrounded = size > 0;
            if (_realGrounded){
                _grounded = groundTimer;
            }
        }

        private void ApplyMovement() {
            var axis = new Vector2(
                (_leftInput.IsPressed ? -1 : 0) + (_rightInput.IsPressed ? 1 : 0),
                (_backwardsInput.IsPressed ? -1 : 0) + (_forwardsInput.IsPressed ? 1 : 0)
            ).normalized;
            var speed = _realGrounded ? movementSpeed : airMovementSpeed;
            if(isRunning) speed *= runningMultiplier;
            var vertical = axis.y * speed * Time.fixedDeltaTime * playerCameraHolder.transform.forward;
            if(!_realGrounded) vertical = axis.y * speed * Time.fixedDeltaTime * playerCameraHolder.transform.forward;
            var horizontal = axis.x * speed * Time.fixedDeltaTime * playerCameraHolder.transform.right;
            if(!_realGrounded) vertical = axis.y * speed * Time.fixedDeltaTime * playerCameraHolder.transform.forward;
            var magnitude = vertical.magnitude;
            var hmagnitude = horizontal.magnitude;
            if(_realGrounded){
                vertical = Vector3.Project(vertical,gfx.transform.forward);
                vertical = vertical.normalized*magnitude;
                horizontal = Vector3.Project(horizontal,gfx.transform.right);
                horizontal = horizontal.normalized*hmagnitude;
            }
            if (CanApplyForce(vertical, axis)){
                playerRigidBody.velocity += vertical;
            }
            if (CanApplyForce(horizontal, axis))
                playerRigidBody.velocity += horizontal;
        }

        private void ApplyFriction() {
            var vel = playerRigidBody.velocity;
            var target = _realGrounded ? friction : airFriction;
            vel.x = Mathf.Lerp(vel.x, 0f, target * Time.fixedDeltaTime);
            vel.z = Mathf.Lerp(vel.z, 0f, target * Time.fixedDeltaTime);
            vel.y = Mathf.Lerp(vel.y, 0f, target * Time.fixedDeltaTime);
            playerRigidBody.velocity = vel;
        }
        
        private void Rotation() {
            Cursor.lockState = CursorLockMode.Locked;
            var mouseDelta = Mouse.current.delta.ReadValue();
            _xRotation -= mouseDelta.y * rotationSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, -rotationBounds, rotationBounds);
            _yRotation += mouseDelta.x * rotationSensitivity;
            //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + _yRotation, transform.rotation.z);
            Vector3 rot;
            rot.x = transform.rotation.x;
            rot.y = transform.rotation.y;
            rot.z = transform.rotation.z;
            rotateY = _yRotation;
            playerCameraHolder.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            gfx.transform.localRotation = Quaternion.Euler(0, _yRotation, 0);
        }

        private void ApplyGravity() {
            Vector3 grav = currentAttractor.gravitationalCenter;
            float distance = Vector3.Distance(currentAttractor.transform.position, transform.position);
            Vector3 surfaceNorm = Vector3.zero;
            Vector3 realSurfaceNorm = Vector3.zero;
            RaycastHit hit;
            RaycastHit surfaceHit;
            int layerMask = 1 << 8;
            int surfaceLayerMask = LayerMask.GetMask("Surface");
            if(Physics.Raycast(gravityRaycastStartingPoint.position, grav - transform.position, out hit, distance, layerMask)){
                surfaceNorm = hit.normal;
            }
            if(Physics.Raycast(gravityRaycastStartingPoint.position, -transform.up, out surfaceHit, distance, surfaceLayerMask)){
                realSurfaceNorm = surfaceHit.normal;
                isOnSurface = true;
            } else{
                isOnSurface = false;
            }
            if(isOnSurface && surfaceHit.distance < maxSurfaceDistance){
                Vector3 backup = transform.localRotation.eulerAngles;
                var myRotation = transform.rotation;
                var targetRotation = Quaternion.FromToRotation(transform.up, realSurfaceNorm)*myRotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*smoothRotation);
                Vector3 n = transform.localRotation.eulerAngles;
                float pullForce = 0.0f;
                pullForce = surfaceAttractionForce;
                Vector3 pullVec = gravityRaycastStartingPoint.position - surfaceHit.point;
                var vel = playerRigidBody.velocity;
                vel.y -= Mathf.Abs(vel.y) * Time.fixedDeltaTime * extraGravity;
                vel.x -= (pullForce*pullVec.normalized).x;
                vel.y -= (pullForce*pullVec.normalized).y;
                vel.z -= (pullForce*pullVec.normalized).z;
                playerRigidBody.velocity = vel;
            }else{
                Vector3 backup = transform.localRotation.eulerAngles;
                var myRotation = transform.rotation;
                var targetRotation = Quaternion.FromToRotation(transform.up, surfaceNorm)*myRotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*smoothRotation);
                Vector3 n = transform.localRotation.eulerAngles;
                float pullForce = 0.0f;
                float divider = Mathf.Pow(Vector3.Distance(grav, gravityRaycastStartingPoint.position), 2);
                //pullForce = currentAttractor.gravity * ((currentAttractor.attractorMass*playerRigidBody.mass)  / divider); // FISICA CORRETTA
                pullForce = currentAttractor.gravity / 11;
                Vector3 pullVec = gravityRaycastStartingPoint.position - grav;
                var vel = playerRigidBody.velocity;
                vel.y -= Mathf.Abs(vel.y) * Time.fixedDeltaTime * extraGravity;
                vel.x -= (pullForce*pullVec.normalized).x;
                vel.y -= (pullForce*pullVec.normalized).y;
                vel.z -= (pullForce*pullVec.normalized).z;
                playerRigidBody.velocity = vel;
            }
        }

        private void ApplyComplexGravity() {
            //TODO
            /*
            Vector3 grav = currentAttractor.gravitationalCenter;
            float distance = Vector3.Distance(currentAttractor.transform.position, transform.position);
            Vector3 surfaceNorm = Vector3.zero;
            RaycastHit hit;
            int layerMask = 1 << 8;
            if(Physics.Raycast(gravityRaycastStartingPoint.position, grav - transform.position, out hit, distance, layerMask)){
                surfaceNorm = hit.normal;
            }
            Vector3 backup = transform.localRotation.eulerAngles;
            var myRotation = transform.rotation;
            var targetRotation = Quaternion.FromToRotation(transform.up, surfaceNorm)*myRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*smoothRotation);
            Vector3 n = transform.localRotation.eulerAngles;
            float pullForce = 0.0f;
            float divider = Mathf.Pow(Vector3.Distance(grav, gravityRaycastStartingPoint.position), 2);
            //pullForce = currentAttractor.gravity * ((currentAttractor.attractorMass*playerRigidBody.mass)  / divider); // FISICA CORRETTA
            pullForce = currentAttractor.gravity / 11;
            Vector3 pullVec = gravityRaycastStartingPoint.position - grav;
            var vel = playerRigidBody.velocity;
            vel.y -= Mathf.Abs(vel.y) * Time.fixedDeltaTime * extraGravity;
            vel.x -= (pullForce*pullVec.normalized).x;
            vel.y -= (pullForce*pullVec.normalized).y;
            vel.z -= (pullForce*pullVec.normalized).z;
            playerRigidBody.velocity = vel;
            */
        }

        public void ChangeAttractor(GravityAttractor attractor){
            newAttractor = attractor;
            changingAttractor = true;
            if(attractor == null) Debug.Log("Sono nello spazio");
            if(attractor != null){
                Debug.Log("Cambio attractor... " + attractor);
                float distance = Vector3.Distance(attractor.transform.position, transform.position);
                Vector3 surfaceNorm = Vector3.zero;
                RaycastHit hit;
                int layerMask = 1 << 8;
                if(Physics.Raycast(transform.position, attractor.gravitationalCenter - transform.position, out hit, distance, layerMask)){
                    surfaceNorm = hit.normal;
                }
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, surfaceNorm) * transform.rotation;
                transform.localRotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime*0.01f);
            }
            //Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime*0.01f);
            currentAttractor = attractor;
            changingAttractor = false;
        }


        private void Jumping() {
            _jumpCooldown -= Time.deltaTime;
            if (!(_grounded >= 0) || !(_jumpCooldown <= 0) || !_jumpInput.IsDown) return;
            Vector3 force = transform.up * 250 * jumpForce;
            playerRigidBody.AddForce(force);
            _jumpCooldown = jumpCooldown;
        }
        
        private bool CanApplyForce(Vector3 target, Vector2 axis) {
            var targetC = Get2DVec1(target).normalized;
            var velocityC = Get2DVec1(playerRigidBody.velocity).normalized;
            var dotProduct = Vector2.Dot(velocityC, targetC);
            return (dotProduct <= 0 || (dotProduct * Get2DVec1(playerRigidBody.velocity).magnitude < maxSpeed * GetAxisForce(axis))
                                    && dotProduct * Get2DVec2(playerRigidBody.velocity).magnitude < maxSpeed * GetAxisForce(axis)
                                    && dotProduct * Get2DVec3(playerRigidBody.velocity).magnitude < maxSpeed * GetAxisForce(axis));
        }

        private static float GetAxisForce(Vector2 axis) {
            return (int)axis.x != 0 ? Mathf.Abs(axis.x) : Mathf.Abs(axis.y);
        }

        private static Vector2 Get2DVec1(Vector3 vec) {
            return new Vector2(vec.x, vec.z);
        }
        private static Vector2 Get2DVec2(Vector3 vec) {
            return new Vector2(vec.x, vec.y);
        }
        private static Vector2 Get2DVec3(Vector3 vec) {
            return new Vector2(vec.y, vec.z);
        }
    }
}
