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
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public GameObject PickUp(Transform player){
        transform.SetParent(player);
        transform.position = player.position;
        boxCollider2D.enabled = false;
        return gameObject;
    }

    public int Attack(Enemy attackedEnemy){
        return attackedEnemy.hp -= damage;
    }

    public void Drop(){
        transform.SetParent(null);
        boxCollider2D.enabled = true;

    }
}
