using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] private GameObject newRunButton;
    [SerializeField] private GameObject seedInput;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject deleteRunButton;

    private void Start()
    {
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
}
