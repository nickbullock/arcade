using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
//        protected virtual void OnGUI()
//        {
//            if (isDead)
//            {    
//                GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
//                centeredStyle.alignment = TextAnchor.UpperCenter;
//                GUI.Label(new Rect (Screen.width/2-50, Screen.height/2-25, 100, 50), gameObject.name + " lose!", centeredStyle);
//            }
//        }

        public virtual IEnumerator RestartScene()
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("level 01");
        }

        public virtual void Sprint(bool value)
        {
            isSprinting = value;
        }

        public virtual void Strafe()
        {
            if (locomotionType == LocomotionType.OnlyFree) return;
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // conditions to do this action
            bool jumpConditions = isGrounded && !isJumping;
            // return if jumpCondigions is false
            if (!jumpConditions) return;
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;
            // trigger jump animations            
            if (_rigidbody.velocity.magnitude < 1)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", 0.2f);
        }

        public virtual void PickUp()
        {
            if (itemAround)
            {
                pickUpItem = itemAround;
                weaponIKIsActive = true;
                
                pickUpItem.transform.parent = gameObject.transform;

                WeaponMeta meta = GetComponentInChildren<WeaponMeta>();

                if (meta != null)
                {
                    pickUpItem.transform.localPosition = meta.weaponOffset;
                }
                
                pickUpItem.transform.rotation = gameObject.transform.rotation;
                
                Rigidbody itemRigidbody = pickUpItem.GetComponent<Rigidbody>();
                BoxCollider itemCollider = pickUpItem.GetComponent<BoxCollider>();

                if (itemRigidbody && itemCollider)
                {
                    itemRigidbody.useGravity = false;
                    itemRigidbody.isKinematic = true;
                    itemCollider.enabled = false;
                }
            }
        }

        public virtual void Drop()
        {
            if (pickUpItem)
            {
                weaponIKIsActive = false;
                
                Rigidbody itemRigidbody = pickUpItem.GetComponent<Rigidbody>();
                BoxCollider itemCollider = pickUpItem.GetComponent<BoxCollider>();


                if (itemRigidbody && itemCollider)
                {
                    itemRigidbody.useGravity = true;
                    itemRigidbody.isKinematic = false;
                    itemCollider.enabled = true;

                    pickUpItem.transform.parent = null;
                    pickUpItem = null;
                }
            }
        }

        public virtual void Fire()
        {
            if (pickUpItem && pickUpItem.CompareTag("Weapon"))
            {
                var generator = pickUpItem.GetComponent<BulletGenerator>();
                    
                if(generator) generator.Generate();
            }
        }

        public virtual void Damage(float hp, Vector3 hitPoint)
        {
            health = health - hp;

            if (health <= 0)
            {
               _rigidbody.AddForce(hitPoint * 100);
                Die();
            }
        }

        public virtual void Die()
        {
            if (!isDead)
            {
                isDead = true;
            
                DisableRagdoll(false);
                GetComponent<Animator>().enabled = false;
                Destroy(gameObject, 5);
            }
        }

        public virtual void Turn()
        {   
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(camRay.origin, camRay.direction * 10, Color.green);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if (Physics.Raycast(camRay, out floorHit, 100f, groundLayer))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Set the player's rotation to this new rotation.
                _rigidbody.rotation = Quaternion.LookRotation(playerToMouse);
            }
        }

        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y,
                transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation),
                strafeRotationSpeed * Time.fixedDeltaTime);
            targetRotation = transform.rotation;
        }
    }
}