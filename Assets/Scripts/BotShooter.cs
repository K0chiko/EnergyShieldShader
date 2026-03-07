using UnityEngine;


    [RequireComponent(typeof(Weapon))]
    public class BotShooter : MonoBehaviour
    {
        [Header("Targeting")]
        public Transform target; 

        [Header("Bot Fire Cadence")]
        [Tooltip("How long (in seconds) the bot holds the trigger each cycle. New cycles start every X seconds.")]
        public float attemptEverySeconds = 2f; 

        private Weapon _weapon;
        private float _nextAttempt;   
        private bool _isHoldingTrigger;
        private float _releaseAt;     

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        private void Update()
        {
            AimAtTarget();

            float holdDuration = Mathf.Max(0.01f, attemptEverySeconds);

            if (_isHoldingTrigger)
            {
                _weapon.SetTrigger(true);

                if (Time.time >= _releaseAt)
                {
                    _isHoldingTrigger = false;
                    _weapon.SetTrigger(false);
                }
            }
            else
            {
                _weapon.SetTrigger(false);

                if (Time.time >= _nextAttempt)
                {
                    _isHoldingTrigger = true;
                    _releaseAt = Time.time + holdDuration;
                    _weapon.SetTrigger(true); 
                    _nextAttempt = Time.time + holdDuration;
                }
            }
        }

        private void AimAtTarget()
        {
            if (target == null) return;
            Vector3 dir = (target.position - transform.position).normalized;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10f);
            }
        }
    }

