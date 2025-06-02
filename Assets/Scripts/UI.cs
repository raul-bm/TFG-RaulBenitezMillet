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
        yield return new WaitForSeconds(3);
        loadingScreen.SetActive(false);
        InventoryUI.Instance.loadingLevel = false;
    }
}
