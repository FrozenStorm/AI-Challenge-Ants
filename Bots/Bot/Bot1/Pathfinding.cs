using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    class Pathfinding
    {
        List<Point> Pathpoints = new List<Point>();
        public Location pathfinding(GameState state, Ant Beginn, Location Finish)
        {
            int x;
            int y;
            int z = 0;
            int lastindex = 0;
            bool foundway = false;
            List<Knoten> myKnoten = new List<Knoten>();
            Pathpoints = new List<Point>();
            myKnoten.Add(new Knoten(z, Beginn.Row, Beginn.Col));
            try
            {
                while (foundway == false)
                {
                    x = myKnoten[z].Now_x;
                    y = myKnoten[z].Now_y;
                    if ((x == Finish.Row) && (y == Finish.Col))
                    {
                        foundway = true;
                    }
                    else
                    {
                        try
                        {
                            if ((state.map[x + 1, y] != Tile.Water) && (knotenschonvorhanden(myKnoten, x + 1, y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x + 1, y));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x - 1, y] != Tile.Water) && (knotenschonvorhanden(myKnoten, x - 1, y, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x - 1, y));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, y + 1] != Tile.Water) && (knotenschonvorhanden(myKnoten, x, y + 1, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, y + 1));
                                lastindex++;
                            }
                        }
                        catch { }
                        try
                        {
                            if ((state.map[x, y - 1] != Tile.Water) && (knotenschonvorhanden(myKnoten, x, y - 1, lastindex) == false))
                            {
                                myKnoten.Add(new Knoten(z, x, y - 1));
                                lastindex++;
                            }
                        }
                        catch { }
                        if (z < lastindex) z++;
                        else return myKnoten[0];
                    }
                }
            }
            catch { }


            while (z > 0)
            {
                Pathpoints.Add(new Point(myKnoten[z].Now_x, myKnoten[z].Now_y));
                z = myKnoten[z].From_z;
            }
            return true;
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
    }
}
