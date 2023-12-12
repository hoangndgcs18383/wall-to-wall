using System;
using TMPro;
using UnityEngine;

public class TextVersion : MonoBehaviour
{
    private void Awake()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = $"Version: {Application.version}";
    }
}