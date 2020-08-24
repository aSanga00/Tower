using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Chara
{

    public class BaseAvator : MonoBehaviour
    {
        [Header("移動スピード"), Range(0, 30)]
        [SerializeField] private float speed;
        [SerializeField] private Transform target;
        private Vector3 homePosition;
        private Vector3 targetPosition;

        private Vector3 GetTargetPosition()
        {
            return targetPosition - transform.localPosition;
        }
        // Use this for initialization
        private void Awake()
        {
            targetPosition = target.localPosition;
            homePosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            MovePosition();
        }

        private void MovePosition()
        {
            
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(homePosition, targetPosition, step);
            homePosition = transform.localPosition;
        }

    }
}