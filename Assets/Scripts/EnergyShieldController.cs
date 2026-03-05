using System.Collections;
using UnityEngine;

namespace EnergyShield
{
    /// <summary>
    /// Controls Energy Shield shader parameters and the shield transform during a deploy/retract effect,
    /// then restores original values.
    /// Shader properties expected (must match EnergyShield.shader):
    /// - _MainTexture (for tiling via texture scale)
    /// - _Smoothness (float)
    /// - _Alpha (float)
    /// - _VertexAmount (float3/vector)
    /// - _HexColor (color)
    /// - _hexPulseSpeed (float)
    /// </summary>
    [DisallowMultipleComponent]
    public class EnergyShieldController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Renderer that uses the EnergyShield material. If empty, will use Renderer on this GameObject.")]
        public Renderer shieldRenderer;

        [Tooltip("Transform to scale for the shield size. Defaults to this.transform.")]
        public Transform shieldTransform;

        [Header("Timings")]
        [Min(0f)] public float deployDuration = 0.6f;
        [Min(0f)] public float holdDuration = 0.2f;
        [Min(0f)] public float retractDuration = 0.6f;
        public AnimationCurve deployCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve retractCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Parameter Deltas (applied relative to current, then restored)")]
        public Vector2 tilingDelta = Vector2.zero; // _MainTexture tiling XY
        [Range(-1f, 1f)] public float smoothnessDelta = 0.0f; // _Smoothness
        [Range(-1f, 1f)] public float alphaDelta = 0.0f; // _Alpha
        public Vector3 vertexAmountDelta = Vector3.zero; // _VertexAmount (X,Y,Z)
        public bool changeHexColor = false;
        public Color hexColorTarget = Color.white; // _HexColor (only used if changeHexColor)
        public float hexPulseSpeedDelta = 0.0f; // _hexPulseSpeed
        public Vector3 scaleDelta = Vector3.zero; // local scale delta

        [Header("Auto Start")]
        public bool playOnEnable = false;

        // Runtime state
        Material _mat;
        bool _isPlaying;

        // Cached originals
        Vector2 _origTiling;
        float _origSmoothness;
        float _origAlpha;
        Vector3 _origVertexAmount;
        Color _origHexColor;
        float _origHexPulseSpeed;
        Vector3 _origLocalScale;

        const string PROP_MAIN_TEX = "_MainTexture";
        const string PROP_SMOOTHNESS = "_Smoothness";
        const string PROP_ALPHA = "_Alpha";
        const string PROP_VERTEX_AMT = "_VertexAmount";
        const string PROP_HEX_COLOR = "_HexColor";
        const string PROP_HEX_PULSE_SPEED = "_hexPulseSpeed"; // note lowercase 'h'

        void Awake()
        {
            if (!shieldRenderer) shieldRenderer = GetComponent<Renderer>();
            if (!shieldTransform) shieldTransform = transform;

            if (shieldRenderer)
            {
                // Create a unique material instance for safe runtime modifications
                _mat = shieldRenderer.material;

                // Cache originals
                _origTiling = _mat.GetTextureScale(PROP_MAIN_TEX);
                _origSmoothness = SafeGetFloat(_mat, PROP_SMOOTHNESS, 0.5f);
                _origAlpha = SafeGetFloat(_mat, PROP_ALPHA, 0f);
                _origVertexAmount = SafeGetVector3(_mat, PROP_VERTEX_AMT, Vector3.zero);
                _origHexColor = _mat.HasProperty(PROP_HEX_COLOR) ? _mat.GetColor(PROP_HEX_COLOR) : Color.white;
                _origHexPulseSpeed = SafeGetFloat(_mat, PROP_HEX_PULSE_SPEED, 1f);
            }

            _origLocalScale = shieldTransform ? shieldTransform.localScale : Vector3.one;
        }

        void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        /// <summary>
        /// Starts the deploy-hold-retract sequence.
        /// If already playing, restarts from the beginning.
        /// </summary>
        public void Play()
        {
            if (!gameObject.activeInHierarchy) return;
            if (_isPlaying) StopAllCoroutines();
            StartCoroutine(PlayRoutine());
        }

        IEnumerator PlayRoutine()
        {
            _isPlaying = true;

            // Compute targets (relative deltas)
            var targetTiling = _origTiling + tilingDelta;
            var targetSmoothness = Mathf.Clamp01(_origSmoothness + smoothnessDelta);
            var targetAlpha = Mathf.Clamp01(_origAlpha + alphaDelta);
            var targetVertex = _origVertexAmount + vertexAmountDelta;
            var targetHexColor = changeHexColor ? hexColorTarget : _origHexColor;
            var targetPulse = _origHexPulseSpeed + hexPulseSpeedDelta;
            var targetScale = _origLocalScale + scaleDelta;

            // Deploy
            if (deployDuration > 0f)
            {
                float t = 0f;
                while (t < deployDuration)
                {
                    t += Time.deltaTime;
                    float k = deployCurve.Evaluate(Mathf.Clamp01(t / deployDuration));
                    ApplyLerp(k, targetTiling, targetSmoothness, targetAlpha, targetVertex, targetHexColor, targetPulse, targetScale);
                    yield return null;
                }
            }
            else
            {
                ApplyLerp(1f, targetTiling, targetSmoothness, targetAlpha, targetVertex, targetHexColor, targetPulse, targetScale);
            }

            // Hold
            if (holdDuration > 0f)
                yield return new WaitForSeconds(holdDuration);

            // Retract back to originals
            if (retractDuration > 0f)
            {
                float t = 0f;
                while (t < retractDuration)
                {
                    t += Time.deltaTime;
                    float k = retractCurve.Evaluate(Mathf.Clamp01(t / retractDuration));
                    // Reverse: interpolate from target back to original using (1-k)
                    ApplyLerp(1f - k, targetTiling, targetSmoothness, targetAlpha, targetVertex, targetHexColor, targetPulse, targetScale);
                    yield return null;
                }
            }

            // Ensure exact originals restored
            ApplyValues(_origTiling, _origSmoothness, _origAlpha, _origVertexAmount, _origHexColor, _origHexPulseSpeed, _origLocalScale);

            _isPlaying = false;
        }

        void ApplyLerp(float k,
            Vector2 targetTiling, float targetSmoothness, float targetAlpha,
            Vector3 targetVertex, Color targetHexColor, float targetPulse, Vector3 targetScale)
        {
            var tiling = Vector2.Lerp(_origTiling, targetTiling, k);
            var smoothness = Mathf.Lerp(_origSmoothness, targetSmoothness, k);
            var alpha = Mathf.Lerp(_origAlpha, targetAlpha, k);
            var vertex = Vector3.Lerp(_origVertexAmount, targetVertex, k);
            var hexColor = Color.Lerp(_origHexColor, targetHexColor, k);
            var pulse = Mathf.Lerp(_origHexPulseSpeed, targetPulse, k);
            var scale = Vector3.Lerp(_origLocalScale, targetScale, k);

            ApplyValues(tiling, smoothness, alpha, vertex, hexColor, pulse, scale);
        }

        void ApplyValues(Vector2 tiling, float smoothness, float alpha, Vector3 vertexAmount, Color hexColor, float hexPulseSpeed, Vector3 localScale)
        {
            if (_mat)
            {
                _mat.SetTextureScale(PROP_MAIN_TEX, tiling);
                if (_mat.HasProperty(PROP_SMOOTHNESS)) _mat.SetFloat(PROP_SMOOTHNESS, smoothness);
                if (_mat.HasProperty(PROP_ALPHA)) _mat.SetFloat(PROP_ALPHA, alpha);
                if (_mat.HasProperty(PROP_VERTEX_AMT)) _mat.SetVector(PROP_VERTEX_AMT, new Vector4(vertexAmount.x, vertexAmount.y, vertexAmount.z, 0f));
                if (_mat.HasProperty(PROP_HEX_COLOR)) _mat.SetColor(PROP_HEX_COLOR, hexColor);
                if (_mat.HasProperty(PROP_HEX_PULSE_SPEED)) _mat.SetFloat(PROP_HEX_PULSE_SPEED, hexPulseSpeed);
            }

            if (shieldTransform)
                shieldTransform.localScale = localScale;
        }

        static float SafeGetFloat(Material m, string prop, float fallback)
            => m != null && m.HasProperty(prop) ? m.GetFloat(prop) : fallback;

        static Vector3 SafeGetVector3(Material m, string prop, Vector3 fallback)
        {
            if (m != null && m.HasProperty(prop))
            {
                var v4 = m.GetVector(prop);
                return new Vector3(v4.x, v4.y, v4.z);
            }
            return fallback;
        }
    }
}
