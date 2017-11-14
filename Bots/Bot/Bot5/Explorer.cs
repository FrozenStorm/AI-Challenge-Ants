using System;
using System.Collections.Generic;



namespace Ants
{
    //funktion 66 // 77
    class Explorer : Bot
    {
        public Ant[] ExplorerTeam = new Ant[500 * 2];
        public void Handler(IGameState state)
        {
            int countantarray = ExplorerTeam.Length - 1;
            for (int i = 0; i < countantarray; i++)
            {
                if (ExplorerTeam[i] != null)
                {
                    int countantlist = state.MyExpAnts.Count-1;
                    bool found = false;
                    for (int ant = 0; ant <= countantlist; ant++)
                    {
                        if (ExplorerTeam[i].id == state.MyExpAnts[ant].id) found = true;
                    }
                    if (found == false) ExplorerTeam[i] = null;
                }
                if (ExplorerTeam[i] == null)
                {
                    int countantlist = state.MyExpAnts.Count-1;
                    for (int ant = 0; ant <= countantlist; ant++)
                    {
                        if (state.MyExpAnts[ant].funktion == 0)
                        {
                            state.MyExpAnts[ant].funktion = 66;
                            ExplorerTeam[i] = state.MyExpAnts[ant];
                        }
                    }
                }
            }
        }
        public override void DoTurn(IGameState state)
        {
            int countant = ExplorerTeam.Length - 2;
            for (int ant = 0; ant <= countant; ant+=2)
            {
                state.MyExpAnts.

                if (NormalMove(state, ant)) { }
                else if (state.Explore(state, state.MyExpAnts[ant], 200)) { }
                else state.RandomMove(state, state.MyExpAnts[ant]);



                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 20) break;
            }
        }

        private bool NormalMove(IGameState state, int ant)
        {
            Direction direction = new Direction();
            direction = Direction.Noone;

            if (direction == Direction.Noone && state.EnemyHills.Count > 0) direction = state.DirectionToTarget(state, 1000, state.MyExpAnts[ant], (int)Tile.EnemyHill, (int)Layer.Tiles);
            if (direction == Direction.Noone && state.FoodTiles.Count > 0) direction = state.DirectionToTarget(state, 365, state.MyExpAnts[ant], (int)Tile.Food, (int)Layer.Tiles);
            //if (direction == Direction.Noone && state.EnemyAnts.Count > 0) direction = state.AtkEnemy(state, ant);
            if (direction == Direction.Noone && state.EnemyAnts.Count > 0) direction = state.DirectionToTarget(state, 365, state.MyExpAnts[ant], (int)Tile.EnemyAnt, (int)Layer.Tiles);

            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(state.MyExpAnts[ant], direction);
                state.SendOrder(state, state.MyExpAnts[ant], direction, newLoc);


                ant++;

                switch (direction)
                {
                    case Direction.East:
                    case Direction.West:
                        if (state.GetIsUnoccupied(new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col - 1))))
                        {
                            direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant - 1], new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col - 1)));
                        }
                        else
                        {
                            if (direction == Direction.East)
                            {
                                if (state.GetIsUnoccupied(new Location(state.test_x(state.MyExpAnts[ant - 1].Row-1), state.MyExpAnts[ant - 1].Col)))
                                {
                                    direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant - 1], new Location(state.test_x(state.MyExpAnts[ant - 1].Row-1), state.MyExpAnts[ant - 1].Col));
                                }
                            }
                            else
                            {
                                if (state.GetIsUnoccupied(new Location(state.test_x(state.MyExpAnts[ant - 1].Row + 1), state.MyExpAnts[ant - 1].Col)))
                                {
                                    direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant + 1], new Location(state.test_x(state.MyExpAnts[ant - 1].Row - 1), state.MyExpAnts[ant - 1].Col));
                                }
                            }
                        }
                        break;
                    case Direction.North:
                    case Direction.South:
                        if (state.GetIsUnoccupied(new Location(state.test_x(state.MyExpAnts[ant - 1].Row - 1), state.MyExpAnts[ant - 1].Col)))
                        {
                            direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant - 1], new Location(state.test_x(state.MyExpAnts[ant - 1].Row - 1), state.MyExpAnts[ant - 1].Col));
                        }
                        else
                        {
                            if (direction == Direction.North)
                            {
                                if (state.GetIsUnoccupied(new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col - 1))))
                                {
                                    direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant - 1], new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col - 1)));
                                }
                            }
                            else
                            {
                                if (state.GetIsUnoccupied(new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col + 1))))
                                {
                                    direction = state.DirectionToLocation(state, 100, state.MyExpAnts[ant - 1], new Location(state.MyExpAnts[ant - 1].Row, state.test_y(state.MyExpAnts[ant - 1].Col + 1)));
                                }
                            }
                        }
                        break;
                }
                if (direction != Direction.Noone)
                {
                    newLoc = state.GetDestination(state.MyExpAnts[ant], direction);
                    state.SendOrder(state, state.MyExpAnts[ant], direction, newLoc);
                }




                return true;
            }
            else return false;
        }

        

    }
}