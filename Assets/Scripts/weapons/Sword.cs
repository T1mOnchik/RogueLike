using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    public override GameObject PickUp(Transform player)
    {
        return base.PickUp(player);
    }

    public override int Attack(Enemy attackedEnemy)
    {
        return base.Attack(attackedEnemy);
    }

    public override void Drop()
    {
        base.Drop();
    }

    public override bool BreakDown()
    {
        return base.BreakDown();
    }
}
