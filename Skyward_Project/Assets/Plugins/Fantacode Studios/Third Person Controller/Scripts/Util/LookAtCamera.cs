using UnityEngine;

namespace Skyward.Characters
{
    public class LookAtCamera : MonoBehaviour
    {
        Transform cam;
        private void Start()
        {
            cam = Camera.main.transform;
        }

        private void Update()
        {
            transform.forward = cam.forward;
        }
    }
}