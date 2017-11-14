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

		public List<Ant> MyAnts { get; set; }
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
			
			MyAnts = new List<Ant>();
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
				}
			}
		}

		#region State mutators
        public void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.Now;

            // clear ant data
            foreach (Location loc in MyAnts) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in MyHills) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in EnemyAnts) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in EnemyHills) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            foreach (Location loc in DeadTiles) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;

            MyHills.Clear();
            MyAnts.Clear();
            EnemyHills.Clear();
            EnemyAnts.Clear();
            DeadTiles.Clear();

            // set all known food to unseen
            foreach (Location loc in FoodTiles) map[loc.Row, loc.Col, (int)Layer.Tiles] = (int)Tile.Land;
            FoodTiles.Clear();
        }

		public void AddAnt (int row, int col, int team) {
			
			
			Ant ant = new Ant(row, col, team);
			if (team == 0) 
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.MyAnt;
				MyAnts.Add(ant);
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
            map[row, col, (int)Layer.Steps] = 2000;
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
            map[newLoc.Row, newLoc.Col, (int)Layer.Steps]++;
            map[newLoc.Row, newLoc.Col, (int)Layer.Tiles] = map[ant.Row, ant.Col, (int)Layer.Tiles];
            map[ant.Row, ant.Col, (int)Layer.Tiles] = (int)Tile.Land;
            MyAnts[MyAnts.IndexOf(ant)] = new Ant(newLoc.Row, newLoc.Col, ant.Team);
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
			foreach (Ant ant in this.MyAnts)
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

	}
}

