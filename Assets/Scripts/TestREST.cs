// using System;
// using System.Collections.Generic;
// using System.Text;
// using Newtonsoft.Json;
// using Proyecto26;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.Serialization;
// using Zeff.Framework.Extensions;
//
// [Serializable]
// public struct User
// {
//     [JsonProperty("na")] public string na;
//     [JsonProperty("em")] public string em;
//     [JsonProperty("pa")] public string pa;
//     [JsonProperty("av")] public string av;
// }
//
// public class TestREST : MonoBehaviour
// {
//     public const string URL = "https://zeff-web-service.onrender.com/";
//     public const string LOGIN = "users/login";
//     public const string SIGHUP = "users/signup";
//     public const string GET_USER = "users/{id}";
//     public const string GET_ALL_USERS = "users";
//
//     public string username;
//     public string email;
//     public string password;
//
//     [FilePath(AbsolutePath = true, Extensions = "png,jpg,jpeg")]
//     public string userImage;
//     
//     private List<User> _users = new List<User>();
//
//     [Button]
//     public void GetAll()
//     {
//         RestClient.Get(EndpointConstants.GET_ALL_USERS)
//             .Then(response =>
//             {
//                 _users = JsonConvert.DeserializeObject<List<User>>(response.Text);
//                 Debug.Log(_users.ToJson());
//             });
//     }
//
//     [Button]
//     public void Login()
//     {
//         RestClient.Post(EndpointConstants.POST_LOGIN, new User
//         {
//             em = "hoangndgcs18384@gmail.com",
//             pa = "Jiyeon3012"
//         }, (error, res) =>
//         {
//             if (res.StatusCode == 200)
//                 Debug.Log(res.Text);
//
//             if (error != null)
//             {
//                 Debug.Log(error);
//             }
//         });
//     }
//
//     [Button]
//     public void CreateAccount()
//     {
//         RestClient.Post(EndpointConstants.POST_CREATE, new User
//         {
//             na = username,
//             em = email,
//             pa = password,
//             av = userImage
//         }, (error, res) =>
//         {
//             if (res.StatusCode == 200)
//                 Debug.Log(res.Text);
//
//             if (error != null)
//             {
//                 LogError(error);
//             }
//         });
//     }
//
//     [Button]
//     public void GetUserById(string id)
//     {
//         RestClient.Get(EndpointConstants.GET_USER_BY_ID.Replace("{id}", id))
//             .Then(response => { Debug.Log(response.Text); });
//     }
//
//     [Button]
//     public void UpdateUser(string id)
//     {
//         RestClient.Patch(EndpointConstants.GET_USER_BY_ID.Replace("{id}", id), new User
//         {
//             na = username,
//             em = email,
//             pa = password,
//             av = userImage
//         }, (error, res) =>
//         {
//             if (res.StatusCode == 200 || res.StatusCode == 201)
//                 Debug.Log(res.Text);
//
//             if (error != null)
//             {
//                 LogError(error);
//             }
//         });
//     }
//
//     [Button]
//     public void DeleteUser(string id)
//     {
//         RestClient.Delete(EndpointConstants.GET_USER_BY_ID.Replace("{id}", id))
//             .Then(response => { Debug.Log(response.Text); });
//     }
//
//     private void LogError(RequestException requestException)
//     {
//         StringBuilder sb = new StringBuilder();
//         sb.AppendFormat("StatusCode: {0} - ", requestException.StatusCode);
//         sb.AppendFormat("Response: {0} - Message: {1}", requestException.Response, requestException.Message);
//
//         Debug.Log(sb);
//     }
// }