using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nankink.Controller
{
    public class MeleeWeapon : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player")
            {
                if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
                {
                    Debug.Log("BAtata");
                }
            }
        }
    }
}
