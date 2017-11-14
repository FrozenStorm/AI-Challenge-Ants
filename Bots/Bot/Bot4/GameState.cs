using System;
using System.Collections.Generic;

namespace Ants 
{
	
	public class GameState : IGameState 
    {
		
		public int Width { get; set; }
		public int Height { get; set; }
		
		public int LoadTime { get; set; }
		public int TurnTime { get; set; }
		
		private DateTime turnStart;
		public int TimeRemaining 
        {
			get 
            {
				TimeSpan timeSpent = DateTime.Now - turnStart;
				return TurnTime - timeSpent.Milliseconds;
			}
		}

		public int ViewRadius2 { get; set; }
		public int AttackRadius2 { get; set; }
		public int SpawnRadius2 { get; set; }

        public int nextid;
        public List<Ant> MyNewAnts { get; set; }
		public List<Ant> MyAtkAnts { get; set; }
        public List<Ant> MyDefAnts { get; set; }
        public List<Ant> MyExpAnts { get; set; }
		public List<AntHill> MyHills { get; set; }
		public List<Ant> EnemyAnts { get; set; }
		public List<AntHill> EnemyHills { get; set; }
		public List<Location> DeadTiles { get; set; }
		public List<Location> FoodTiles { get; set; }

        public int[,,] map { get; set; }
		
		public GameState (int width, int height, int turntime, int loadtime, int viewradius2, int attackradius2, int spawnradius2) 
        {
			
			Width = width;
			Height = height;
			
			LoadTime = loadtime;
			TurnTime = turntime;
			
			ViewRadius2 = viewradius2;
			AttackRadius2 = attackradius2;
			SpawnRadius2 = spawnradius2;

            nextid = 1;
            MyNewAnts = new List<Ant>();
			MyAtkAnts = new List<Ant>();
            MyDefAnts = new List<Ant>();
            MyExpAnts = new List<Ant>();
			MyHills = new List<AntHill>();
			EnemyAnts = new List<Ant>();
			EnemyHills = new List<AntHill>();
			DeadTiles = new List<Location>();
			FoodTiles = new List<Location>();
			
			map = new int[height, width, 3];
			for (int row = 0; row < height; row++) 
            {
				for (int col = 0; col < width; col++) 
                {
					map[row, col,(int)Layer.Tiles] = (int)Tile.Land;
                    map[row, col, (int)Layer.Steps] = 0;
                    map[row, col, (int)Layer.Special] = 999;
				}
			}
		}

		#region State mutators
        public void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.Now;

            // clear ant data
            foreach (Location loc in MyHills) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in EnemyAnts) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in DeadTiles) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;

            MyNewAnts.Clear();
            MyHills.Clear();
            EnemyHills.Clear();
            EnemyAnts.Clear();
            DeadTiles.Clear();

            // set all known food to unseen
            foreach (Location loc in FoodTiles) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            FoodTiles.Clear();
        }
        public void UpdateSpecial()
        {
            int counthill = MyHills.Count;
            for (int i = 0; i < counthill; i++)
            {
                if (map[test_x(MyHills[i].Row + 1), test_y(MyHills[i].Col + 1), (int)Layer.Tiles] != (int)Tile.Water) map[test_x(MyHills[i].Row + 1), test_y(MyHills[i].Col + 1), (int)Layer.Special] = i * 4 + 0;
                if (map[test_x(MyHills[i].Row - 1), test_y(MyHills[i].Col - 1), (int)Layer.Tiles] != (int)Tile.Water) map[test_x(MyHills[i].Row - 1), test_y(MyHills[i].Col - 1), (int)Layer.Special] = i * 4 + 1;
                if (map[test_x(MyHills[i].Row + 1), test_y(MyHills[i].Col - 1), (int)Layer.Tiles] != (int)Tile.Water) map[test_x(MyHills[i].Row + 1), test_y(MyHills[i].Col - 1), (int)Layer.Special] = i * 4 + 2;
                if (map[test_x(MyHills[i].Row - 1), test_y(MyHills[i].Col + 1), (int)Layer.Tiles] != (int)Tile.Water) map[test_x(MyHills[i].Row - 1), test_y(MyHills[i].Col + 1), (int)Layer.Special] = i * 4 + 3;
            }
        }
        public void UpdateAnts()
        {
            int countant = MyAtkAnts.Count - 1;
            for (int ant = 0; ant <= countant; ant++)
            {
                if (!MyNewAnts.Contains(MyAtkAnts[ant]))
                {
                    if (map[MyAtkAnts[ant].Row, MyAtkAnts[ant].Col, (int)Layer.Tiles] != (int)Tile.Food) map[MyAtkAnts[ant].Row, MyAtkAnts[ant].Col, (int)Layer.Tiles] = (int)Tile.Land;
                    MyAtkAnts.RemoveAt(ant);
                    countant--;
                    ant--;
                }
                else MyNewAnts.Remove(MyAtkAnts[ant]);
            }

            countant = MyDefAnts.Count - 1;
            for (int ant = 0; ant <= countant; ant++)
            {
                if (!MyNewAnts.Contains(MyDefAnts[ant]))
                {
                    if (map[MyDefAnts[ant].Row, MyDefAnts[ant].Col, (int)Layer.Tiles] != (int)Tile.Food) map[MyDefAnts[ant].Row, MyDefAnts[ant].Col, (int)Layer.Tiles] = (int)Tile.Land;
                    MyDefAnts.RemoveAt(ant);
                    countant--;
                    ant--;
                }
                else MyNewAnts.Remove(MyDefAnts[ant]);
            }

            countant = MyExpAnts.Count - 1;
            for (int ant = 0; ant <= countant; ant++)
            {
                if (!MyNewAnts.Contains(MyExpAnts[ant]))
                {
                    if (map[MyExpAnts[ant].Row, MyExpAnts[ant].Col, (int)Layer.Tiles] != (int)Tile.Food) map[MyExpAnts[ant].Row, MyExpAnts[ant].Col, (int)Layer.Tiles] = (int)Tile.Land;
                    MyExpAnts.RemoveAt(ant);
                    countant--;
                    ant--;
                }
                else MyNewAnts.Remove(MyExpAnts[ant]);
            }
            foreach (Ant ant in MyNewAnts)
            {
                ant.id = nextid;
                nextid++;
                map[ant.Row, ant.Col, (int)Layer.Tiles] = (int)Tile.MyAnt;
                MyExpAnts.Add(ant);
            }
        }
		public void AddAnt (int row, int col, int team) {
			
			
			Ant ant = new Ant(row, col, team);
			if (team == 0) 
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.MyAnt;
                MyNewAnts.Add(ant);
			} 
            else 
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.EnemyAnt;
				EnemyAnts.Add(ant);

			}
		}

		public void AddFood (int row, int col) {
            map[row, col, (int)Layer.Tiles] = (int)Tile.Food;
			FoodTiles.Add(new Location(row, col));
		}

		public void RemoveFood (int row, int col) {
			// an ant could move into a spot where a food just was
			// don't overwrite the space unless it is food
            if (map[row, col, (int)Layer.Tiles] == (int)Tile.Food)
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.Land;
			}
			FoodTiles.Remove(new Location(row, col));
		}

		public void AddWater (int row, int col) {
            map[row, col, (int)Layer.Tiles] = (int)Tile.Water;
            map[row, col, (int)Layer.Steps] = 5000;
		}

		public void DeadAnt (int row, int col) {
			// food could spawn on a spot where an ant just died
			// don't overwrite the space unless it is land
            if (map[row, col, (int)Layer.Tiles] == (int)Tile.Land)
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.Dead;
			}
			
			// but always add to the dead list
			DeadTiles.Add(new Location(row, col));
		}

		public void AntHill (int row, int col, int team) {

			

			AntHill hill = new AntHill (row, col, team);
            if (team == 0)
            {
                if (map[row, col, (int)Layer.Tiles] == (int)Tile.Land)
                {
                    map[row, col, (int)Layer.Tiles] = (int)Tile.MyHill;
                }
                MyHills.Add(hill);
            }
            else
            {
                if (map[row, col, (int)Layer.Tiles] == (int)Tile.Land)
                {
                    map[row, col, (int)Layer.Tiles] = (int)Tile.EnemyHill;
                }
                EnemyHills.Add(hill);
            }
		}
		#endregion

		/// <summary>
		/// Gets whether <paramref name="location"/> is passable or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
		/// <seealso cref="GetIsUnoccupied"/>
		public bool GetIsPassable (Location location) 
        {
            return map[location.Row, location.Col, (int)Layer.Tiles] != (int)Tile.Water;
		}
		
		/// <summary>
		/// Gets whether <paramref name="location"/> is occupied or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
		public bool GetIsUnoccupied (Location location) 
        {
            return GetIsPassable(location) && map[location.Row, location.Col, (int)Layer.Tiles] != (int)Tile.MyAnt;
		}
		
		/// <summary>
		/// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
		/// </summary>
		/// <param name="location">The starting location.</param>
		/// <param name="direction">The direction to move.</param>
		/// <returns>The new location, accounting for wrap around.</returns>
		public Location GetDestination (Location location, Direction direction) 
        {
			Location delta = Ants.Aim[direction];
			
			int row = (location.Row + delta.Row) % Height;
			if (row < 0) row += Height; // because the modulo of a negative number is negative

			int col = (location.Col + delta.Col) % Width;
			if (col < 0) col += Width;
			
			return new Location(row, col);
		}

		/// <summary>
		/// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The first location to measure with.</param>
		/// <param name="loc2">The second location to measure with.</param>
		/// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
		public int GetDistance (Location loc1, Location loc2) 
        {
			int d_row = Math.Abs(loc1.Row - loc2.Row);
			d_row = Math.Min(d_row, Height - d_row);
			
			int d_col = Math.Abs(loc1.Col - loc2.Col);
			d_col = Math.Min(d_col, Width - d_col);
			
			return d_row + d_col;
		}

		/// <summary>
		/// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The location to start from.</param>
		/// <param name="loc2">The location to determine directions towards.</param>
		/// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
		public Direction GetDirections (Location loc1, Location loc2) 
        {
			if (loc1.Row < loc2.Row) 
            {
                if (loc2.Row - loc1.Row >= Height / 2)
                    return Direction.North;
				if (loc2.Row - loc1.Row <= Height / 2)
                    return Direction.South;
            }
			if (loc2.Row < loc1.Row) 
            {
				if (loc1.Row - loc2.Row >= Height / 2)
                    return Direction.South;
				if (loc1.Row - loc2.Row <= Height / 2)
                    return Direction.North;
			}
			
			if (loc1.Col < loc2.Col) 
            {
				if (loc2.Col - loc1.Col >= Width / 2)
                    return Direction.West;
				if (loc2.Col - loc1.Col <= Width / 2)
                    return Direction.East;
			}
			if (loc2.Col < loc1.Col) 
            {
				if (loc1.Col - loc2.Col >= Width / 2)
                    return Direction.East;
				if (loc1.Col - loc2.Col <= Width / 2)
                    return Direction.West;
			}
			return Direction.Noone;

		}

        public void MoveAnt(Ant ant, Location newLoc)
        {
            Ant newAnt = new Ant(newLoc.Row, newLoc.Col, ant.Team);
            newAnt.id=ant.id;
            newAnt.funktion=ant.funktion;
            map[newLoc.Row, newLoc.Col, (int)Layer.Steps]++;
            map[newLoc.Row, newLoc.Col, (int)Layer.Tiles] = map[ant.Row, ant.Col, (int)Layer.Tiles];
            map[ant.Row, ant.Col, (int)Layer.Tiles] = (int)Tile.Land;
            if (MyExpAnts.Contains(ant)) MyExpAnts[MyExpAnts.IndexOf(ant)] = newAnt;
            if (MyDefAnts.Contains(ant)) MyDefAnts[MyDefAnts.IndexOf(ant)] = newAnt;
            if (MyAtkAnts.Contains(ant)) MyAtkAnts[MyAtkAnts.IndexOf(ant)] = newAnt;
        }
		
		public bool GetIsVisible(Location loc)
		{
			List<Location> offsets = new List<Location>();
			int squares = (int)Math.Floor(Math.Sqrt(this.ViewRadius2));
			for (int r = -1 * squares; r <= squares; ++r)
			{
				for (int c = -1 * squares; c <= squares; ++c)
				{
					int square = r * r + c * c;
					if (square < this.ViewRadius2)
					{
						offsets.Add(new Location(r, c));
					}
				}
			}
            foreach (Ant ant in this.MyExpAnts)
			{
				foreach (Location offset in offsets)
				{
					if ((ant.Col + offset.Col) == loc.Col &&
						(ant.Row + offset.Row) == loc.Row)
					{
								 return true;
					}
				}
			}
			return false;
		}
        public int test_x(int x)
        {
            if (x > Height - 1) x = 0;
            if (x < 0) x = Height - 1;
            return x;
        }
        public int test_y(int y)
        {
            if (y > Width - 1) y = 0;
            if (y < 0) y = Width - 1;
            return y;
        }
	}
}

