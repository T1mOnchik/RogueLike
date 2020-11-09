using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
    public class Coords{
        public int x;
        public int y;

        public Coords(int x, int y){
            this.x = x;
            this.y = y;
        }
    }

public class MapManager : MonoBehaviour
{

    public static MapManager instance = null;
    public int maxMapScale; // Тут должна быть переменная хранящая максимальный размер карты
    // ПЕРЕДЕЛАТЬ В ПРИВАТ
    public ArrayList roomList; // Массив комнат
    public int[,] roomMap;  // Массив местаположений комнат
                                // Пример: Мы заходим в комнату, добавляем ее в roomList,
                                // а в roomMap на позицию например [5][6] записываем адрес этой комнаты в roomList
    private Coords coordinates; // СДЕЛАТЬ ВЫЧИСЛЕНИЯ СЕРЕДИНЫ МАТРИЦЫ
    

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        coordinates = new Coords((maxMapScale-1)/2,(maxMapScale-1)/2); // Начальные координаты = середина матрицы
        roomList = new ArrayList();
        roomMap = new int[maxMapScale, maxMapScale];
        for(int i=0; i<maxMapScale; i++) // Делаем по дефолту отрицательное значение 
        {                                // чтоб гейм менеджер не перепутал новую комнату с уже существующей. Так как подефолту в интовом массиве все значения нулии.
            for(int j=0; j<maxMapScale; j++)
                roomMap[i,j] = -1;
        }
        
    }

    public void SaveRoomInList(GameObject room){
        if(roomMap[coordinates.x, coordinates.y]==-1){
            roomList.Add(room);
            roomMap[coordinates.x, coordinates.y] = roomList.IndexOf(room);
            Debug.Log("Saved index: " + roomMap[coordinates.x, coordinates.y]);
        }
        else{
            roomList[GetRoomFromMap()] = room;
        }
    }

    public GameObject GetRoomFromList(){
        return roomList[GetRoomFromMap()] as GameObject;
    }

    public int GetRoomFromMap(){
        Debug.Log("Coordinates: "+GetPlayerCoordinates().x + ", "+ GetPlayerCoordinates().y );
        return roomMap[coordinates.x, coordinates.y];
    }

    public void ChangePlayerCoordinates(int shiftX, int shiftY){    //Тут мы будем менять координаты комнаты 
        coordinates.x += shiftX;                                    // в зависимости от того куда пошел игрок
        coordinates.y += shiftY;
    }
    public Coords GetPlayerCoordinates(){
        return coordinates;
    }

    public void ClearMap(){
        for (int i = 0; i < roomList.Count; i++)
        {
            
            Destroy(roomList[i] as GameObject);
        }
        
    }
}

// 0,0 0,1 0,2
// 1,0 1,1 1,2
// 2,0 2,1 2,2