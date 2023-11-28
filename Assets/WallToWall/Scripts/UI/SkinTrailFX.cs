using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SkinTrailFX : MonoBehaviour
{
    [SerializeField] private ParticleImage particleImage;
    [SerializeField] private ParticleImage finalFx;

    private Material _material;
    private ParticleImage final;
    private RectTransform _target;

    public void Initialize(Sprite sprite, RectTransform target, UnityAction onFinish = null, UnityAction onStart = null,
        UnityAction onStop = null)
    {
        particleImage.Play();
        particleImage.texture = sprite.texture;
        particleImage.attractorTarget = target;
        particleImage.onStart.AddListener(onStart);
        particleImage.onStop.AddListener(onStop);
        particleImage.onParticleFinish.AddListener(Dispose);
        if (final == null)
        {
            final = Instantiate(finalFx, target.parent, false);
            _material = final.material;
            _target = target;
        }

        final.Stop();
    }

    private void Dispose()
    {
        final.transform.position = _target.position;
        _target.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _target.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
        });
        final.Play();
        UpdateMaterial();
        particleImage.Stop();
        particleImage.onStart.RemoveAllListeners();
        particleImage.onStop.RemoveAllListeners();
        particleImage.onParticleFinish.RemoveAllListeners();
    }

    private void UpdateMaterial()
    {
        int hsv = Random.Range(0, 360);
        _material.SetInt("_HsvShift", hsv);
    }

    private void OnDestroy()
    {
        particleImage.onStart.RemoveAllListeners();
        particleImage.onStop.RemoveAllListeners();
        particleImage.onParticleFinish.RemoveAllListeners();
    }
}