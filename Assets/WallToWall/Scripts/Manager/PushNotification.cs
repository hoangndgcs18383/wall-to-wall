using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Messaging;
using UnityEngine;

public class PushNotification : Singleton<PushNotification>
{
    private void Start()
    {
        Firebase.FirebaseApp.CheckDependenciesAsync().ContinueWith(task =>
        {
            if(task.Result == Firebase.DependencyStatus.Available)
            {
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        Debug.Log("Received a new token: " + e.Token);
    }
}
