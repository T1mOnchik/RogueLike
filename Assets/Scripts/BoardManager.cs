using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random; 
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count{
        public int max;
        public int min;

        public Count(int min, int max){
            this.min = min;
            this.max = max;
        }
    }

    public int columns = 16;
    public int rows = 8;
    public Count wallCount = new Count(5,10);
    public Count foodCount = new Count(1,5);
    public GameObject exit;
    public GameObject outterWall;
    //public GameObject player;
    public GameObject door;
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    private Transform boardHolder;
    private GameObject board;
    private List<Vector3> gridPositions = new List<Vector3>();
    //private Vector3 playerPosition;

    void boardSetup(){
        board = new GameObject("Board");
        //boardHolder = board.transform;
        for(int x=-1; x < columns + 1; x++){
            for(int y=-1; y < rows+1; y++){
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                Quaternion quaternion = Quaternion.identity;
                MapManager mapManager = MapManager.instance;
                if(x==-1|| x == columns || y==-1 || y==rows){
                    if(x>=7 && x<=8){ //top and bottom door
                        if(mapManager.GetPlayerCoordinates().x == 0 && y == rows){
                            toInstantiate = outterWall;
                        }
                        else if(mapManager.GetPlayerCoordinates().x == mapManager.maxMapScale-1 && y == -1){
                            toInstantiate = outterWall;
                        }
                        else{
                            toInstantiate = door;
                        }
                        
                    }
                    else if( y>=3 && y<=4 && x==columns && mapManager.GetPlayerCoordinates().y != mapManager.maxMapScale-1){ //right door
                        toInstantiate = door;
                        quaternion = Quaternion.Euler(0,0,-90);
                    }
                    else if(y>=3 && y<=4 && x==-1 && mapManager.GetPlayerCoordinates().y != 0){ //left door
                        toInstantiate = door;
                        quaternion = Quaternion.Euler(0,0,90);
                    }
                    else{
                        toInstantiate = outterWall;
                    }
                }
                
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), quaternion) as GameObject;
                instance.transform.SetParent(board.transform);
            }
        }
        
    }

    void InitializeList(Vector3 playerPosition){
        gridPositions.Clear();
        for(int x = 1; x < columns - 1; x++){
            for(int y = 1; y < rows - 1; y++){
                if(x!=playerPosition.x && y!=playerPosition.y) 
                    gridPositions.Add(new Vector3(x,y,0));
            }
        }
    }

    Vector3 randomPosition(){
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randPos = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randPos;
    }

    void spawnAtRandomPositions(int min, int max, GameObject[] tileArray){
        int objectCount = Random.Range(min, max+1);
        for(int i = 0; i < objectCount; i++){
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)] ;
            Instantiate(tileChoice, randomPosition(), Quaternion.identity).transform.SetParent(board.transform);
            //instance.transform.SetParent(boardHolder);
        }
    }

    public GameObject setupScene(int level, Vector3 spawnPlayerPosition){
        //playerPosition = spawnPlayerPosition;
        boardSetup();
        InitializeList(spawnPlayerPosition);
        spawnAtRandomPositions(wallCount.min, wallCount.max, wallTiles);
        spawnAtRandomPositions(foodCount.min, foodCount.max, foodTiles);
        int enemyCount = (int)Mathf.Log(level, 2f);
        spawnAtRandomPositions(enemyCount, enemyCount, enemyTiles);
        //Instantiate(exit, new Vector3(columns-1, rows-1), Quaternion.identity);
        //Instantiate(player, playerPosition, Quaternion.identity); 
        return board;
    }
}

// x>0 && y>0 && x>y  

// x>0 && y>0 && y>x 

// x<0 && y>0


// x>0 && y<0