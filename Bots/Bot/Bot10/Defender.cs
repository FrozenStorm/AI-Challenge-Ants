using System;
using System.Collections.Generic;
using System.Linq;


namespace Ants
{

    class Defender : Bot
    {
        public int[] DefenderTeam = new int[20*4];
        Random myRandom = new Random();
        public void Handler(IGameState state)
        {
            for (int i = 0; i < state.MyHills.Count*4; i++)
            {
                if (DefenderTeam[i] != 0)
                {
                    int countant = state.MyDefAnts.Count - 1;
                    bool found = false;
                    for (int ant = 0; ant <= countant; ant++)
                    {
                        if (DefenderTeam[i] == state.MyDefAnts[ant].id) found = true;
                    }
                    if (found == false) DefenderTeam[i] = 0;
                }
                if (DefenderTeam[i] == 0)
                {
                    if(state.MyExpAnts.Count > (8 + ((i/4)*8)))
                    {
                        int countant = state.MyExpAnts.Count - 1;
                        for (int ant = 0; ant <= countant; ant++)
                        {
                            if ((state.MyExpAnts[ant].Col == state.MyHills[i / 4].Col) && (state.MyExpAnts[ant].Row == state.MyHills[i / 4].Row))
                            {
                                DefenderTeam[i] = state.MyExpAnts[ant].id;
                                state.MyExpAnts[ant].funktion = i;
                                state.MyDefAnts.Add(state.MyExpAnts[ant]);
                                state.MyExpAnts.Remove(state.MyExpAnts[ant]);
                                break;
                            }
                        }
                    }
                           
                  
                }
            }
        }
        public override void DoTurn(IGameState state)
        {
            int countant = state.MyDefAnts.Count - 1;
            for (int ant = 0; ant <= countant; ant++)
            {
                if (state.MyDefAnts[ant].turndone == false)
                {
                    if (NormalMove(state, state.MyDefAnts[ant])) { }
                    else if (A_Star.Explore(state, state.MyDefAnts[ant], 200)) { }
                    else RandomMove(state, state.MyDefAnts[ant]);
                }
                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 20) break;
            }
        }

        private bool NormalMove(IGameState state, Ant ant)
        {
            Direction direction = new Direction();
            direction = Direction.Noone;

            if (state.map[ant.Row, ant.Col, (int)Layer.Special] == ant.funktion) return true;
            else
            {
                if (direction == Direction.Noone) direction = A_Star.DirectionToTarget(state, 200, ant, ant.funktion, (int)Layer.Special);
            }
            
            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(ant, direction);
                A_Star.SendOrder(state, ant, direction, newLoc);
                return true;
            }
            else return false;
        }

        private void RandomMove(IGameState state, Ant ant)
        {
            List<Direction> directionList = new List<Direction>();
            directionList.Clear();
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
                    A_Star.SendOrder(state, ant, direction, newLoc);
                    break;
                }
                else directionList.Remove(direction);
            }
        }
    }
}