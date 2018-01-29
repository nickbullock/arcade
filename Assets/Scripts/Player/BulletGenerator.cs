using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace Invector.CharacterController
{
    public class BulletGenerator : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public GameObject hitPlayerPrefab;
        public Transform bulletSpawn;
        public float shotSpread = 0.08f;
        
        public float bulletSpeed;
        public float fireRate;
        public float damage;
        public float range;
        protected float timer;
        
        protected ParticleSystem gunParticles;
        protected Ray shootRay;
        protected RaycastHit shootHit;
        protected int shootMask;

        protected virtual void Start()
        {
            timer = 0;
            gunParticles = GetComponent<ParticleSystem>();
            shootMask = LayerMask.GetMask ("Player");
            shootRay = new Ray();
        }

        protected void Update()
        {
            Debug.DrawRay(bulletSpawn.position, gameObject.transform.forward * 10, Color.red);
        }

        Vector3 SprayDirection()
        {
            float vx = (1 - 2 * UnityEngine.Random.value) * shotSpread;
            float vy = (1 - 2 * UnityEngine.Random.value) * shotSpread;
            float vz = 1.0f;
            
            return transform.TransformDirection(new Vector3(vx, vy, vz));
        }

        public void Generate()
        {
            timer = timer + 100 * Time.deltaTime;

            if (bulletPrefab && bulletSpawn && timer > fireRate)
            {
                shootRay.origin = bulletSpawn.position;
                shootRay.direction = gameObject.transform.forward;
                
                gunParticles.Stop();
                gunParticles.Play();
                
                Quaternion rotation = Quaternion.Euler(
                    90,
                    gameObject.transform.parent.rotation.eulerAngles.y,
                    gameObject.transform.parent.rotation.eulerAngles.z);

                GameObject bullet = (GameObject) Instantiate(
                    bulletPrefab,
                    bulletSpawn.position,
                    rotation);
                
                timer = timer - fireRate;

                bullet.GetComponent<Rigidbody>().velocity = SprayDirection() * bulletSpeed;

                if (Physics.Raycast(shootRay, out shootHit, range, shootMask))
                {
                    vThirdPersonController enemyController = shootHit.collider.GetComponent<vThirdPersonController>();
                    if (enemyController != null)
                    {   
                        Destroy(bullet, 0.1f);
                       
                        enemyController.Damage(damage, shootHit.point);
                        
                        GameObject blood = (GameObject) Instantiate(
                            hitPlayerPrefab,
                            shootHit.point,
                            rotation);
                        
                        Destroy(blood, 0.4f);
                    }
                }
                else
                {
                    Destroy(bullet, 2.0f);
                }
            }
        }
    }
}