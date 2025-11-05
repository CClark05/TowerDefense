using System;

public interface IOnCardSelfDestruct
{
    public void Apply(TowerWaveData towerWaveData);
    public void OnComplete();
}