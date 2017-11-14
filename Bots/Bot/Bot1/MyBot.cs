using System;
using System.Collections.Generic;

namespace Ants 
{

	class MyBot : Bot 
    {
        
		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) 
        {

			// loop through all my ants and try to give them orders
			foreach (Ant ant in state.MyAnts) 
            {
				Location newLoc = new Pathfinding().pathfinding(state,ant,new Location(state.FoodTiles[0].Row,state.FoodTiles[0].Col);
                Direction direction = state.GetDirections(new Location(ant.Row,ant.Col),newLoc);
				IssueOrder(ant, direction);
				if (state.TimeRemaining < 10) break;
			}
			
		}
		
		
		public static void Main (string[] args) 
        {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}