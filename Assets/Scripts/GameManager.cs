using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //Instances
    public static GameManager instance = null;
    public BoardManager boardScript;
    public GameObject player;
    
    //Game`s state vars
    [HideInInspector] public bool isPlayerTurn = true;
    public float delayTurn = .1f; 
    public float levelStartDelay = 0.5f;     
    private bool enemiesMoving;
    private bool doingSetup;
    
    //Player`s stats
    [HideInInspector] public Vector3 spawnPosition;
    public int playerFoodPoints = 100;
    public int playerWeapon = 0;
    private int level = 1;
    
    //UI
    public GameObject grabItemTxt;
    private Button StartGameButton;
    private GameObject restartButton;
    private GameObject levelImage;
    private Text levelTxt;
    public List<Enemy> enemies;
    
    
    
    private void Start(){
        if(instance == null){
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // Это шоб когда мы на новый левел переходили не уничтожался этот объект
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        spawnPosition = boardScript.GetDefaultPlayerPosition(); //типа середина комнаты, чисто для примера поставил
        StartGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        StartGameButton.onClick.AddListener(InitGame);
        
    }

    private void Update() {
        if(isPlayerTurn || enemiesMoving || doingSetup){
            return;
        }    
        StartCoroutine(MoveEnemies());
    }

    private void InitGame(){
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        grabItemTxt = GameObject.Find("GrabItemHint");
        restartButton = GameObject.Find("RestartButton");
        restartButton.GetComponent<Button>().onClick.AddListener(RestartGame);
        
        HideHomeScreen();

        levelTxt.text = "Day " + level;
        levelImage.SetActive(true);
        if(GameObject.Find("Board") != null){
            GameObject.Find("Board").SetActive(false);
        }
        
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        MapManager mapManager = MapManager.instance; 
        if(mapManager.GetRoomFromMap() < 0){  
            boardScript.setupScene(level, spawnPosition);      
        }
        else{
            mapManager.GetRoomFromList().SetActive(true);
        }
        if (FindObjectOfType(typeof(Player)) == null)
        {
            DontDestroyOnLoad(Instantiate(player, spawnPosition, Quaternion.identity)); 
        } 
        else{
            player = GameObject.Find("Player(Clone)");
            player.transform.position = spawnPosition;
            player.GetComponent<Player>().enabled=true;
        }
    }

    public void SaveRoomBeforeExit(){
        GameObject roomToList = GameObject.Find("Board");             
        DontDestroyOnLoad(roomToList);
        MapManager.instance.SaveRoomInList(roomToList);
    }

    private void OnLevelWasLoaded(int index){
        level++;
        InitGame();
    }

    private void HideLevelImage(){
        levelImage.SetActive(false);
        doingSetup = false;
    }

    private void HideHomeScreen(){
        GameObject.Find("HomeScreen").SetActive(false);
        restartButton.SetActive(false);
        grabItemTxt.SetActive(false);
    }

    

    public void AddEnemyToList(Enemy script){
        enemies.Add(script);

    }

    public void GameOver(){
        levelTxt.text = "After " + level + " days, you died.";
        levelImage.SetActive(true);
        restartButton.SetActive(true);
        enabled = false;
    }

    private void RestartGame(){
        Destroy(player);
        Destroy(gameObject);
        MapManager.instance.ClearMap();
        Destroy(GameObject.Find("MapManager"));
        SceneManager.LoadScene("Main");
        
    }

    private IEnumerator MoveEnemies(){ 
        enemiesMoving = true;
        yield return new WaitForSeconds(delayTurn);
        if(enemies.Count==0){
            yield return new WaitForSeconds(delayTurn);
        }
        for(int i = 0; i < enemies.Count; i++){
            if(enemies[i].MoveEnemy()){
                yield return new WaitForSeconds(enemies[i].speed);
            }
        }
        isPlayerTurn = true;
        enemiesMoving = false;
    }
}

