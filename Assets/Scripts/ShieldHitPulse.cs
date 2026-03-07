using UnityEngine;


    // Helper component placed on a shield Renderer to play a short hit pulse
    // by writing shader parameters: _HitPos, _HitRadius, _HitStrength.
    public class ShieldHitPulse : MonoBehaviour
    {
        private Renderer _renderer;
        private Material _materialInstance;
        private float _currentStrength;
        private float _lerpSpeed;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _materialInstance = _renderer.material; // instanced material
            }
        }

        private void OnDestroy()
        {
            // Let Unity handle material cleanup; we don't manually destroy it here.
        }

        public void Trigger(Vector3 hitPos, float maxStrength, float hitRadius, float lerpSpeed, Renderer sourceRenderer)
        {
            if (_renderer == null || _renderer != sourceRenderer)
            {
                _renderer = sourceRenderer != null ? sourceRenderer : GetComponent<Renderer>();
                if (_renderer != null)
                {
                    _materialInstance = _renderer.material;
                }
            }

            _lerpSpeed = Mathf.Max(0.01f, lerpSpeed);
            _currentStrength = Mathf.Max(_currentStrength, maxStrength); // stack/refresh to the higher

            if (_materialInstance != null)
            {
                _materialInstance.SetVector("_HitPos", hitPos);
                _materialInstance.SetFloat("_HitRadius", hitRadius);
                _materialInstance.SetFloat("_HitStrength", _currentStrength);
            }
        }

        private void Update()
        {
            if (_materialInstance == null) return;

            if (_currentStrength > 0f)
            {
                _currentStrength = Mathf.Lerp(_currentStrength, 0f, Time.deltaTime * _lerpSpeed);
                _materialInstance.SetFloat("_HitStrength", _currentStrength);
            }
        }
    }

