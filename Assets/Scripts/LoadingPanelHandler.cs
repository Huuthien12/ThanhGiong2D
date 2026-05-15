using System.Collections;
using UnityEngine;
using System;

public class LoadingPanelHandler : MonoBehaviour
{
    public void StartLoadingCoroutine(float loadingTime, Action onComplete)
    {
        StartCoroutine(LoadingCoroutine(loadingTime, onComplete));
    }

    IEnumerator LoadingCoroutine(float loadingTime, Action onComplete)
    {
        // Bật panel
        gameObject.SetActive(true);

        // Chờ
        yield return new WaitForSeconds(loadingTime);

        // Thực hiện teleport
        onComplete?.Invoke();

        // Tắt panel
        gameObject.SetActive(false);
    }
}