using System;
using System.Collections.Generic;



namespace Ants 
{

	class MyBot : Bot 
    {
        Random myRandom = new Random();
        List<Direction> directionList = new List<Direction>();
		
        
        // DoTurn is run once per turn
		public override void DoTurn (IGameState state) 
        {
            int countant = state.MyAnts.Count-1;
            for (int ant = 0; ant <= countant; ant++)
            {
                if (NormalMove(state, ant)) { }
                else if (Explore(state, ant, 200)) { }
                else RandomMove(state, ant);
                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 10) break;
            }
			
		}

        private bool NormalMove(IGameState state, int ant)
        {
            Direction direction = new Direction();
            direction = Direction.Noone;


            if (direction == Direction.Noone && state.FoodTiles.Count > 0) direction = DirectionToTarget(state, 300, ant, (int)Tile.Food, (int)Layer.Tiles);
            if (direction == Direction.Noone && state.EnemyHills.Count > 0) direction = DirectionToTarget(state, 400, ant, (int)Tile.EnemyHill, (int)Layer.Tiles);
            if (direction == Direction.Noone && state.EnemyAnts.Count > 0) direction = DirectionToTarget(state, 200, ant, (int)Tile.EnemyAnt, (int)Layer.Tiles);

            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(state.MyAnts[ant], direction);
                SendOrder(state, state.MyAnts[ant], direction, newLoc);
                return true;
            }
            else return false;
        }

        private Direction DirectionToTarget(IGameState state,int radius, int ant, int Target ,int TargetLayer)
        {
            int x;
            int y;
            int z = 0;
            bool foundway = false;
            int lastindex = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, state.MyAnts[ant].Row, state.MyAnts[ant].Col));
            Direction direction= new Direction();
            direction = Direction.Noone;
            Location startlocation = new Location(state.MyAnts[ant].Row,state.MyAnts[ant].Col);
            try
            {
                while (foundway == false)
                {
                    x = myKnoten[z].Now_x;
                    y = myKnoten[z].Now_y;
                    if (z > radius) return Direction.Noone;
                    if (state.map[x, y, TargetLayer] == Target)
                    {
                        foundway = true;
                    }
                    else
                    {
                        try
                        {
                            if ((state.map[test_x(state,x+1), y, TargetLayer] != (int)Tile.Water) && (state.map[test_x(state,x+1), y, TargetLayer] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, test_x(state,x+1), y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, test_x(state,x+1), y));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[test_x(state,x-1), y, TargetLayer] != (int)Tile.Water) && (state.map[test_x(state,x-1), y, TargetLayer] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, test_x(state,x-1), y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, test_x(state,x-1), y));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, test_y(state,y+1), TargetLayer] != (int)Tile.Water) && (state.map[x, test_y(state,y+1), TargetLayer] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, test_y(state,y+1), lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, test_y(state,y+1)));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, test_y(state,y-1), TargetLayer] != (int)Tile.Water) && (state.map[x, test_y(state,y-1), TargetLayer] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, test_y(state,y-1), lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, test_y(state,y-1)));
                                lastindex++;
                            }
                        }
                        catch { }
                        if (z < lastindex) z++;
                        else return Direction.Noone;
                    }
                }
            }
            catch { }

            while (z > 0)
            {
                direction = state.GetDirections(startlocation, new Location(myKnoten[z].Now_x, myKnoten[z].Now_y));
                z = myKnoten[z].From_z;
            }
            return direction;
        }

        private bool knotenschonvorhanden(List<Knoten> myKnoten, int x, int y, int lastindex)
        {
            for (; lastindex >= 0; lastindex--)
            {
                if ((myKnoten[lastindex].Now_x == x) && (myKnoten[lastindex].Now_y == y))
                {
                    return true;
                }
            }
            return false;
        }

        private bool Explore(IGameState state,int ant, int radius)
        {
            int x;
            int y;
            int z = 0;
            int lastindex = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, state.MyAnts[ant].Row, state.MyAnts[ant].Col));
            Direction direction = new Direction();
            direction = Direction.Noone;
            Location startlocation = new Location(state.MyAnts[ant].Row, state.MyAnts[ant].Col);
            try
            {
                while (z < radius)
                {
                    x = myKnoten[z].Now_x;
                    y = myKnoten[z].Now_y;
                    try
                    {
                        if ((state.map[test_x(state,x+1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[test_x(state,x+1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, test_x(state,x+1), y, lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, test_x(state,x+1), y));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[test_x(state,x-1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[test_x(state,x-1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, test_x(state,x-1), y, lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, test_x(state,x-1), y));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[x, test_y(state,y+1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, test_y(state,y+1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, test_y(state,y+1), lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, x, test_y(state,y+1)));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[x, test_y(state,y-1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, test_y(state,y-1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, test_y(state,y-1), lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, x, test_y(state,y-1)));
                            lastindex++;
                        }
                    }
                    catch { }
                    if (z < lastindex) z++;
                    else break;
                }
            }
            catch { }

            int minimum=2000;
            Knoten minimumk=myKnoten[0];
            foreach (Knoten k in myKnoten)
            {
                if (state.map[k.Now_x, k.Now_y, (int)Layer.Steps] < minimum)
                {
                    minimum = state.map[k.Now_x, k.Now_y, (int)Layer.Steps];
                    minimumk = k;
                }
            }
            z = myKnoten.IndexOf(minimumk);

            while (z > 0)
            {
                direction = state.GetDirections(startlocation, new Location(myKnoten[z].Now_x, myKnoten[z].Now_y));
                z = myKnoten[z].From_z;
            }

            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(state.MyAnts[ant], direction);
                if (state.GetIsUnoccupied(newLoc))
                {
                    SendOrder(state, state.MyAnts[ant], direction, newLoc);
                    return true;
                }
            }
            return false;
        }

        private void RandomMove(IGameState state, int ant)
        {
            directionList.Clear();
            Direction direction = new Direction();

            directionList.Add(Direction.West);
            directionList.Add(Direction.North);
            directionList.Add(Direction.South);
            directionList.Add(Direction.East);

            for (int directionsleft = 4; directionsleft > 0; directionsleft--)
            {
                direction = directionList[myRandom.Next(directionsleft)];
                Location newLoc = state.GetDestination(state.MyAnts[ant], direction);
                if (state.GetIsUnoccupied(newLoc))
                {
                    SendOrder(state, state.MyAnts[ant], direction, newLoc);
                    break;
                }
                else directionList.Remove(direction);
            }
        }

        private void SendOrder(IGameState state, Ant ant, Direction direction, Location newLoc)
        {
            state.MoveAnt(ant, newLoc);
            IssueOrder(ant, direction);
        }
        private int test_x(IGameState state,int x)
        {
            if (x > state.Height-1) x = 0;
            if (x < 0) x = state.Height - 1;
            return x;
        }
        private int test_y(IGameState state, int y)
        {
            if (y > state.Width-1) y = 0;
            if (y < 0) y = state.Width - 1;
            return y;
        }


		public static void Main (string[] args) 
        {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}