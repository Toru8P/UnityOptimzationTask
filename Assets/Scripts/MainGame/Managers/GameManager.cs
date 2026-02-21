using System;
using MainGame.Characters;
using MainGame.Hazards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public PlayerCharacterController playerCharacterController;
        [SerializeField] private FireHazardScriptableObject[] fireHazardScriptableObjects;
        [SerializeField] private FireHazard[] fireHazards;

        private void Awake()
        {
            foreach (FireHazard fireHazard in fireHazards)
            {
                fireHazard.Setup(fireHazardScriptableObjects[Random.Range(0, fireHazardScriptableObjects.Length)]);
            }
        }

        private void OnEnable()
        {
            foreach (FireHazard fireHazard in fireHazards)
            {
                fireHazard.Subscribe(HandleCharacterEnteredFire);
            }
        }
        
        private void OnDisable()
        {
            foreach (FireHazard fireHazard in fireHazards)
            {
                fireHazard.Unsubscribe(HandleCharacterEnteredFire);
            }
      
        }

        private static void HandleCharacterEnteredFire(FireEnteredEventArgs args)
        {
            args.TargetCharacterController.TakeDamage(args.DamageDealt);
        }
    
    }
}
