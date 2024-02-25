using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyScreen : Singleton<NotifyScreen>
{
    [SerializeField] private NotifyBar notifyBarPrefab;
    
    public void ShowNotify(string message)
    {
        notifyBarPrefab.Show(message);
    }
}