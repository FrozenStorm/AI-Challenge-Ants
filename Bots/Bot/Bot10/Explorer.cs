using System;
using System.Collections.Generic;
using System.Linq;


namespace Ants
{
    // Funktion 200 == Fight 201 == lauf weg
    class Explorer : Bot
    {
        Random myRandom = new Random();

        public void Handler(IGameState state)
        {
        }
        public override void DoTurn(IGameState state)
        {
            int countant = state.MyExpAnts.Count - 1;
            for (int ant = countant; ant >= 0; ant--)
            {
                if (state.MyExpAnts[ant].turndone == false)
                {
                    if (state.TimeRemaining > 100)
                    {
                        if (NormalMove(state, state.MyExpAnts[ant])) { }
                        else if (AtkEnemy(state, state.MyExpAnts[ant])) { }
                        else if (A_Star.Explore(state, state.MyExpAnts[ant], 200)) { }
                        else RandomMove(state, state.MyExpAnts[ant]);
                    }
                    else
                    {
                        if (A_Star.Explore(state, state.MyExpAnts[ant], 200)) { }
                        else RandomMove(state, state.MyExpAnts[ant]);
                    }
                }
                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 20) break;
            }
        }

        private bool NormalMove(IGameState state, Ant ant)
        {
            Direction direction = new Direction();
            direction = Direction.Noone;

            if (direction == Direction.Noone && state.EnemyHills.Count > 0) direction = A_Star.DirectionToTarget(state, 550, ant, (int)Tile.EnemyHill, (int)Layer.Tiles);
            if (direction == Direction.Noone && state.FoodTiles.Count > 0) direction = A_Star.DirectionToTarget(state, 365, ant, (int)Tile.Food, (int)Layer.Tiles);

            if (direction != Direction.Noone)
            {
                Location newLoc = state.GetDestination(ant, direction);
                A_Star.SendOrder(state, ant, direction, newLoc);
                return true;
            }
            else return false;
        }

 
        public void FightOrNot(IGameState state)
        {
            int count = 0;
            int count2 = 0;
            List<Ant> fightingmyant = new List<Ant>();
            List<Ant> nearmyant = new List<Ant>();
            List<Ant> nearenemyant = new List<Ant>();
            count = state.EnemyAnts.Count;
            for (int i = 0; i < count; i++)
            {
                count2 = state.MyExpAnts.Count;
                for (int m = 0; m < count2; m++)
                {
                    if (state.GetDistance(state.EnemyAnts[i], state.MyExpAnts[m]) <= 36 && !fightingmyant.Contains(state.MyExpAnts[m])) fightingmyant.Add(state.MyExpAnts[m]);
                }
            }
            count2 = fightingmyant.Count;
            for (int m = 0; m < count2; m++)
            {
                if (fightingmyant[m].funktion != 200)
                {
                    nearenemyant.Clear();
                    count = state.EnemyAnts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (state.GetDistance(fightingmyant[m], state.EnemyAnts[i]) <= 36) nearenemyant.Add(state.EnemyAnts[i]);
                    }
                    count = nearenemyant.Count;
                    for (int g = 0; g < count; g++)
                    {
                        nearmyant.Clear();
                        count = fightingmyant.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (state.GetDistance(nearenemyant[g], fightingmyant[i]) <= 36) nearmyant.Add(fightingmyant[i]);
                        }
                        if (nearmyant.Count > nearenemyant.Count)
                        {
                           //Problem mit eigenen verschiedenen Lists
                        }
                        else
                        {
                        }
                    }
                    
                }
             }
        }


        public bool AtkEnemy(IGameState state, Ant ant)
        {
            bool walked = false;
            List<Ant> nearmyant = new List<Ant>();
            List<Ant> nearenemyant = new List<Ant>();
            int count = 0;
            count = state.EnemyAnts.Count;
            for(int i=0;i<count;i++)
            {
                if (state.GetDistance(ant, state.EnemyAnts[i]) <= 36) nearenemyant.Add(state.EnemyAnts[i]);
            }
            if (nearenemyant.Count == 0) return false;
            nearmyant.Clear();
            count = state.MyExpAnts.Count;
            for(int i=0;i<count;i++)
            {
                if (state.GetDistance(nearenemyant[0], state.MyExpAnts[i]) <= 36) nearmyant.Add(state.MyExpAnts[i]);
            }

            if (nearmyant.Count > nearenemyant.Count)
            {
                Direction direction;
                state.map[nearenemyant[0].Row, nearenemyant[0].Col, (int)Layer.Special] = 100;
                count = nearmyant.Count;
                for(int i=0;i<count;i++)
                {
                    if (nearmyant[i].turndone == false)
                    {
                        direction = Direction.Noone;
                        direction = A_Star.DirectionToTarget(state, 365, nearmyant[i], 100, (int)Layer.Special);
                        if (direction != Direction.Noone)
                        {
                            if (nearmyant[i] == ant) walked = true;
                            Location newLoc = state.GetDestination(nearmyant[i], direction);
                            if (state.GetIsUnoccupied(newLoc))
                            {
                                A_Star.SendOrder(state, nearmyant[i], direction, newLoc);
                            }
                        }
                    }
                }
                state.map[nearenemyant[0].Row, nearenemyant[0].Col, (int)Layer.Special] = 0;
            }

            if (nearmyant.Count <= nearenemyant.Count)
            {
                state.map[state.test_x(ant.Row - (nearenemyant[0].Row - ant.Row)), state.test_y(ant.Col - (nearenemyant[0].Col - ant.Col)), (int)Layer.Special] = 100;
                Direction direction;
                direction = A_Star.DirectionToTarget(state, 365, ant, 100, (int)Layer.Special);
                if (direction != Direction.Noone)
                {
                    walked = true;
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsUnoccupied(newLoc))
                    {
                        A_Star.SendOrder(state, ant, direction, newLoc);
                    }
                }
                state.map[state.test_x(ant.Row - (nearenemyant[0].Row - ant.Row)), state.test_y(ant.Col - (nearenemyant[0].Col - ant.Col)), (int)Layer.Special] = 0;

            }
            if (walked == true) return true;
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