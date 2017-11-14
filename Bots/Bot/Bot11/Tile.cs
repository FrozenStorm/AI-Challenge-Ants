using System;
namespace Ants 
{
    public enum Tile { MyAnt = -1, EnemyAnt = -2, Dead = -3, Land = -4, Food = -5, Water = -6, Unseen = -7, MyHill = -8, EnemyHill = -9 }
    public enum Layer { Tiles = 0, Danger = 1, Steps = 2}
    public enum DangerGrade { Friend = -1, NoDanger = 1, Enemy = 2 }
}

