using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameScene : MonoBehaviour
{
    [Header("UIPause")]
    public GameObject pauseScreen;
    [SerializeField]private bool pauseScreenActive = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseScreenActive)
        {
            pauseScreen.SetActive(true);
            pauseScreenActive = !pauseScreenActive;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && pauseScreenActive)
        {
            pauseScreen.SetActive(false);
            pauseScreenActive = !pauseScreenActive;
        }
    }
    public void ButtonContinue()
    {
        if (pauseScreenActive)
        {
            pauseScreen.SetActive(false);
            pauseScreenActive = !pauseScreenActive;
        }
    }
}
