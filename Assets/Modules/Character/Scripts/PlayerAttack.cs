using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nankink.Controller
{
    public class PlayerAttack : MonoBehaviour
    {
        public Transform weaponHand;

        public GameObject swordPrefab;

        Collider swordCollider;

        private void Start()
        {
            if (weaponHand.childCount == 0)
            {
                Debug.Log("No weapon in hand. Instantiating..");
                Instantiate(swordPrefab, Vector3.zero, Quaternion.identity, weaponHand);
            }
            swordCollider = weaponHand.GetChild(0).GetComponent<Collider>();
            swordCollider.enabled = false;
        }

        public void EnableCollision(int atkID)
        {
            if (swordCollider == null)
            {
                Debug.LogWarning("Could not find sword");
            }
            else
            {
                swordCollider.enabled = true;
            }
        }
        public void DisableCollision()
        {
            if (swordCollider == null)
            {
                Debug.LogWarning("Could not find sword");
            }
            else
            {
                swordCollider.enabled = false;
            }
        }
    }
}
