using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class CanvasManager : MonoBehaviour
{
    private static CanvasManager canvasManager;
    private CanvasManager() { }
    public static CanvasManager CanvasManagerInstance => canvasManager;

    [SerializeField] private RectTransform canvasHolder;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject tutorial;
    public GameObject damageVFX;
    public GameObject healthRecoverVFX;
    [SerializeField] private GameObject gameoverAnimUI;
    public GameObject GameOverUI => gameoverAnimUI;

    public bool DoesExitPause()
    {
        return !(settingsMenu.activeSelf || tutorial.activeSelf);
    }
    
    private void Awake() 
    {
        canvasManager = this;
    }

    void Start()
    {
        SetPauseMenu(false);
        SetSettingsMenu(false);
        SetTutorial(false);
    }
    
    IEnumerator RebuildingLayout(RectTransform rectT)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        for (int i = 0; i < 40; i++)
        {
            yield return new WaitForSeconds(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectT);
        }
    }

    public void InstantiateVFX(GameObject vfx)
    {
        Destroy(Instantiate(vfx, canvasHolder.position, transform.rotation, transform), 5f);
    }

    public void ReturnToMainPause()
    {
        SetPauseMenu(true);
        SetSettingsMenu(false);
        SetTutorial(false);
    }

    public void SetPauseMenu(bool value) 
    {
        pauseMenu.SetActive(value);
    }
    public void SetSettingsMenu(bool value) 
    {
        settingsMenu.SetActive(value);
    }
    public void SetTutorial(bool value) 
    {
        tutorial.SetActive(value);
    }

    public void ResumeGame() => GameState.PauseGame(false);
    public void QuitGame() => GameState.LoadScene("Menu");
}
