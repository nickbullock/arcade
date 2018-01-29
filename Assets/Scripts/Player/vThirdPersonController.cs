using System;
using UnityEngine;
using System.Collections;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        protected virtual void Start()
        {
#if !UNITY_EDITOR
                Cursor.visible = false;
#endif
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
                
                pickUpItem.transform.localPosition = new Vector3(-0.2f, 1f, 1f);
                pickUpItem.transform.rotation = gameObject.transform.rotation;
                
                Rigidbody itemRigidbody = pickUpItem.GetComponent<Rigidbody>();
                BoxCollider itemCollider = pickUpItem.GetComponent<BoxCollider>();

                itemRigidbody.useGravity = false;
                itemRigidbody.isKinematic = true;
                itemCollider.enabled = false;
            }
        }

        public virtual void Drop()
        {
            if (pickUpItem)
            {
                weaponIKIsActive = false;
                
                Rigidbody itemRigidbody = pickUpItem.GetComponent<Rigidbody>();
                BoxCollider itemCollider = pickUpItem.GetComponent<BoxCollider>();
                
                itemRigidbody.useGravity = true;
                itemRigidbody.isKinematic = false;
                itemCollider.enabled = true;

                pickUpItem.transform.parent = null;
                pickUpItem = null;
            }
        }

        public virtual void Fire()
        {
            if (pickUpItem && pickUpItem.CompareTag("Weapon"))
            {
                pickUpItem.GetComponent<BulletGenerator>().Generate();
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
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
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
#else

            Vector3 turnDir =
new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
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