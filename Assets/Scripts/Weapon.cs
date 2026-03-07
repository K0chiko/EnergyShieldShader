using EnergyShield;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Common Components")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask shieldLayer;
    [SerializeField] private float range = 50f;

    [Header("Fire Timing (set in Inspector)")]
    [SerializeField] private float shotCooldownSeconds = 0f;

    [Header("Visual Hit Feedback")]
    [SerializeField] private float maxStrength = 5f;
    [SerializeField] private float hitRadius = 2f;
    [SerializeField] private float lerpSpeed = 10f;
        
        
    [Header("Damage")]
    [Tooltip("Weapon damage.")]
    [SerializeField] private float damage = 25f;

    [Header("Projectile Settings (used when cooldown > 0)")]
    [SerializeField] private Projectile projectilePrefab;
        
    private float _nextShotTime;
    private bool _triggerHeld;
    private float _currentStrength;
    private bool _isHittingShieldNow;
    private Renderer _currentShieldRenderer;
    private Material _instancedShieldMaterial;
        
    public void SetTrigger(bool isHeld)
    {
        _triggerHeld = isHeld;
    }
        
    public bool TryFireOnce()
    {
        if (shotCooldownSeconds <= 0f)
        {
            _triggerHeld = true;
            return true;
        }

        if (Time.time >= _nextShotTime)
        {
            FireProjectile();
            _nextShotTime = Time.time + shotCooldownSeconds;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (shotCooldownSeconds <= 0f)
        {
            if (_triggerHeld)
            {
                Shoot();
            }
            else
            {
                _isHittingShieldNow = false;
            }

            float targetStrength = (_triggerHeld && _isHittingShieldNow) ? maxStrength : 0f;
            _currentStrength = Mathf.Lerp(_currentStrength, targetStrength, Time.deltaTime * lerpSpeed);

            if (_instancedShieldMaterial != null)
            {
                _instancedShieldMaterial.SetFloat("_HitStrength", _currentStrength);

                if (!_triggerHeld && _currentStrength < 0.01f)
                {
                    _currentShieldRenderer = null;
                    _instancedShieldMaterial = null;
                }
            }
                
            _triggerHeld = false;
        }
        else
        {
            if (_triggerHeld && Time.time >= _nextShotTime)
            {
                FireProjectile();
                _nextShotTime = Time.time + shotCooldownSeconds;
            }
            _triggerHeld = false; 
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        var proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.Initialize(damage, shieldLayer, maxStrength, hitRadius, lerpSpeed);
    }

    private void Shoot()
    {
        if (firePoint == null) return;

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

            if (_instancedShieldMaterial != null)
            {
                _instancedShieldMaterial.SetVector("_HitPos", hit.point);
                _instancedShieldMaterial.SetFloat("_HitRadius", hitRadius);
            }

            var shield = hit.collider.GetComponentInParent<EnergyShieldController>();
            if (shield != null)
            {
                shield.RegisterHit(damage * Time.deltaTime, hit.point);
            }
            
        }
        else
        {
            _isHittingShieldNow = false;
        }
    }
    
}