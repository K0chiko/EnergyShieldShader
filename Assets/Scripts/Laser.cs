using UnityEngine;
using UnityEngine.InputSystem;
using EnergyShield;

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

        [Header("Damage")]
        [SerializeField] private float damagePerSecond = 25f;

        private InputAction _attackAction;
        private float _currentStrength;
        
        private Renderer _currentShieldRenderer;
        private Material _instancedShieldMaterial;
        
        // Добавляем флаг фактического попадания
        private bool _isHittingShieldNow;

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
                _isHittingShieldNow = false;
            }
            
            float targetStrength = (isAttacking && _isHittingShieldNow) ? maxStrength : 0f;
            
            _currentStrength = Mathf.Lerp(_currentStrength, targetStrength, Time.deltaTime * lerpSpeed);
            
            if (_instancedShieldMaterial != null)
            {
                _instancedShieldMaterial.SetFloat("_HitStrength", _currentStrength);
                
                if (!isAttacking && _currentStrength < 0.01f)
                {
                    _currentShieldRenderer = null;
                    _instancedShieldMaterial = null;
                }
            }
        }

        private void ShootLaser()
        {
            Ray ray = new Ray(firePoint.position, firePoint.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range, shieldLayer))
            {
                _isHittingShieldNow = true;
                Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
            
                if (_currentShieldRenderer != hitRenderer)
                {
                    _currentShieldRenderer = hitRenderer;
                    _instancedShieldMaterial = hitRenderer.material;
                }

                _instancedShieldMaterial.SetVector("_HitPos", hit.point);
                _instancedShieldMaterial.SetFloat("_HitRadius", hitRadius);

                var shield = hit.collider.GetComponentInParent<EnergyShieldController>();
                if (shield != null)
                {
                    shield.RegisterHit(damagePerSecond * Time.deltaTime, hit.point);
                }

                if (laserLine != null)
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, firePoint.position);
                    laserLine.SetPosition(1, hit.point);
                }
            }
            else
            {
                _isHittingShieldNow = false; 
                
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