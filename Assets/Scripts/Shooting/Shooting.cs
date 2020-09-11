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

    void Start()
    {
        
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
        GameObject proj = Instantiate(projectile, gunTip.position, gunTip.rotation);
        //proj.GetComponent<ProjectileScript>.direction = player.playerRigidBody.transform;
    }
}
