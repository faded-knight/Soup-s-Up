using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class IngredientController : MonoBehaviour, IPointerClickHandler
    {
        //------------------------------------dependencies------------------------------------
        [Inject] IngretientSettings _settings;

        //-----------------------------------------API------------------------------------------
        public void OnPointerClick(PointerEventData eventData)
        {
            _rigidbody.AddForce(bounceForce, ForceMode.VelocityChange);
        }

        public void BounceUp()
        {
            _rigidbody.AddForce(bounceForce, ForceMode.VelocityChange);
        }

        //----------------------------------Unity Messages----------------------------------
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            // make sure speed doesn't exceed a certain point
            if (isFalling && isMovingFast)
            {
                // slow down the diving speed
                _rigidbody.velocity = Vector3.Scale(_rigidbody.velocity, new Vector3(1.0f, 0.85f, 1.0f));
            }
        }
        //------------------------------------memory pool------------------------------------
        public class Pool : MonoMemoryPool<Vector3, IngredientController>
        {
            protected override void Reinitialize(Vector3 position, IngredientController ingredientController)
            {
                ingredientController.Reset(position);
            }
        }

        void Reset(Vector3 position)
        {
            transform.position = position;
        }
        //----------------------------------------details----------------------------------------
        Rigidbody _rigidbody;

        Vector3 bounceForce
        {
            get
            {
                Vector3 result = Vector3.zero;
                result.y = _settings.UpwardForce;
                result.x = Random.Range(-_settings.BouncingAngle, _settings.BouncingAngle);
                return result;
            }
        }

        bool isFalling
        {
            get { return _rigidbody.velocity.y < 0; }
        }

        bool isMovingFast
        {
            get { return _rigidbody.velocity.sqrMagnitude > _settings.MaxFallingSpeed; }
        }
    }
}
