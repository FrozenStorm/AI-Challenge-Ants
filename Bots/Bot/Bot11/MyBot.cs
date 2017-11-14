using System;
using System.Collections.Generic;
using System.Linq;



namespace Ants 
{
	class MyBot : Bot 
    {
        Random myRandom = new Random();
        List<Direction> directionList = new List<Direction>();
        
        // DoTurn is run once per turn
		public override void DoTurn (IGameState state) 
        {
            state.NotMovedAnts = state.MyAnts.Count;
            int end;
            // Defend ToDo

            // Food
            end = state.FoodTargets.Count;
            for (int index = 0; index < end; index++)
            {
                state.FoodTargets[index].Prio = state.GetDistancePriority(new Location(state.FoodTargets[index].Row, state.FoodTargets[index].Col));
            }
            state.FoodTargets.Sort(Target.ComparePriority);
            for (int index = 0; index < end && state.TimeRemaining > 50 && state.NotMovedAnts > 0; index++)
            {
                Pathfinding.SendAntToTarget(state, 200, state.FoodTargets[index]);//original 100
            }
            
            // Attack EnemyAnt
            end = state.EnemyAntTargets.Count;
            for (int index = 0; index < end; index++)
            {
                state.EnemyAntTargets[index].Prio = state.GetDistancePriority(new Location(state.EnemyAntTargets[index].Row, state.EnemyAntTargets[index].Col));
            }
            state.EnemyAntTargets.Sort(Target.ComparePriority);
            for (int index = 0; index < end && state.TimeRemaining > 50 && state.NotMovedAnts > 0; index++)
            {
                //ToDo nur dann bewegen wenn beide bewegt werden können
                Pathfinding.Send2AntToTarget(state, 100, state.EnemyAntTargets[index]);//original 100
            }

            // Attack EnemyHill
            end = state.EnemyHillTargets.Count;
            for (int index = 0; index < end; index++)
            {
                state.EnemyHillTargets[index].Prio = state.GetDistancePriority(new Location(state.EnemyHillTargets[index].Row, state.EnemyHillTargets[index].Col));
            }
            state.EnemyHillTargets.Sort(Target.ComparePriority);
            for (int index = 0; index < end && state.TimeRemaining > 50 && state.NotMovedAnts > 0; index++)
            {
                Pathfinding.SendAntToTarget(state, 200, state.EnemyHillTargets[index]);//original 300
            }
            
            // Goto Attackborder
            state.AttackBorder = new List<Target>(state.AttackBorder.Distinct<Target>()); // Entfernen aller doppelter Einträge
            for (int index = 0; index < state.AttackBorder.Count; index++)// Kreisüberschneidungen und Ziele auf Wasser löschen
            {
                if ((state.map[state.AttackBorder[index].Row, state.AttackBorder[index].Col, (int)Layer.Danger] > (int)DangerGrade.NoDanger) && (state.map[state.AttackBorder[index].Row, state.AttackBorder[index].Col, (int)Layer.Tiles] == (int)Tile.Water))
                {
                    state.AttackBorder.RemoveAt(index); 
                    index--;
                }
            }
            end = state.AttackBorder.Count;
            for (int index = 0; index < end; index++)
            {
                state.AttackBorder[index].Prio = state.GetDistancePriority(new Location(state.AttackBorder[index].Row, state.AttackBorder[index].Col));
            }
            state.AttackBorder.Sort(Target.ComparePriority);
            for (int index = 0; index < end && state.TimeRemaining > 50 && state.NotMovedAnts > 0; index++)
            {
                Pathfinding.SendAntToTarget(state, 300, state.AttackBorder[index]);//original 200
            }
            
            // Explore
            foreach (Ant MyAnt in state.MyAnts.FindAll(ant => ant.Moved == false))
            {
                Pathfinding.Explore(state, MyAnt, 200);
            }
            // Random Move
            /*
            foreach (Ant MyAnt in state.MyAnts.FindAll(ant => ant.Moved == false))
            {
                Pathfinding.RandomMove(state, MyAnt);
            }*/

            #if DEBUG
            state.SaveState(false);
            #endif
		}

		public static void Main (string[] args)
        {
            #if DEBUG
                System.Diagnostics.Debugger.Launch();
                while (!System.Diagnostics.Debugger.IsAttached)
                {}
            #endif
            new Ants().PlayGame(new MyBot());
		}

	}
	
}