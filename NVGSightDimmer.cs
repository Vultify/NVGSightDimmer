using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Comfort.Common;
using EFT;
using EFT.CameraControl;
using BSG.CameraEffects;

namespace NVGSightDimmer
{
    [BepInPlugin("com.vultify.nvgsightdimmer", "NVG Sight Dimmer", "1.0.0")]
    public class NVGSightDimmerPlugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> _enabled;
        private ConfigEntry<float> _nvgBrightnessSights;
        private ConfigEntry<float> _nvgBrightnessScopes;

        private float _checkInterval = 0.15f;
        private float _timer;

        private readonly Dictionary<int, float> _baseHdr = new Dictionary<int, float>();
        private readonly Dictionary<int, Color> _baseColor = new Dictionary<int, Color>();

        private static readonly int HdrId = Shader.PropertyToID("_HDR");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        // OpticRetrice.material_0 is the exact material passed to CommandBuffer.DrawRenderer
        private static readonly FieldInfo ReticleMaterialField = typeof(OpticRetrice)
            .GetField("material_0", BindingFlags.NonPublic | BindingFlags.Instance);

        private void Awake()
        {
            _enabled = Config.Bind(
                "1. General",
                "Enable Auto Dim",
                true,
                "Automatically dim sights and scopes when NVGs are active");

            _nvgBrightnessSights = Config.Bind(
                "2. Settings",
                "NVG Sight Brightness (Holo / Red Dot)",
                0.3f,
                new ConfigDescription(
                    "Brightness multiplier for holographic and red dot sights when NVGs are on (lower = dimmer)",
                    new AcceptableValueRange<float>(0.01f, 1.0f)));

            _nvgBrightnessScopes = Config.Bind(
                "2. Settings",
                "NVG Sight Brightness (Scopes)",
                0.3f,
                new ConfigDescription(
                    "Brightness multiplier for scope reticles when NVGs are on (lower = dimmer)",
                    new AcceptableValueRange<float>(0.01f, 1.0f)));
        }

        private void LateUpdate()
        {
            if (!_enabled.Value)
                return;

            _timer += Time.deltaTime;
            if (_timer < _checkInterval)
                return;
            _timer = 0f;

            try
            {
                UpdateSights();
            }
            catch { }
        }

        private void UpdateSights()
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null) return;

            var player = gameWorld.MainPlayer;
            if (player?.PlayerBones?.WeaponRoot?.Original == null) return;

            var weaponRoot = player.PlayerBones.WeaponRoot.Original;

            var nvg = FindNightVision();
            bool nvgOn = nvg != null && nvg.On;
            float multiplierSights = nvgOn ? _nvgBrightnessSights.Value : 1f;
            float multiplierScopes = nvgOn ? _nvgBrightnessScopes.Value : 1f;

            // red dots and holos, their shader exposes _HDR
            var collimators = weaponRoot.GetComponentsInChildren<CollimatorSight>(true);
            foreach (var col in collimators)
            {
                if (col.CollimatorMeshRenderer == null) continue;
                ApplyHdr(col.CollimatorMeshRenderer, GetBaseHdr(col.CollimatorMeshRenderer, col.CollimatorMeshRenderer.sharedMaterial), multiplierSights);
            }

            // scope reticle shader only exposes _Color, scale its channels instead
            // TODO: acogs, prism scopes and some LPVOs still not dimming properly
            var opticRetrice = CameraClass.Instance?.OpticCameraManager?.OpticRetrice;
            if (opticRetrice != null && ReticleMaterialField != null)
            {
                var mat = ReticleMaterialField.GetValue(opticRetrice) as Material;
                if (mat != null)
                {
                    int matId = mat.GetInstanceID();
                    if (!_baseColor.TryGetValue(matId, out var baseColor))
                    {
                        baseColor = mat.GetColor(ColorId);
                        _baseColor[matId] = baseColor;
                    }

                    var c = baseColor;
                    c.r *= multiplierScopes;
                    c.g *= multiplierScopes;
                    c.b *= multiplierScopes;
                    c.a *= multiplierScopes;
                    mat.SetColor(ColorId, c);
                }
            }
        }

        private float GetBaseHdr(Renderer renderer, Material sharedMat)
        {
            int id = renderer.GetInstanceID();
            if (_baseHdr.TryGetValue(id, out var hdr))
                return hdr;

            float value = (sharedMat != null && sharedMat.HasProperty(HdrId)) ? sharedMat.GetFloat(HdrId) : 3f;
            _baseHdr[id] = value;
            return value;
        }

        private void ApplyHdr(Renderer renderer, float baseHdr, float multiplier)
        {
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            if (multiplier < 1f)
                block.SetFloat(HdrId, baseHdr * multiplier);
            else
                block.Clear();

            renderer.SetPropertyBlock(block);
        }

        private NightVision FindNightVision()
        {
            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                var nvg = cam.GetComponent<NightVision>();
                if (nvg != null)
                    return nvg;
            }
            return null;
        }
    }
}
