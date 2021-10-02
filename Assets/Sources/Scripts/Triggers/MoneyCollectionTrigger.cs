using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Level
{
    public class MoneyCollectionTrigger : MonoBehaviour
    {
        public int moneyCount = 10;
        //public Vector3 collectParticlesOffset = new Vector3(0,3,0);

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //ParticleManager?.//PlayCollectParticles(transform.position + collectParticlesOffset);
                gameObject.SetActive(false);
                StaticManager.AddMoneyOnLevel(moneyCount);
            }
        }
    }
}
