using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;

    public Image attackCooldownImageFiller;
    public Image healthImageFiller;

    public GameObject loadingScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadingScreenOn()
    {
        loadingScreen.SetActive(true);
        InventoryUI.Instance.loadingLevel = true;

        StartCoroutine(LoadingScreenOff());
    }

    IEnumerator LoadingScreenOff()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        loadingScreen.SetActive(false);
        InventoryUI.Instance.loadingLevel = false;
    }
}
