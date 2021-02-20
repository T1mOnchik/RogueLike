using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnCollision
{
    private GameManager gameManager = GameManager.instance;

    public void doExit(Collider2D other, AudioClip openingDoor)
    {
        gameManager.SaveRoomBeforeExit();
        SoundManager.instance.PlaySingle(openingDoor);
        Vector3 exitPosition = other.transform.position;
        MapManager mapManager = MapManager.instance;
        if (exitPosition.x > 0 && exitPosition.y > 0 && exitPosition.x > exitPosition.y)
        {  // right door        
           gameManager.spawnPosition = new Vector3(0, exitPosition.y, 0);
            mapManager.ChangePlayerCoordinates(0, 1); //shift right y+=1
        }
        if (exitPosition.x > 0 && exitPosition.y > 0 && exitPosition.x <= exitPosition.y)
        { //Top door
            gameManager.spawnPosition = new Vector3(exitPosition.x, 0, 0);
            mapManager.ChangePlayerCoordinates(-1, 0); //shift top x += -1
        }
        if (exitPosition.x < 0 && exitPosition.y > 0)
        {  // left door
            gameManager.spawnPosition = new Vector3(15, exitPosition.y, 0);     // Тут мы записываем в спавнПлеерПозишн
            mapManager.ChangePlayerCoordinates(0, -1); //shift left y += -1
        }                                                                                // Мы берем позицию двери в которую вошли и записываем 
        if (exitPosition.x > 0 && exitPosition.y < 0)
        {  // bot door                           // противоположную точку на карте где прибавляем +1 к координате чтоб заспавниться не в двери
            gameManager.spawnPosition = new Vector3(exitPosition.x, 7, 0);      // НАДА ПЕРЕПИСАТЬ сделать переменные позиций дверей и перемещать игрока на противоположная дверь +1 к координате
            mapManager.ChangePlayerCoordinates(1, 0); //shift bottom x += 1
        }
    }

    // public void doFood(Collider2D other , AudioClip eatSound1, AudioClip eatSound2){
    //     foodPoints += pointsPerFood;
    //     SettingFoodText();
    //     SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
    //     other.gameObject.SetActive(false);
    // }

}
