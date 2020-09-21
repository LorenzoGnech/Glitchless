using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using ProInput.Scripts;

public class Shooting : MonoBehaviour
{
    public PlayerController player;
    public Transform gunTip;
    public GameObject projectile;
    public float cooldownValue;
    private float cooldown = 1f;
    private GameObject pro;
    private Rigidbody playerRb;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ProMouse.LeftButton.IsDown) {
            if(cooldown < 0) {
                Shoot();
                cooldown = cooldownValue;
            }
        }

        if (ProMouse.LeftButton.IsPressed && cooldown < 0) {
            Shoot();
            cooldown = cooldownValue;
        }
    }

    void FixedUpdate(){
        cooldown -= Time.deltaTime;
    }
    
    void Shoot(){
        var proj = Instantiate(projectile, gunTip.position, gunTip.rotation).GetComponent<Rigidbody>();
        var bullet_speed = transform.TransformDirection(new Vector3 (0, 0, 100));
        proj.velocity = bullet_speed /*+ playerRb.velocity/2*/;
    }
}
