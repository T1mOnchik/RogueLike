using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;
    public GameObject mapManager;
    
    // Start is called before the first frame update
    void Start()
    {       
        if(GameManager.instance == null){
            Instantiate(gameManager);
        }
        if(MapManager.instance == null){
            Instantiate(mapManager);
        }
    }
}
