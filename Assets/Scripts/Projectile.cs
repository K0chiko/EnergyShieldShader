using EnergyShield;
using SideViewShooter;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxLifeTime = 6f;
    [SerializeField] private float maxDistance = 100f;

    private float _damage;
    private LayerMask _shieldLayer;
    private float _maxStrength;
    private float _hitRadius;
    private float _lerpSpeed;
    private float _lifeTimer;
    private Vector3 _prevPos;
    private float _traveled;

    public void Initialize(float damage, LayerMask shieldLayer, float maxStrength, float hitRadius, float lerpSpeed)
    {
        _damage = damage;
        _shieldLayer = shieldLayer;
        _maxStrength = maxStrength;
        _hitRadius = hitRadius;
        _lerpSpeed = lerpSpeed;
    }

    private void Awake()
    {
        _prevPos = transform.position;
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= maxLifeTime || _traveled >= maxDistance)
        {
            Destroy(gameObject);
            return;
        }
            
        Vector3 nextPos = transform.position + transform.forward * (speed * Time.deltaTime);
            
        Vector3 dir = nextPos - _prevPos;
        float dist = dir.magnitude;
        if (dist > 0.0001f)
        {
            if (Physics.Raycast(_prevPos, dir.normalized, out RaycastHit hit, dist, _shieldLayer))
            {
                var shield = hit.collider.GetComponentInParent<EnergyShieldController>();
                if (shield != null)
                {
                    shield.RegisterHit(_damage, hit.point);
                }
                    
                var hitRenderer = hit.collider.GetComponent<Renderer>();
                if (hitRenderer != null)
                {
                    var pulse = hitRenderer.GetComponent<ShieldHitPulse>();
                    if (pulse == null)
                    {
                        pulse = hitRenderer.gameObject.AddComponent<ShieldHitPulse>();
                    }
                    pulse.Trigger(hit.point, _maxStrength, _hitRadius, _lerpSpeed, hitRenderer);
                }
                Destroy(gameObject);
                return;
            }
        }

        transform.position = nextPos;
        _traveled += dist;
        _prevPos = transform.position;
    }
}