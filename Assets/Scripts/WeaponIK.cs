using UnityEngine;
using System;
using System.Collections;

namespace Invector.CharacterController
{
    public class WeaponIK : MonoBehaviour
    {
        protected Animator animator;

        public Transform leftHand;
        public Transform rightHand;

        protected vThirdPersonController cc; // access the ThirdPersonController component   

        void Start()
        {
            animator = GetComponent<Animator>();
            cc = GetComponent<vThirdPersonController>();
        }

        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (animator)
            {
                //if the IK is active, set the position and rotation directly to the goal. 
                if (cc.weaponIKIsActive)
                {
                    // Set the right hand target position and rotation, if one has been assigned
                    if (rightHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
                    }

                    if (leftHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
                    }
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }
    }
}