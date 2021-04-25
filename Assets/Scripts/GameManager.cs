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
    private UImanager uiManager;
    
    //Game`s state vars
    [HideInInspector] public bool isPlayerTurn = true;
    [HideInInspector] public bool doingSetup;
    public const float delayTurn = .1f;
    // public float levelStartDelay = 0.5f;     //should be const
    private bool enemiesMoving;
    
    //Player`s stats
    public int playerFoodPoints = 100;
    public int playerWeapon = 0;
    [HideInInspector] public Vector3 spawnPosition; 
    [HideInInspector] public int level = 1;

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
    }

    private void Update() {
        if(isPlayerTurn || enemiesMoving || doingSetup){
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    private void OnLevelWasLoaded(int index){
        level++;
        InitGame();
    }

    public void InitGame(){
        doingSetup = true;
        uiManager = UImanager.instance;
        uiManager.InitGameUI();
        if(GameObject.Find("Board") != null){
            GameObject.Find("Board").SetActive(false);
        }
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
            DontDestroyOnLoad(player = Instantiate(player, spawnPosition, Quaternion.identity)); 
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

    public void AddEnemyToList(Enemy script){
        enemies.Add(script);

    }

    public void GameOver(){
        uiManager.GameOverUI();
        enabled = false;
    }

    public void RestartGame(){
        Destroy(player);
        Destroy(gameObject);
        MapManager.instance.ClearMap();
        Destroy(GameObject.Find("MapManager"));
        Destroy(uiManager);
        SceneManager.LoadScene("Main");
    }

    public void QuitGame(){
        Application.Quit();
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

