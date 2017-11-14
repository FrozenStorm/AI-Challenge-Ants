using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public static class Pathfinding
    {
        public static bool SendAntToTarget(IGameState state, int area, Location target)
        {
            int x;
            int y;
            int z = 0;
            bool foundway = false;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, target.Row, target.Col, 0));
            Direction direction = Direction.Noone;
            Ant myAnt = null;

            while (foundway == false)
            {
                x = myKnoten[z].Now_x;
                y = myKnoten[z].Now_y;

                if (z > area) return false;

                myAnt = state.MyAnts.Find(ant => ((ant.Row == x) && (ant.Col == y)));
                if (myAnt != null)
                {
                    if (myAnt.Moved == false)
                    {
                        foundway = true;
                    }
                    else // Falls ein Knoten ein Ant ist welcher bewegt wurde diesen Punkt aus Liste entfernen um Kollision zu verhindern
                    {
                        myKnoten.RemoveAt(z);
                        if (z < myKnoten.Count) { } // Nur weiterfahren falls es noch einen Knoten hat
                        else return false;
                    }
                }
                else
                {
                    if (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x + 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x + 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                            }
                        }
                    }

                    if (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x - 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x - 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y + 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y + 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y - 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y - 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
                            }
                        }
                    }

                    if (z < myKnoten.Count-1) z++;
                    else return false;
                }
            }

            z = myKnoten[z].From_z; // Letzter Punkt vor Ant
            direction = state.GetDirections(new Location(myAnt.Row, myAnt.Col), new Location(myKnoten[z].Now_x, myKnoten[z].Now_y));

            if (foundway == true)
            {
                if (direction != Direction.Noone)
                {
                    Location newLoc = new Location(myKnoten[z].Now_x, myKnoten[z].Now_y);
                    if (state.GetIsUnoccupied(newLoc))
                    {
                        state.SendOrder(state, myAnt, direction, newLoc);
                    }
                    else
                    {
                        for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                        {
                            for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                            {
                                state.map[state.test_x(myAnt.Row + n), state.test_y(myAnt.Col + m), (int)Layer.Steps] += 2;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    state.MyAnts[state.MyAnts.IndexOf(myAnt)].Moved = true;
                    state.NotMovedAnts--;
                    for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                    {
                        for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                        {
                            state.map[state.test_x(myAnt.Row + n), state.test_y(myAnt.Col + m), (int)Layer.Steps] += 2;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Send2AntToTarget(IGameState state, int area, Location target)
        {
            int x;
            int y;
            
            bool foundway1 = false;
            bool foundway2 = false;
            Direction direction1 = Direction.Noone;
            Direction direction2 = Direction.Noone;
            int from1;
            int from2;
            bool moveOK1 = false;
            bool moveOK2 = false;
            Location newLoc1 = null;
            Location newLoc2 = null;
            Ant myAnt1 = null;
            Ant myAnt2 = null;

            int z = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, target.Row, target.Col, 0));
            while (foundway1 == false)
            {
                x = myKnoten[z].Now_x;
                y = myKnoten[z].Now_y;

                if (z > area) return false;

                myAnt1 = state.MyAnts.Find(ant => ((ant.Row == x) && (ant.Col == y)));
                if (myAnt1 != null)
                {
                    if (myAnt1.Moved == false)
                    {
                        foundway1 = true;
                    }
                    else // Falls ein Knoten ein Ant ist welcher bewegt wurde diesen Punkt aus Liste entfernen um Kollision zu verhindern
                    {
                        myKnoten.RemoveAt(z);
                        if (z < myKnoten.Count) { } // Nur weiterfahren falls es noch einen Knoten hat
                        else return false;
                    }
                }
                else
                {
                    if (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x + 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x + 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                            }
                        }
                    }

                    if (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x - 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x - 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y + 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y + 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y - 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y - 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
                            }
                        }
                    }

                    if (z < myKnoten.Count-1) z++;
                    else return false;
                }
            }

            from1 = myKnoten[z].From_z; // Letzter Punkt vor Ant
            direction1 = state.GetDirections(new Location(myAnt1.Row, myAnt1.Col), new Location(myKnoten[from1].Now_x, myKnoten[from1].Now_y));

            //Zweite Ant

            z = 0;
            myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, target.Row, target.Col, 0));
            while (foundway2 == false)
            {
                x = myKnoten[z].Now_x;
                y = myKnoten[z].Now_y;

                if (z > area) return false;

                myAnt2 = state.MyAnts.Find(ant => ((ant.Row == x) && (ant.Col == y)));
                if (myAnt2 != null && myAnt2 != myAnt1)
                {
                    if (myAnt2.Moved == false)
                    {
                        foundway2 = true;
                    }
                    else // Falls ein Knoten ein Ant ist welcher bewegt wurde diesen Punkt aus Liste entfernen um Kollision zu verhindern
                    {
                        myKnoten.RemoveAt(z);
                        if (z <= myKnoten.Count) { } // Nur weiterfahren falls es noch einen Knoten hat
                        else return false;
                    }
                }
                else
                {
                    if (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x + 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x + 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                            }
                        }
                    }

                    if (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[state.test_x(x - 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x - 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y + 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y + 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] != (int)Tile.Water)
                    {
                        if (state.map[x, state.test_y(y - 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y - 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
                            }
                        }
                    }

                    if (z < myKnoten.Count-1) z++;
                    else return false;
                }
            }

            from2 = myKnoten[z].From_z; // Letzter Punkt vor Ant
            direction2 = state.GetDirections(new Location(myAnt2.Row, myAnt2.Col), new Location(myKnoten[from2].Now_x, myKnoten[from2].Now_y));


            if (foundway1 == true && foundway2 == true)
            {
                if (direction1 != Direction.Noone)
                {
                    newLoc1 = new Location(myKnoten[from1].Now_x, myKnoten[from1].Now_y);
                    if (state.GetIsUnoccupied(newLoc1))
                    {
                        moveOK1 = true;

                    }
                    else
                    {
                        for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                        {
                            for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                            {
                                state.map[state.test_x(myAnt1.Row + n), state.test_y(myAnt1.Col + m), (int)Layer.Steps] += 2;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    moveOK1 = true;
                }
                if (direction2 != Direction.Noone)
                {
                    newLoc2 = new Location(myKnoten[from2].Now_x, myKnoten[from2].Now_y);
                    if (state.GetIsUnoccupied(newLoc2))
                    {
                        moveOK2 = true;

                    }
                    else
                    {
                        for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                        {
                            for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                            {
                                state.map[state.test_x(myAnt2.Row + n), state.test_y(myAnt2.Col + m), (int)Layer.Steps] += 2;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    moveOK2 = true;
                }
                if (moveOK1 == true && moveOK2 == true)
                {
                    if (direction1 != Direction.Noone) state.SendOrder(state, myAnt1, direction1, newLoc1);
                    else
                    {
                        state.MyAnts[state.MyAnts.IndexOf(myAnt1)].Moved = true;
                        state.NotMovedAnts--;
                        for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                        {
                            for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                            {
                                state.map[state.test_x(myAnt1.Row + n), state.test_y(myAnt1.Col + m), (int)Layer.Steps]+=2;
                            }
                        }
                    }

                    if (direction2 != Direction.Noone) state.SendOrder(state, myAnt2, direction2, newLoc2);
                    else
                    {
                        state.MyAnts[state.MyAnts.IndexOf(myAnt2)].Moved = true;
                        state.NotMovedAnts--;
                        for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                        {
                            for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                            {
                                state.map[state.test_x(myAnt2.Row + n), state.test_y(myAnt2.Col + m), (int)Layer.Steps] += 2;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Explore(IGameState state, Ant ant, int radius)
        {
            int x;
            int y;
            int z = 0;
            List<Knoten> myKnoten = new List<Knoten>();
            myKnoten.Add(new Knoten(z, ant.Row, ant.Col, 0));
            Direction direction = new Direction();
            direction = Direction.Noone;
            Location startlocation = new Location(ant.Row, ant.Col);
            while (z < radius)
            {
                x = myKnoten[z].Now_x;
                y = myKnoten[z].Now_y;


                    if (state.map[state.test_x(x + 1), y, (int)Layer.Tiles] == (int)Tile.Land)
                    {
                        if (state.map[state.test_x(x + 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x + 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x + 1), y, 0));
                            }
                        }
                    }

                    if (state.map[state.test_x(x - 1), y, (int)Layer.Tiles] == (int)Tile.Land)
                    {
                        if (state.map[state.test_x(x - 1), y, (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == state.test_x(x - 1)) && (knoten.Now_y == y)))==null)
                            {
                                myKnoten.Add(new Knoten(z, state.test_x(x - 1), y, 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y + 1), (int)Layer.Tiles] == (int)Tile.Land)
                    {
                        if (state.map[x, state.test_y(y + 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y + 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y + 1), 0));
                            }
                        }
                    }

                    if (state.map[x, state.test_y(y - 1), (int)Layer.Tiles] == (int)Tile.Land)
                    {
                        if (state.map[x, state.test_y(y - 1), (int)Layer.Danger] <= (int)DangerGrade.NoDanger)
                        {
                            if(myKnoten.Find(knoten => ((knoten.Now_x == x) && (knoten.Now_y == state.test_y(y - 1))))==null)
                            {
                                myKnoten.Add(new Knoten(z, x, state.test_y(y - 1), 0));
                            }
                        }
                    }
                if (z < myKnoten.Count-1) z++;
                else break;
            }
            
            int minimum = 10000;
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
                    state.SendOrder(state, ant, direction, newLoc);
                    return true;
                }
                else
                {
                    for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                    {
                        for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                        {
                            state.map[state.test_x(ant.Row + n), state.test_y(ant.Col + m), (int)Layer.Steps] += 2;
                        }
                    }
                }
            }
            else
            {
                for (int n = -state.StepsIncreasRadius; n <= state.StepsIncreasRadius; n++)
                {
                    for (int m = -state.StepsIncreasRadius; m <= state.StepsIncreasRadius; m++)
                    {
                        state.map[state.test_x(ant.Row + n), state.test_y(ant.Col + m), (int)Layer.Steps] += 2;
                    }
                }
            }
            return false;
        }
        
        public static void RandomMove(IGameState state, Ant ant)
        {
            Random myRandom = new Random(DateTime.Now.Millisecond);

            List<Direction> directionList = new List<Direction>();
            Direction direction = new Direction();

            directionList.Add(Direction.West);
            directionList.Add(Direction.North);
            directionList.Add(Direction.South);
            directionList.Add(Direction.East);

            for (int directionsleft = 4; directionsleft > 0; directionsleft--)
            {
                direction = directionList[myRandom.Next(directionsleft)];
                Location newLoc = state.GetDestination(ant, direction);
                if (state.GetIsUnoccupied(newLoc))
                {
                    state.SendOrder(state, ant, direction, newLoc);
                    break;
                }
                else directionList.Remove(direction);
            }
        }
    }
}
