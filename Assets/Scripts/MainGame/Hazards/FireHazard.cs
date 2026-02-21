using System;
using System.Diagnostics;
using MainGame.Characters;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace MainGame.Hazards
{
    public class FireHazard : MonoBehaviour
    {
        [SerializeField] private UnityEvent<FireEnteredEventArgs> onCharacterEntered = new UnityEvent<FireEnteredEventArgs>();

        private FireHazardScriptableObject _fireHazardData;
        
        public void Subscribe(UnityAction<FireEnteredEventArgs> input)
        {
            onCharacterEntered.AddListener(input);
        }
        
        public void Unsubscribe(UnityAction<FireEnteredEventArgs> input)
        {
            onCharacterEntered.RemoveListener(input);
        }

        public void Setup(FireHazardScriptableObject fireHazardData)
        {
            _fireHazardData = fireHazardData;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("PlayerCharacter")) return;
            Debug.Log("Player entered this hazard");

            if (!_fireHazardData)
            {
                throw new Exception("Someone tried to enter this hazard but data wasn't set");
            }
                
            FireEnteredEventArgs fireEnteredEventArgs = new FireEnteredEventArgs
            {
                DamageDealt = _fireHazardData.GetRandomFireDamage(),
                TargetCharacterController = other.GetComponent<PlayerCharacterController>()
            };
            onCharacterEntered?.Invoke(fireEnteredEventArgs);
        }
    }

    public class FireEnteredEventArgs
    {
        public int DamageDealt;
        public PlayerCharacterController TargetCharacterController;
    }
}