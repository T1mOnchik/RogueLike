using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{   
    
    public float speed = 0.1f;
    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody2D;
    private float inverseSpeed;

    // Using OnEnable instead of Start because it works every time when object become enabled(Starts or SetActive(true))
    protected virtual void OnEnable(){ // protected virtual can be overriden by their inheriting class
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        inverseSpeed = 1/speed; // мы храним время передвижения в -1 степени, чтобы потом умножать его, а не делить т к это эффективнее при вычисленни.

    }

    // В этой функции мы проверяем можем ли мы пройти в точку по заданным координатам, мы используем рейкаст чтоб прочекать что впереди нас. Во входных параметрах у нас есть out, это значит что этот параметр мы сможем использовать из вне после вызова функции.
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled=false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        if(hit.transform == null){
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end){ // Плавное передвижение
        
        // sqrRemainingDistance();  calculating remainDistance
        while(sqrRemainingDistance(end) > float.Epsilon){ // float.Epsilon = almost zero
            // inverseSpeed * Time.deltaTime it is remaining distance
            rigidBody2D = GetComponent<Rigidbody2D>();
            Vector3 newPosition = Vector3.MoveTowards(rigidBody2D.position, end, inverseSpeed * Time.deltaTime); // MoveTowards Calculates a position between the points specified by current and target, moving no farther than the distance specified by maxDistanceDelta.
            rigidBody2D.MovePosition(newPosition); 
            sqrRemainingDistance(end);
            yield return null;
        }
    }

    float sqrRemainingDistance(Vector3 end){
        return (transform.position - end).sqrMagnitude;   // sqrMagnitude returns the squared length of this vector  and cheaper than magnitude
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) //проверяем есть ли что то перед нами
        where T: Component                                     // если перед нами объект с blocking layer
    {                                                           // то в классе объекта описываем что происходит
        RaycastHit2D hit;                                       // если нет то двигаемся
        bool canMove = Move(xDir, yDir, out hit);
        if(hit.transform == null)
            return;
        
        T hitComponent = hit.transform.GetComponent<T>();
        if(!canMove && hitComponent != null){
            onCantMove(hitComponent);
        }
    }

    // T type is универсальный параметр любого типа
    protected abstract void onCantMove <T> (T component) // is going to be overridden by inhariting class
        where T: Component;

}
