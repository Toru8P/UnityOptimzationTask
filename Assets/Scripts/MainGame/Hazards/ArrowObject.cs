using System;
using UnityEngine;

namespace MainGame.Hazards
{
    public class ArrowObject : MonoBehaviour
    {
        public float speed;
        public float damage;
        public float endTime;

        private float currentTime;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void LateUpdate()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= endTime) Destroy(gameObject);
        }
    }
}
