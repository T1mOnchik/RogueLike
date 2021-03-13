using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;
    public GameObject mapManager;
    public GameObject uiManager;
    
    // Start is called before the first frame update
    void Start()
    {       
        if(GameManager.instance == null){
            Instantiate(gameManager);
        }
        // if(MapManager.instance == null){
        //     Instantiate(mapManager);
        // }
        if(UImanager.instance == null){
            StartCoroutine("waitForGM");
        }
    }

    IEnumerator waitForGM(){
        while(GameManager.instance == null){
            yield return new WaitForEndOfFrame();
        }
        Instantiate(uiManager);
        yield break;
    }
}
