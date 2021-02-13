using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    
    public int damage;
    public int durability;
    public static Weapon instance = null;
    private BoxCollider2D boxCollider2D;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public virtual GameObject PickUp(Transform player){
        transform.SetParent(player);
        transform.position = player.position;
        boxCollider2D.enabled = false;
        return gameObject;
    }

    public virtual int Attack(Enemy attackedEnemy){
        attackedEnemy.hp -= damage;
        durability--;
        BreakDown();
        Debug.Log("enemy's hp: " + attackedEnemy.hp + " durability  " + durability);  
        return attackedEnemy.hp;
    }

    public virtual void Drop(){
        transform.SetParent(null);
        boxCollider2D.enabled = true;
    }

    public virtual bool BreakDown(){
        if(durability <= 0){
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
