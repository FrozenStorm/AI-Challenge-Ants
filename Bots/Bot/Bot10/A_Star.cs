using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public static class A_Star
    {
        public static Direction DirectionToTarget(IGameState state, int radius, Ant ant, int Target, int TargetLayer)
        {
            int x;
            int y;
            int z = 0;
            bool foundway = false;
            int lastindex = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, ant.Row, ant.Col, 0));
            Direction direction = new Direction();
            direction = Direction.Noone;
            Location startlocation = new Location(ant.Row, ant.Col);
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
                            if ((state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, state.test_x(x + 1), y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, state.test_x(x - 1), y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, state.test_y(y + 1), lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, state.test_y(y - 1), lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
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

        public static bool Explore(IGameState state, Ant ant, int radius)
        {
            int x;
            int y;
            int z = 0;
            int lastindex = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, ant.Row, ant.Col, 0));
            Direction direction = new Direction();
            direction = Direction.Noone;
            Location startlocation = new Location(ant.Row, ant.Col);
            try
            {
                while (z < radius)
                {
                    x = myKnoten[z].Now_x;
                    y = myKnoten[z].Now_y;
                    try
                    {
                        if ((state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, state.test_x(x + 1), y, lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.Water) && (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, state.test_x(x - 1), y, lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, state.test_y(y + 1), lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                            lastindex++;
                        }
                    }
                    catch { }
                    try
                    {
                        if ((state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.Water) && (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.MyAnt) && (knotenschonvorhanden(myKnoten, x, state.test_y(y - 1), lastindex) == false))
                        {
                            myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
                            lastindex++;
                        }
                    }
                    catch { }
                    if (z < lastindex) z++;
                    else break;
                }
            }
            catch { }

            int minimum = 2000;
            Knoten minimumk = myKnoten[0];
            int count = myKnoten.Count - 1;
            for (int i = 0; i < count; i++)
            {
                if (state.map[myKnoten[i].Now_x, myKnoten[i].Now_y, (int)Layer.Steps] < minimum)
                {
                    minimum = state.map[myKnoten[i].Now_x, myKnoten[i].Now_y, (int)Layer.Steps];
                    minimumk = myKnoten[i];
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
                Location newLoc = state.GetDestination(ant, direction);
                if (state.GetIsUnoccupied(newLoc))
                {
                    SendOrder(state, ant, direction, newLoc);
                    return true;
                }
            }
            return false;
        }

        public static bool knotenschonvorhanden(List<Knoten> myKnoten, int x, int y, int lastindex)
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

        public static void SendOrder(IGameState state, Ant ant, Direction direction, Location newLoc)
        {
            state.MoveAnt(ant, newLoc);
            IssueOrder(ant, direction);
        }

        public static void IssueOrder(Location loc, Direction direction)
        {
            System.Console.Out.WriteLine("o {0} {1} {2}", loc.Row, loc.Col, direction.ToChar());
        }
    }
}
