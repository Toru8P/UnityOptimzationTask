using UnityEngine;

namespace MainGame.Hazards
{
    public class ArrowHazard : MonoBehaviour
    {
        public GameObject arrowPrefab;
        [SerializeField] float shootInterval;
        private float _shootIntervalLeft;
    
        private void Awake()
        {
        
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _shootIntervalLeft = shootInterval;
        }

        // Update is called once per frame
        void Update()
        {
            _shootIntervalLeft -= Time.deltaTime;
            if (_shootIntervalLeft <= 0)
            {
                ArrowObject arrow = Instantiate(arrowPrefab,transform.position,Quaternion.identity).GetComponent<ArrowObject>();
                arrow.transform.Rotate(0,180,0);
                _shootIntervalLeft = shootInterval;
            }
        }
    }
}
