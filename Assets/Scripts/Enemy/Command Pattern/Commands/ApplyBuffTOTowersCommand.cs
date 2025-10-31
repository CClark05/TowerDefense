using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyBuffToTowersCommand : EnemyCommand
{
    private BuffData stunDebuff;
    private int stacks;
    private List<TowerDataHolder> towers;
    public ApplyBuffToTowersCommand(BuffData stunDebuff, int stacks, List<TowerDataHolder> towers)
    {
        this.stunDebuff = stunDebuff;
        this.stacks = stacks;
        this.towers = towers;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        while (towers.Count == 0) yield return null;
        foreach (var tower in towers)
        {
            tower.GetComponent<IBuffOverride>().AddBuff(stunDebuff as IBuff, stacks);
        }
        yield return new WaitForSeconds(stacks);
        foreach (var tower in towers)
        {
            tower.GetComponent<IBuffOverride>().RemoveBuff(stunDebuff as IBuff, stacks);
        }
    }
}