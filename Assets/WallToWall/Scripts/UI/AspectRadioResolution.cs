using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class AspectRadioResolution : MonoBehaviour
{
    private float _masterWidth = 1940f;
    private float _masterHeight = 1080f;
    private AspectRatioFitter _aspectRatioFitter;
    private float _masterAspectRatio = 1.777778f;

    [Button]
    private void Awake()
    {
        _aspectRatioFitter = GetComponent<AspectRatioFitter>();
        
        float currentWidth = Screen.width;
        float currentHeight = Screen.height;
        float aspectRatio = currentWidth / currentHeight;
        float masterAspectRatio = _masterWidth / _masterHeight;
        float ratio = aspectRatio / masterAspectRatio;
        _aspectRatioFitter.aspectMode = _masterAspectRatio <= aspectRatio ? AspectRatioFitter.AspectMode.EnvelopeParent : AspectRatioFitter.AspectMode.FitInParent;
        Debug.Log($"currentWidth: {currentWidth}, currentHeight: {currentHeight}, aspectRatio: {aspectRatio}, masterAspectRatio: {masterAspectRatio}, ratio: {ratio}");
    }
}
