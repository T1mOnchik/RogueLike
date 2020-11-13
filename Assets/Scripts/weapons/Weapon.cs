using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    public int damage;
    public int durability;
    public static Weapon instance = null;
    private BoxCollider2D boxCollider2D;

    // Start is called before the first frame update
    private void Start()
    {
        // if(instance == null){ 
        //     instance = this;
        // }
        //Debug.Log(instance);
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void PickUp(Transform player){
        transform.SetParent(player);
        transform.position = player.position;
        boxCollider2D.enabled = false;
    }

    public int Attack(int hp){
        hp -= damage;
        return hp;
    }

    public void Drop(){

    }
}
