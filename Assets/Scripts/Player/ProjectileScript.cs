﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed;
	[Tooltip("From 0% to 100%")]
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public List<GameObject> trails;
    public float lifetime;
    public Vector3 direction;

    private Vector3 startPos;
	private bool collided;
	private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent <Rigidbody> ();
		if (muzzlePrefab != null) {
			var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
			//muzzleVFX.transform.forward = gameObject.transform.forward + offset;
			var ps = muzzleVFX.GetComponent<ParticleSystem>();
			if (ps != null)
				Destroy (muzzleVFX, ps.main.duration);
			else {
				var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy (muzzleVFX, psChild.main.duration);
			}
		}
        StartCoroutine(DestroyBullet());
    }

	void FixedUpdate () {
        if (speed != 0 && rb != null)
			rb.position += (transform.forward/*+ offset*/) * (speed * Time.deltaTime);   
    }

	void OnCollisionEnter (Collision co) {
        if (co.gameObject.tag != "Bullet" && !collided && co.gameObject.tag != "Player")
            {
                collided = true;

                if (trails.Count > 0)
                {
                    for (int i = 0; i < trails.Count; i++)
                    {
                        trails[i].transform.parent = null;
                        var ps = trails[i].GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Stop();
                            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }
                }

                speed = 0;
                GetComponent<Rigidbody>().isKinematic = true;

                ContactPoint contact = co.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 pos = contact.point;

                if (hitPrefab != null)
                {
                    var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

                    var ps = hitVFX.GetComponent<ParticleSystem>();
                    if (ps == null)
                    {
                        var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                        Destroy(hitVFX, psChild.main.duration);
                    }
                    else
                        Destroy(hitVFX, ps.main.duration);
                }
                StartCoroutine(DestroyParticle(0f));
        }
	}

	public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}

	public IEnumerator DestroyBullet () {
		yield return new WaitForSeconds (lifetime);
		Destroy (gameObject);
	}
}
