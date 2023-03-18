using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeumorphismUI
{
    [ExecuteAlways]
    [AddComponentMenu("UI/Effects/Neumorphism/Neumorphism", 100)]
    public class Neumorphism : BaseMeshEffect
    {
        public enum Mode
        {
            Simple,
            Photo,
        }

        [SerializeField] Mode _mode = Mode.Simple;
        [SerializeField] Color _lightColor = new Color(1f, 1f, 1f, 0.68f);
        [SerializeField] Color _shadowColor = new Color(0.73f, 0.87f, 0.90f, 0.68f);
        [SerializeField, Range(0f, 24f)] float _pixelRange = 12f;
        [SerializeField, Range(0f, 1f)] float _bevelSize = 0.01f;
        [SerializeField, Range(-10f, 10f)] float _height = 4f;
        [SerializeField, Range(-10f, 10f)] float _clickedHeight = 4f;
        [SerializeField, Range(-10f, 10f)] float _mouseOverHeight = 4f;
        [SerializeField] bool _drawGradient = true;
        [SerializeField] bool _drawShadow = true;
        [SerializeField] bool _isTrigger = false;
        [SerializeField] bool _disableOnMouseExit = false;
        [SerializeField, Range(0f, 20f)] float _shadowOffset = 8f;
        [SerializeField] Texture _sdfTex = null;
        private float initialHeight;

        static readonly int _LightColor = Shader.PropertyToID("_LightColor");
        static readonly int _ShadowColor = Shader.PropertyToID("_ShadowColor");
        static readonly int _PixelRange = Shader.PropertyToID("_PixelRange");
        static readonly int _BevelSize = Shader.PropertyToID("_BevelSize");
        static readonly int _Height = Shader.PropertyToID("_Height");
        static readonly int _WorldRotation = Shader.PropertyToID("_WorldRotation");

        private Material material = null;
        private Quaternion rotation = Quaternion.identity;
        private List<IModifier> modifiers = new List<IModifier>();

        #region Setter / Getter
        public Color lightColor
        {
            get => _lightColor;
            set => _lightColor = value;
        }

        public Color shadowColor
        {
            get => _shadowColor;
            set => _shadowColor = value;
        }

        public float pixelRange
        {
            get => _pixelRange;
            set => _pixelRange = value;
        }

        public float bevelSize
        {
            get => _bevelSize;
            set => _bevelSize = value;
        }

        public float height
        {
            get => _height;
            set => _height = value;
        }

        public float clickedHeight
        {
            get => _clickedHeight;
            set => _clickedHeight = value;
        }

        public float mouseOverHeight
        {
            get => _mouseOverHeight;
            set => _mouseOverHeight = value;
        }

        public bool drawGradient
        {
            get => _drawGradient;
            set => _drawGradient = value;
        }

        public bool isTrigger
        {
            get => _isTrigger;
            set => _isTrigger = value;
        }


        public bool drawShadow
        {
            get => _drawShadow;
            set => _drawShadow = value;
        }
        public bool disableOnMouseExit
        {
            get => _disableOnMouseExit;
            set => _disableOnMouseExit = value;
        }

        public float shadowOffset
        {
            get => _shadowOffset;
            set => _shadowOffset = value;
        }
        #endregion // Setter / Getter

#if UNITY_EDITOR
        protected override void Awake()
        {
            base.Awake();

            // Need all shader channels to use Neumophism script
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
            }
        }
#endif // UNITY_EDITOR

        protected override void Start()
        {
            base.Start();

            if (disableOnMouseExit)
                enabled = false;

            initialHeight = height;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

    //        CheckMaterial();

            // Setup modifieres
            modifiers.Clear();
            if (drawGradient)
            {
                modifiers.Add(new Gradient());
            }
            if (drawShadow)
            {
                modifiers.Add(new Shadow(Shadow.Mode.Light));
                modifiers.Add(new Shadow(Shadow.Mode.Shadow));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (material != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(material);
                }
                else
                {
                    // DestroyImmediate(material);
                }
            }

            modifiers.Clear();
        }

        private void Update()
        {
            CheckMaterial();

            material.SetColor(_LightColor, _lightColor);
            material.SetColor(_ShadowColor, _shadowColor);
            material.SetFloat(_PixelRange, _pixelRange);
            material.SetFloat(_BevelSize, _bevelSize);
            material.SetFloat(_Height, _height);

            var rotation = transform.rotation;
            if (rotation != this.rotation)
            {
                material.SetMatrix(_WorldRotation, Matrix4x4.Rotate(transform.rotation).inverse);
                this.rotation = transform.rotation;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            foreach (var mod in modifiers)
            {
                mod.ModifyMesh(this, vh);
            }
        }

        private void CheckMaterial()
        {
            // Activate material
            if (material == null)
            {
                material = GetMaterial(_mode);
                if (_mode == Mode.Photo)
                {
                    material.SetTexture("_SDFTex", _sdfTex);
                }
            }
            graphic.material = material;
        }

        private static Material GetMaterial(Mode mode)
        {
            switch (mode)
            {
                case Mode.Simple:
                    return new Material(Shader.Find("UI/Neumorphism"));
                case Mode.Photo:
                    return new Material(Shader.Find("UI/Neumorphism Photo"));
            }
            throw new System.Exception($"Not supported mode: {mode}");
        }

        public void Click()
        {
            if(_height == _clickedHeight)
                _height = initialHeight;
            else
                _height = _clickedHeight;
        }

        public void Collapse()
        {
            _height = initialHeight;
        }

        public void PointerEnter()
        {
            this.enabled = true;
            if (_height != _clickedHeight)
            {
                _height = _mouseOverHeight;
            }
        }

        public void PointerExit()
        {
            if (_height != _clickedHeight || isTrigger)
            {
                _height = initialHeight;
                if (disableOnMouseExit)
                    this.enabled = false;
            }
        }

        public void PointerDown()
        {
            if(isTrigger)
                _height = _clickedHeight;
        }

        public void PointerUp()
        {
            if (isTrigger)
                _height = _mouseOverHeight;
        }

    }
}
