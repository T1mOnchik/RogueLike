﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{

    public int damage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    private Animator animator;
    private Transform target;
    private bool skipMove;

    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir){
        if(skipMove){
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = false;  //поменять на true чтоб противник ходили через 1 ход
    }

    public void MoveEnemy(){  // Тут мы начинаем прописывать как противник должен к нам идти
        int xDir = 0;
        int yDir = 0;
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon){
            yDir = target.position.y > transform.position.y ? 1 : -1; // short if (condition ? consequent : alternative)
        }
        else{
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);

        
    }

    protected override void onCantMove<T>(T component){
        Player hitPlayer = component as Player;
        animator.SetTrigger("ifPlayerHere");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        hitPlayer.LoseFood(damage); 
    }
}
