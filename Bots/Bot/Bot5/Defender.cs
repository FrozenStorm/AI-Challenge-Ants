using System;
using System.Collections.Generic;



namespace Ants
{

    class Defender : Bot
    {
        public int[] DefenderTeam = new int[20*4];
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
                    if(state.MyExpAnts.Count > (10 + ((i/4)*10)))
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
                if (NormalMove(state, state.MyDefAnts[ant])) { }
                else if (state.Explore(state, state.MyDefAnts[ant], 200)) { }
                else state.RandomMove(state, state.MyDefAnts[ant]);
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
                if (direction == Direction.Noone) direction = state.DirectionToTarget(state, 100, ant, ant.funktion, (int)Layer.Special);
            }
            
            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(ant, direction);
                state.SendOrder(state, ant, direction, newLoc);
                return true;
            }
            else return false;
        }       

    }
}