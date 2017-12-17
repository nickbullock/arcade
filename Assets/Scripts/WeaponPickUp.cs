using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.CharacterController
{
	public class WeaponPickUp : MonoBehaviour {
		protected vThirdPersonController cc;                // access the ThirdPersonController component   

		// Use this for initialization√
		void Start ()
		{
			cc = gameObject.transform.parent.GetComponent<vThirdPersonController>();
		}

		void OnTriggerEnter(Collider _collider)
		{
			if (_collider.gameObject.tag == "Weapon")
			{
				cc.itemAround = _collider.gameObject;
			}
		}

		void OnTriggerExit(Collider _collider)
		{
			if (_collider.gameObject.tag == "Weapon")
			{
				cc.itemAround = null;
			}
		}
	}
}
