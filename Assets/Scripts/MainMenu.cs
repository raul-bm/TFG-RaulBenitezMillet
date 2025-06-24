using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] private GameObject newRunButton;
    [SerializeField] private GameObject seedInput;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject deleteRunButton;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject controlsMenu;

    [SerializeField] private ShopMenu shopScript;

    private void Start()
    {
        ShowMainMenu();

        if(SaveSystem.IsThereAnySaveData())
        {
            newRunButton.SetActive(false);
            seedInput.SetActive(false);
            continueButton.SetActive(true);
            deleteRunButton.SetActive(true);
        }
        else
        {
            newRunButton.SetActive(true);
            seedInput.SetActive(true);
            continueButton.SetActive(false);
            deleteRunButton.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            PlayerPrefs.SetInt("moneyPlayer", 500);
        }
    }

    public void PlayButton()
    {
        if (inputField.text.Length < 8) PlayerPrefs.SetString("gameSeed", inputField.text + GenerateRandomSeed(8 - inputField.text.Length));
        else PlayerPrefs.SetString("gameSeed", inputField.text);

        PlayerPrefs.SetInt("gameSaved", 0);

        SceneManager.LoadScene(1);
    }

    public void ContinueButton()
    {
        PlayerPrefs.SetInt("gameSaved", 1);

        SceneManager.LoadScene(1);
    }

    public void DeleteButton()
    {
        SaveSystem.Delete();

        newRunButton.SetActive(true);
        seedInput.SetActive(true);
        continueButton.SetActive(false);
        deleteRunButton.SetActive(false);
    }

    private string GenerateRandomSeed(int lengthSeed)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] seed = new char[lengthSeed];
        for(int i = 0; i < lengthSeed; i++)
        {
            seed[i] = chars[Random.Range(0, chars.Length)];
        }

        return new string(seed);
    }

    public void ShowShopMenu()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(true);
        controlsMenu.SetActive(false);

        shopScript.InitializeShopMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        shopMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void ShowControlsMenu()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void DeleteAllButton()
    {
        PlayerPrefs.DeleteKey("moneyPlayer");
        PlayerPrefs.DeleteKey("unlockedItems");

        DeleteButton();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
