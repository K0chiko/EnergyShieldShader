using UnityEngine;
using UnityEngine.InputSystem; 
namespace SideViewShooter
{
    public class LaserController : MonoBehaviour
    {
        [Header("Laser Components")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private LineRenderer laserLine;
        [SerializeField] private LayerMask shieldLayer;

        [Header("Shader Settings")]
        [SerializeField] private float maxStrength = 5f;
        [SerializeField] private float hitRadius = 2f;
        [SerializeField] private float lerpSpeed = 10f;
        [SerializeField] private float range = 50f;

        private InputAction _attackAction;
        private float _currentStrength;
        private Material _shieldMaterial;

        private void Awake()
        {
            var playerInput = GetComponent<PlayerInput>();
            
            _attackAction = playerInput.actions["Attack"];
        }

        private void Update()
        {
            bool isAttacking = _attackAction.IsPressed();

            if (isAttacking)
            {
                ShootLaser();
            }
            else
            {
                StopLaser();
            }


            float targetStrength = isAttacking ? maxStrength : 0f;
            _currentStrength = Mathf.Lerp(_currentStrength, targetStrength, Time.deltaTime * lerpSpeed);
            
            if (_shieldMaterial != null)
            {
                _shieldMaterial.SetFloat("_HitStrength", _currentStrength);
            }
        }

        private void ShootLaser()
        {
            Ray ray = new Ray(firePoint.position, firePoint.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, shieldLayer))
            {
                _shieldMaterial = hit.collider.GetComponent<Renderer>().material;
                
                _shieldMaterial.SetVector("_HitPos", hit.point);
                _shieldMaterial.SetFloat("_HitRadius", hitRadius);
                
                if (laserLine != null)
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, firePoint.position);
                    laserLine.SetPosition(1, hit.point);
                }
            }
            else
            {
                if (laserLine != null)
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, firePoint.position);
                    laserLine.SetPosition(1, firePoint.position + firePoint.forward * range);
                }
            }
        }

        private void StopLaser()
        {
            if (laserLine != null)
            {
                laserLine.enabled = false;
            }
        }
    }
}