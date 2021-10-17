using System.Collections;
using Facebook.Unity;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        FB.Init();
        while (!FB.IsInitialized)
        {
            yield return null;
        }

        FB.ActivateApp();
        yield break;
    }
}