using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class TorchLight : MonoBehaviour
{
    protected Light[] lightsSources;
    protected float[] lightsIntensities;
    protected bool isLit = true;
    public bool IsLit => isLit;
    protected List<Tween> tweens = new List<Tween>();
    [SerializeField] protected float tweenDurantion = 0.25f;
    [SerializeField] protected Ease tweenEase = Ease.OutSine;
    [SerializeField] protected MeshRenderer[] emissionMeshs;
    protected List<Material> emissionMaterials = new List<Material>();
    protected Color[] materialsIntensities;

    private void Start()
    {
        foreach (var mesh in emissionMeshs)
        {
            var mats = mesh.materials;
            foreach (var m in mats) emissionMaterials.Add(m);
        }
        lightsSources = GetComponentsInChildren<Light>();
        lightsIntensities = lightsSources.Select(l => l.intensity).ToArray();
        materialsIntensities = emissionMaterials.Select(m => m.GetColor("_EmissionColor")).ToArray();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Torch"))
        {
            foreach (var t in tweens) t.Kill();
            isLit = !isLit;
            for (int i = 0; i < lightsSources.Length; i++)
            {
                var intensity = isLit ? lightsIntensities[i] : 0f;
                tweens.Add(lightsSources[i].DOIntensity(intensity, tweenDurantion).SetEase(Ease.OutSine));
            }
            for (int i = 0; i < emissionMaterials.Count; i++)
            {
                var c = materialsIntensities[i];
                var intensity = isLit ? c : new Color(c.r, c.g, c.b) / 64f;
                var curMat = emissionMaterials[i];

                if (isLit)
                {
                    curMat.EnableKeyword("_EMISSION");
                    curMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                }

                tweens.Add(curMat.DOColor(intensity, "_EmissionColor", tweenDurantion).SetEase(Ease.OutSine)
                .OnComplete(() => SetLit(curMat)));
            }
        }
    }

    protected void SetLit(Material mat)
    {
        if (isLit) return;
        mat.DisableKeyword("_EMISSION");
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
    }
}
