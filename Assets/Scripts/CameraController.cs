using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSW
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private Transform target;

        // Start is called before the first frame update
        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 20 * Time.deltaTime);
        }
    }

}