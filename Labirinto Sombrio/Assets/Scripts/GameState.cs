using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using Unity.Mathematics;

public class GameState : MonoBehaviour
{
    static public CanvasManager mainCanvas => CanvasManager.CanvasManagerInstance;

    public Transform playerTransform;
    public Transform playerMiddleT;
    static public Transform PlayerTransform => GameStateInstance.playerTransform;
    static public Transform PlayerMiddleT => GameStateInstance.playerMiddleT;
    public bool isPlayerDead = false;
    static public bool IsPlayerDead {
        get => GameStateInstance.isPlayerDead;
        set => GameStateInstance.isPlayerDead = value;
    }
    public static bool isGamePaused {get; set;} = false;
    private MovimentoMouse movimentoMouse;
    static public MovimentoMouse MovimentoMouse { get => gameState.movimentoMouse; }
    private Movimento movimento;

    private Camera mainCamera;
    static public Camera MainCamera { get => gameState.mainCamera; }
    private Camera cutsceneCamera;
    [SerializeField] private GameObject GenericAudioSourcePrefab;
    private static GameState gameState;
    private GameState() { }
    public static GameState GameStateInstance => gameState;

    public SaveData saveData;
    public static SaveData SaveData { get => gameState.saveData; set => gameState.saveData = value; }
    public static SaveManager saveManager = new SaveManager();
    public SettingsData settingsData;
    public static SettingsData SettingsData { get => gameState.settingsData; set => gameState.settingsData = value; }
    public static SettingsDataManager settingsManager = new SettingsDataManager();

    public static AudioMixer AudioMixer => gameState.audioMixer;
    [SerializeField] private AudioMixer audioMixer;

    public static Action OnSettingsUpdated;
    public static Action OnCutsceneEnd;
    
    //[SerializeField] private string nextScene = "MenuInicial";
    [SerializeField] private GameObject endCanvas;
    [HideInInspector] public static int nEnemies = 0;
    [HideInInspector] public static int nKillEnemies = 0;
    private DateTime levelStartTime = DateTime.Now;
    [HideInInspector] public static List<string> allPdasOnLevel = new List<string>();
    [HideInInspector] public static List<string> allPdasfound = new List<string>();

    protected TorchLight torchLight;
    public static bool IsTorchLit => gameState.torchLight.IsLit;
    public static float SpeedPorcent => 
    math.min(gameState.movimento.CurSpeed, gameState.movimento.CrouchSpeed / 2) / gameState.movimento.WalkSpeed;

    private void Awake()
    {
        torchLight = GetComponentInChildren<TorchLight>(true);
        movimento = GetComponentInChildren<Movimento>();
        mainCamera = Camera.main;
        movimentoMouse = GetComponentInChildren<MovimentoMouse>();
        //mainCanvas = gameObject.GetComponentInChildren<CanvasManager>();
        gameState = this;
        SaveData = saveManager.LoadGame();

        saveData.jumpCutscene = false;
        UpdateQuality();
        OnSettingsUpdated += ReloadSettings;
        OnSettingsUpdated?.Invoke();
        saveManager.SaveGame(saveData);
        PauseGame(false);
        nEnemies = 0;
        nKillEnemies = 0;
        allPdasfound = new List<string>();
        allPdasOnLevel = new List<string>();
        levelStartTime = DateTime.Now;
        Application.targetFrameRate = GameState.SettingsData.FPS;
        Debug.Log("Setting FPS to " + GameState.SettingsData.FPS);
    }

    static public void SaveGameData() => saveManager.SaveGame(SaveData);

    static public void ReloadSettings()
    {
        Debug.Log("RELOADING SETTINGS");
        SettingsData = settingsManager.LoadSettings();
        Application.targetFrameRate = GameState.SettingsData.FPS;
    }
    private void OnDestroy() 
    {
        OnSettingsUpdated -= ReloadSettings;
    }

    private void Start() 
    {
        var music = settingsData.mute ? 0 : SettingsData.musicVolume;
        var sfx = settingsData.mute ? 0 : SettingsData.sfxVolume;
        AudioMixer.SetFloat("music", Mathf.Log10(music + 0.0001f) * 20);
        AudioMixer.SetFloat("sfx", Mathf.Log10(sfx + 0.0001f) * 20);
    }

    private void Update() 
    {
        if(Input.GetButtonDown("Pause")) PauseGame(!isGamePaused);
    }

    public static void RestartStage()
    {
        SaveData.jumpCutscene = false;
        saveManager.SaveGame(SaveData);
        ReloadScene(0f, false);
    }

    public static string GetSceneName() => SceneManager.GetActiveScene().name;

    public static void ReloadScene(float waitTime, bool jumpCutscene = true)
    {
        var ob = GameStateInstance;
        var sceneName = ob.gameObject.scene.name;
        SaveData.jumpCutscene = jumpCutscene;
        saveManager.SaveGame(SaveData);
        ob.StartCoroutine(ob.LoadSceneCourotine(waitTime, sceneName));
    }

    public static void LoadScene(string sceneName, float waitTime = 0)
    {
        var ob = GameStateInstance;
        ob.StartCoroutine(ob.LoadSceneCourotine(waitTime, sceneName));
    }

    public static void UpdateQuality()
    {
        if(QualitySettings.GetQualityLevel() != (int)SettingsData.quality)
        {
            QualitySettings.SetQualityLevel((int)SettingsData.quality);
        }
    }

    IEnumerator LoadSceneCourotine(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(sceneName);
    }

    public static void InstantiateSound(Sound sound, Vector3 position, float destroyTime = 10f)
    {
        var AudioObject = GameObject.Instantiate(gameState.GenericAudioSourcePrefab, position, Quaternion.identity);
        var audioSource = AudioObject.GetComponent<AudioSource>();
        sound.PlayOn(audioSource);
        Destroy(AudioObject, destroyTime);
    }

    public static void PauseGame(bool pause)
    {
        /* if(mainCanvas == null)
        {
            Debug.Log("On main menu Pause Action");
            var mainMenu = FindFirstObjectByType<MenuController>(FindObjectsInactive.Include);
            if(mainMenu.settings.gameObject.activeInHierarchy) mainMenu.settings.Close();
            else if(mainMenu.instructions.activeInHierarchy) mainMenu.instructions.GetComponentInChildren<Button>().onClick.Invoke();
            else if(mainMenu.creditos.activeInHierarchy) mainMenu.creditos.GetComponentInChildren<Button>().onClick.Invoke();
            else if(mainMenu.backJogar.gameObject.activeInHierarchy) mainMenu.backJogar.onClick.Invoke();
            else if(mainMenu.backSelecionarFase.gameObject.activeInHierarchy) mainMenu.backSelecionarFase.onClick.Invoke();
            Cursor.lockState = CursorLockMode.None;
            isGamePaused = false;
            return;
        } */

        if(isGamePaused && mainCanvas != null && !mainCanvas.DoesExitPause())
        {
            Debug.Log("should exit pause? " + mainCanvas.DoesExitPause());
            mainCanvas.SetSettingsMenu(false);
            mainCanvas.SetTutorial(false);
            mainCanvas.SetPauseMenu(true);
            Cursor.lockState = isGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
            return;
        }
        isGamePaused = pause;
        Cursor.lockState = isGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pause ? 0f : 1f;
        mainCanvas?.SetPauseMenu(pause);
    }

    // public static void EndLevel()
    // {
    //     gameState.StartCoroutine(gameState.EndLevelCourotine());
    // }
    // IEnumerator EndLevelCourotine()
    // {
    //     var totalTime = DateTime.Now - levelStartTime;
    //     var go = GameObject.Instantiate(endCanvas,null);
    //     go.SetActive(true);

    //     yield return new WaitForSecondsRealtime(10f);
    //     var nextSceneI = this.gameObject.scene.buildIndex + 1;
    //     if (nextSceneI >= SceneManager.sceneCountInBuildSettings) nextSceneI = 0;


    //     string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneI);
    //     string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

    //     GameState.LoadScene(sceneName);
    // }
}
