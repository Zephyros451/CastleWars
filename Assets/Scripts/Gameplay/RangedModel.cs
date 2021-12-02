using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedModel : Model
{
    private float range;
    private float shootCooldown = 0.3f;

    public override void Init(UnitData unitData, Allegiance allegiance, int level, ITower target)
    {
        base.Init(unitData, allegiance, level, target);

        this.range = unitData.Range;

        Shoot();
    }

    private void Shoot()
    {
        DOTween.Sequence()
            .AppendInterval(shootCooldown)
            .AppendCallback(() =>
            {
                if (target.Allegiance != Allegiance
                && Vector3.Distance(transform.position, target.Transform.position) < range)
                {
                    target.ReceiveDamage(this.Attack);
                }
            })
            .SetLoops(-1);
    }
}
