using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public BoardManager boardScript;
    public GameObject player;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool isPlayerTurn = true;
    public float delayTurn = .1f; 
    public float levelStartDelay = 0.5f;     
    [HideInInspector] public Vector3 spawnPosition;
    private bool isGameStarted = false;
    private Button StartGameButton;
    private GameObject restartButton;
    private int level = 1;
    private GameObject levelImage;
    private Text levelTxt;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    
     
    private void InitGame(){
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        restartButton = GameObject.Find("RestartButton");
        restartButton.GetComponent<Button>().onClick.AddListener(RestartGame);
        restartButton.SetActive(false);
        //Debug.Log("isgamestrted = " + isGameStarted);
        // if(!isGameStarted){
            HideHomeScreen();
        //}
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
        isGameStarted = true;
    }

    private void Update() {
        if(isPlayerTurn || enemiesMoving || doingSetup){
            return;
        }    
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script){
        enemies.Add(script);

    }

    public void GameOver(){
        levelTxt.text = "After " + level + " days, you died.";
        levelImage.SetActive(true);
        restartButton.SetActive(true);
        isGameStarted = false;
        enabled = false;
    }

    private void RestartGame(){
        Destroy(player);
        Destroy(gameObject);
        MapManager.instance.ClearMap();
        Destroy(GameObject.Find("MapManager"));
        SceneManager.LoadScene("Main");
        
    }

    IEnumerator MoveEnemies(){ 
        enemiesMoving = true;
        yield return new WaitForSeconds(delayTurn);
        if(enemies.Count==0){
            yield return new WaitForSeconds(delayTurn);
        }
        for(int i = 0; i < enemies.Count; i++){
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].speed);
        }
        isPlayerTurn = true;
        enemiesMoving = false;
    }
}

