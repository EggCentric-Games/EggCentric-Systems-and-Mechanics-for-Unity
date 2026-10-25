using UnityEngine;

namespace EggCentric.Ballistics
{
    public class Bullet2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private float _speed;

        private void Update() => transform.position += transform.right * _speed * Time.deltaTime;

        private void FixedUpdate()
        {
            if (CollissionCheck(Time.fixedDeltaTime))
                enabled = false;
        }

        private bool CollissionCheck(float timeStep) => Physics2D.Raycast(transform.position, transform.right, _speed * timeStep);
    }
}