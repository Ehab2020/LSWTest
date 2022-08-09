using UnityEngine;

namespace LSW.Player
{
    public interface IInputHandler
    {
        float GetVerticalAxis();
        float GetHorizontalAxis();
        float GetDeltaTime();
    }
    public class InputHandler : IInputHandler
    {
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }

        public float GetHorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }
    }
}

