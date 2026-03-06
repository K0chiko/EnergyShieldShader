using UnityEngine;

namespace EnergyShield
{
    /// <summary>
    /// Animation-driven energy shield controller. Plays deploy/retract/damage/destroy animations
    /// and maintains shield health. No shader parameter tweening here.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnergyShieldController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Renderer shieldRenderer;
        [SerializeField] private Collider shieldCollider; 

        [Header("Animator Parameters")]
        [SerializeField] private string deployTrigger = "Deploy";
        [SerializeField] private string retractTrigger = "Retract";
        [SerializeField] private string isUnderAttackBool = "IsUnderAttack";
        [SerializeField] private string destroyTrigger = "Destroy";

        [Header("Health")]
        [Min(1)] [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        [Header("Behavior")]
        [SerializeField] private bool activeOnEnable = false;
        
        [Header("Hit Settings")]
        [SerializeField] private float hitResetDelay = 0.5f; 
        private float _lastHitTime;

        bool _isDeployed;

        void Reset()
        {
            animator = GetComponent<Animator>();
            shieldRenderer = GetComponent<Renderer>();
            shieldCollider = GetComponent<Collider>();
        }

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (!shieldRenderer) shieldRenderer = GetComponent<Renderer>();
            if (!shieldCollider) shieldCollider = GetComponent<Collider>();

            currentHealth = Mathf.Clamp(currentHealth <= 0 ? maxHealth : currentHealth, 0, maxHealth);
        }

        void OnEnable()
        {
            if (activeOnEnable)
            {
                Deploy();
            }
            else
            {
                _isDeployed = false;
                if (shieldCollider) shieldCollider.enabled = false;
                if (shieldRenderer) shieldRenderer.enabled = false;
            }
        }

 
        
        public void Play()
        {
            if (_isDeployed)
            {
                Retract();
            }
            else
            {
                Deploy();
            }
        }
        

        public void Deploy()
        {
            if (_isDeployed) return;
            currentHealth = maxHealth;

    
            Trigger(deployTrigger);
        }
        
        public void Retract()
        {
            if (!_isDeployed) return;

            Trigger(retractTrigger); 
        }
        void Update()
        {
            if (_isDeployed && Time.time - _lastHitTime > hitResetDelay)
            {
                SetUnderAttack(false);
            }
        }
        
        public void RegisterHit(float damage, Vector3 hitPoint)
        {
            if (!_isDeployed || currentHealth <= 0) return;

            int dmg = Mathf.Max(0, Mathf.RoundToInt(damage));
            currentHealth = Mathf.Max(0, currentHealth - dmg);

            if (currentHealth > 0)
            {
                _lastHitTime = Time.time; 
                SetUnderAttack(true);
            }
            else
            {
                SetUnderAttack(false);
                Trigger(destroyTrigger);
            }
        }
        private void SetUnderAttack(bool active)
        {
            if (animator)
            {
                animator.SetBool(isUnderAttackBool, active);
            }
        }

 

        // These methods can be called as animation events
        public void OnDeployed()
        {
            _isDeployed = true;
            if (shieldCollider) shieldCollider.enabled = true;
            if (shieldRenderer) shieldRenderer.enabled = true;
        }

        public void OnRetracted()
        {
            _isDeployed = false;
            if (shieldCollider) shieldCollider.enabled = false;
            if (shieldRenderer) shieldRenderer.enabled = false;
        }

        public void OnDestroyed()
        {
            _isDeployed = false;
            if (shieldCollider) shieldCollider.enabled = false;
            if (shieldRenderer) shieldRenderer.enabled = false;
        }

        void SetActiveState(bool active)
        {
            _isDeployed = active;
            if (shieldCollider) shieldCollider.enabled = active;
            if (shieldRenderer) shieldRenderer.enabled = active;
            if (active && currentHealth <= 0) currentHealth = maxHealth;
        }

        void Trigger(string trigger)
        {
            if (animator && !string.IsNullOrEmpty(trigger))
            {
                animator.ResetTrigger(retractTrigger);
                animator.ResetTrigger(deployTrigger);
                animator.ResetTrigger(destroyTrigger);
                animator.SetTrigger(trigger);
            }
        }

        public bool IsDeployed => _isDeployed;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
    }
}

