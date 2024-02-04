using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;

public class ADsText : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;

    private CoroutineHandle _animateTextHandle;
    private Sequence _sequence;

    private void OnEnable()
    {
        _animateTextHandle = Timing.RunCoroutine(AnimateText());
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(_animateTextHandle);
    }

    private IEnumerator<float> AnimateText()
    {
        _sequence = DOTween.Sequence();
        while (_sequence != null && _sequence.IsActive())
        {
            DOTweenTMPAnimator animator = new DOTweenTMPAnimator(tmpText);

            for (int i = 0; i < tmpText.text.Length; i++)
            {
                tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0);
            }

            for (int i = 0; i < tmpText.text.Length; i++)
            {
                _sequence.Append(animator.DOFadeChar(i, 1, 0.1f));
            }

            yield return Timing.WaitForSeconds(_sequence.Duration());
            _sequence.Restart();
        }
    }
}