using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSW.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 1f;

        private Animator _anim;
        public IInputHandler _inputHandler;

        // Start is called before the first frame update
        void Start()
        {
            if (_inputHandler == null)
                _inputHandler = new InputHandler();

            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
        }

        void HandleMovement()
        {
            float vert = _inputHandler.GetVerticalAxis();
            float horiz = _inputHandler.GetHorizontalAxis();

            transform.position += new Vector3(horiz, vert, 0) 
                * _inputHandler.GetDeltaTime() * movementSpeed;

            _anim.SetFloat("Vert", vert);
            _anim.SetFloat("Horiz", horiz);

            if(vert == 1 || vert == -1 || horiz == 1 || horiz == -1)
            {
                _anim.SetFloat("LastVert", vert);
                _anim.SetFloat("LastHoriz", horiz);
            }
        }
    }
}

