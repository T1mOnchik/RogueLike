using UnityEngine;
using UnityEngine.UI;


public class UImanager : MonoBehaviour
{

    public static UImanager instance = null;
    private GameManager gameManager = GameManager.instance;
    public const float levelStartDelay = 0.5f;

    public GameObject grabItemTxt;
    private Button StartGameButton;
    private Button exitGameButton;
    private GameObject restartButton;
    private GameObject levelImage;
    private Text levelTxt;

    //Esc menu
    private GameObject escMenu;
    private Button menuExitToMainMenuButton;
    private Button menuResumeGameButton;

    

    // Start is called before the first frame update
    private void Start()
    {
        if(instance == null){
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        StartGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        exitGameButton = GameObject.Find("ExitGameButton").GetComponent<Button>();
        StartGameButton.onClick.AddListener(gameManager.InitGame);
        exitGameButton.onClick.AddListener(gameManager.QuitGame);
        

    }

    public void InitGameUI(){
        levelImage = GameObject.Find("LevelImage");
        levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        grabItemTxt = GameObject.Find("GrabItemHint");
        restartButton = GameObject.Find("RestartButton");
        restartButton.GetComponent<Button>().onClick.AddListener(gameManager.RestartGame);
        InitializeEscMenu();
        HideHomeScreen();
        levelTxt.text = "Day " + gameManager.level;
        levelImage.SetActive(true);

        Invoke("HideLevelImage", levelStartDelay);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(escMenu.activeSelf == false){
                escMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                escMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }    
    }

    private void InitializeEscMenu(){
        escMenu = GameObject.Find("EscMenu");
        menuResumeGameButton = escMenu.transform.Find("ResumeMenuButton").GetComponent<Button>();
        menuExitToMainMenuButton = GameObject.Find("ExitMenuButton").GetComponent<Button>();
        menuResumeGameButton.onClick.AddListener(delegate{escMenu.SetActive(false);});
        menuExitToMainMenuButton.onClick.AddListener(gameManager.RestartGame);
    }

    private void HideHomeScreen(){
        GameObject.Find("HomeScreen").SetActive(false);
        restartButton.SetActive(false);
        grabItemTxt.SetActive(false);
        escMenu.SetActive(false);
    }

    private void HideLevelImage(){
        levelImage.SetActive(false);
        gameManager.doingSetup = false;
    }

    public void GameOverUI(){
        levelTxt.text = "After " + gameManager.level + " days, you died.";
        levelImage.SetActive(true);
        restartButton.SetActive(true);
    }
}
