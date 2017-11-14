using System;
using System.Collections.Generic;
#if DEBUG
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace Ants 
{
	
	public class GameState : IGameState 
    { 
        #if DEBUG
        public List<Bitmap> mybitmap = new List<Bitmap>();
        #endif
        public int StepsIncreasRadius { get; set; }
        public int Layers { get; set; }
        public int DangerRadius { get; set; }
        public int AttackBorderRadius { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }

        public int TurnNumber { get; set; }
		public int LoadTime { get; set; }
		public int TurnTime { get; set; }
        public int NotMovedAnts { get; set; }
		
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
        public List<Target> FoodTargets { get; set; }
        public List<Target> EnemyAntTargets { get; set; }
        public List<Target> EnemyHillTargets { get; set; }
        public List<Target> AttackBorder { get; set; }

        public int[,,] map { get; set; }

		public GameState (int width, int height, int turntime, int loadtime, int viewradius2, int attackradius2, int spawnradius2) 
        {
            StepsIncreasRadius = 4;//Original 4
            DangerRadius = 3;//Original 3
            AttackBorderRadius = 4;//Original 4
            Layers = 3;

            TurnNumber = 0;
			
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
			FoodTargets = new List<Target>();
            EnemyAntTargets = new List<Target>();
            EnemyHillTargets = new List<Target>();
            AttackBorder = new List<Target>();

            map = new int[Height, Width, Layers];
            for (int row = 0; row < Height; row++) 
            {
                for (int col = 0; col < Width; col++) 
                {
					map[row, col,(int)Layer.Tiles] = (int)Tile.Land;
                    map[row, col, (int)Layer.Danger] = 0;
                    map[row, col, (int)Layer.Steps] = 0;
				}
			}
		}

		#region State mutators
        public void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.Now;

            // Increase TurnNumber
            TurnNumber++;

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if(map[row, col, (int)Layer.Tiles] != (int)Tile.Water) map[row, col, (int)Layer.Tiles] = (int)Tile.Land;
                    map[row, col, (int)Layer.Danger] = 0;
                }
            }

            MyHills.Clear();
            MyAnts.Clear();
            EnemyHills.Clear();
            EnemyAnts.Clear();
            DeadTiles.Clear();
            FoodTargets.Clear();
            EnemyAntTargets.Clear();
            EnemyHillTargets.Clear();
            AttackBorder.Clear();
        }

		public void AddAnt (int row, int col, int team) {
			
			
			Ant ant = new Ant(row, col, team, false);
			if (team == 0) 
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.MyAnt;
                AddDanger(row, col, DangerGrade.Friend);
				MyAnts.Add(ant);
			} 
            else 
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.EnemyAnt;
                EnemyAntTargets.Add(new Target(row, col, 0));
                AddDanger(row, col, DangerGrade.Enemy);
                AddAttackBorder(row, col);
				EnemyAnts.Add(ant);
			}
		}

		public void AddFood (int row, int col) {
            map[row, col, (int)Layer.Tiles] = (int)Tile.Food;
            FoodTargets.Add(new Target(row, col, 0));
		}

		public void RemoveFood (int row, int col) {
			// an ant could move into a spot where a food just was
			// don't overwrite the space unless it is food
            if (map[row, col, (int)Layer.Tiles] == (int)Tile.Food)
            {
                map[row, col, (int)Layer.Tiles] = (int)Tile.Land;
			}
            int index = FoodTargets.FindIndex(point => point.Row == row && point.Col == col);
            if(index>=0)FoodTargets.RemoveAt(index);
		}

		public void AddWater (int row, int col) {
            map[row, col, (int)Layer.Tiles] = (int)Tile.Water;
            map[row, col, (int)Layer.Steps] = 100000;
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
                EnemyHillTargets.Add(new Target(row, col, 0));
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

        public int GetDistancePriority(Location loc1) //Umso näher am eigenen Hügel umso wichtiger ist das Ziel
        {
            int closest = 200 * 200;
            int distance = 0;
            int count = MyHills.Count;
            for(int i = 0;i<count;i++)
            {
                distance = GetDistance(loc1, MyHills[i]);
                if (distance < closest) closest = distance;
            }
            return closest;
        }

        public void MoveAnt(Ant ant, Location newLoc)
        {
            map[newLoc.Row, newLoc.Col, (int)Layer.Tiles] = map[ant.Row, ant.Col, (int)Layer.Tiles];
            map[ant.Row, ant.Col, (int)Layer.Tiles] = (int)Tile.Land;
            MyAnts[MyAnts.IndexOf(ant)] = new Ant(newLoc.Row, newLoc.Col, ant.Team, true);
            NotMovedAnts--;
            for (int x = -StepsIncreasRadius; x <= StepsIncreasRadius; x++)
            {
                for (int y = -StepsIncreasRadius; y <= StepsIncreasRadius; y++)
                {
                    map[test_x(newLoc.Row + x), test_y(newLoc.Col + y), (int)Layer.Steps]++;
                }
            }
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
            int count = MyAnts.Count;
            int count2 = offsets.Count;
			for(int i = 0;i<count;i++)
			{
				for(int j=0;j<count2;j++)
				{
                    if ((MyAnts[i].Col + offsets[j].Col) == loc.Col && (MyAnts[i].Row + offsets[j].Row) == loc.Row)
					{
						return true;
					}
				}
			}
			return false;
		}

        public void SendOrder(IGameState state, Ant ant, Direction direction, Location newLoc)
        {
            state.MoveAnt(ant, newLoc);
            IssueOrder(ant, direction);
        }

        public void IssueOrder(Location loc, Direction direction)
        {
            System.Console.Out.WriteLine("o {0} {1} {2}", loc.Row, loc.Col, direction.ToChar());
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

        private void AddDanger(int row, int col, DangerGrade grade)
        {
            for (int x = -DangerRadius; x <= DangerRadius; x++)
            {
                for (int y = -DangerRadius; y <= DangerRadius; y++)
                {
                    if (Math.Abs(x) + Math.Abs(y) <= DangerRadius+1)
                    {
                        map[test_x(row + x), test_y(col + y), (int)Layer.Danger] += (int)grade;
                    }
                }
            }
        }

        private void AddAttackBorder(int row, int col)
        {
            //ToDo optimieren
            int x,y;
            AttackBorder.Add(new Target(test_x(row + AttackBorderRadius), test_y(col), 0));
            AttackBorder.Add(new Target(test_x(row - AttackBorderRadius), test_y(col), 0));
            AttackBorder.Add(new Target(test_x(row), test_y(col + AttackBorderRadius), 0));
            AttackBorder.Add(new Target(test_x(row), test_y(col - AttackBorderRadius), 0));
            /*
            x=1;
            y=AttackBorderRadius;
            while (x <= AttackBorderRadius)
            {
                AttackBorder.Add(new Target(test_x(row + x), test_y(col + y), 0));
                AttackBorder.Add(new Target(test_x(row + x), test_y(col - y), 0));
                AttackBorder.Add(new Target(test_x(row - x), test_y(col + y), 0));
                AttackBorder.Add(new Target(test_x(row - x), test_y(col - y), 0));
                x++;
                y--;
            }*/
            AttackBorder.Add(new Target(test_x(row + AttackBorderRadius), test_y(col + 1), 0));
            AttackBorder.Add(new Target(test_x(row + AttackBorderRadius), test_y(col - 1), 0));
            AttackBorder.Add(new Target(test_x(row - AttackBorderRadius), test_y(col + 1), 0));
            AttackBorder.Add(new Target(test_x(row - AttackBorderRadius), test_y(col - 1), 0));
            AttackBorder.Add(new Target(test_x(row + 1), test_y(col + AttackBorderRadius), 0));
            AttackBorder.Add(new Target(test_x(row + 1), test_y(col - AttackBorderRadius), 0));
            AttackBorder.Add(new Target(test_x(row - 1), test_y(col + AttackBorderRadius), 0));
            AttackBorder.Add(new Target(test_x(row - 1), test_y(col - AttackBorderRadius), 0));
        }

        #if DEBUG
        public void SaveState(bool ToFile)
        {
            int size = 8;
            Bitmap bitmap1 = new Bitmap(Width * size, Height * size);
            Bitmap bitmap2 = new Bitmap(Width * size, Height * size);
            Bitmap bitmap3 = new Bitmap(Width * size, Height * size);
            Bitmap bitmap4 = new Bitmap(Width * size, Height * size);

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    for (int m = 0; m < size; m++)
                    {
                        for (int n = 0; n < size; n++)
                        {
                            bitmap1.SetPixel(col*size+m, row*size+n, Ants.Aim3[(Tile)map[row, col, (int)Layer.Tiles]]);
                            bitmap2.SetPixel(col * size + m, row * size + n, Color.FromArgb(map[row, col, (int)Layer.Steps] % 255, map[row, col, (int)Layer.Steps] / 10 % 255, map[row, col, (int)Layer.Steps]/100 % 255));
                            if (map[row, col, (int)Layer.Danger] == 0) bitmap3.SetPixel(col * size + m, row * size + n, Color.FromArgb(255,255,255));
                            if (map[row, col, (int)Layer.Danger] > 0) bitmap3.SetPixel(col * size + m, row * size + n, Color.FromArgb(map[row, col, (int)Layer.Danger] * 20 % 255, 0, 0));
                            else bitmap3.SetPixel(col * size + m, row * size + n, Color.FromArgb(0, -map[row, col, (int)Layer.Danger] * 20 % 255, 0));
                        }
                    }
                }
            }
            foreach (Target loc in AttackBorder){for (int m = 0; m < size; m++){
                    for (int n = 0; n < size; n++){
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Color.FromArgb(loc.Prio * 10 % 255, 0, 0));}}}
            foreach (Ant loc in MyAnts) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Ants.Aim3[Tile.MyAnt]); }}}
            foreach (Ant loc in EnemyAnts) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Ants.Aim3[Tile.EnemyAnt]); }}}
            foreach (AntHill loc in EnemyHills) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Ants.Aim3[Tile.EnemyHill]); }}}
            foreach (Location loc in DeadTiles) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Ants.Aim3[Tile.Dead]); }}}
            foreach (Target loc in FoodTargets) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Ants.Aim3[Tile.Food]); }}}
            foreach (Target loc in EnemyAntTargets) {
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Color.FromArgb(0, loc.Prio*10 % 255, 0)); }}}
            foreach (Target loc in EnemyHillTargets){
                for (int m = 0; m < size; m++) {for (int n = 0; n < size; n++) {
                        bitmap4.SetPixel(loc.Col * size + m, loc.Row * size + n, Color.FromArgb(0, 0, loc.Prio*10 % 255)); }}}

            mybitmap.Add(bitmap1);
            mybitmap.Add(bitmap2);
            mybitmap.Add(bitmap3);
            mybitmap.Add(bitmap4);
            mybitmap[mybitmap.Count - 4].Save("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\game_logs\\Tile\\log" + ((mybitmap.Count - 4)/4 +1).ToString() + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
            mybitmap[mybitmap.Count - 3].Save("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\game_logs\\Steps\\log" + ((mybitmap.Count - 3) / 4+1).ToString() + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
            mybitmap[mybitmap.Count - 2].Save("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\game_logs\\Danger\\log" + ((mybitmap.Count - 2) / 4+1).ToString() + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
            mybitmap[mybitmap.Count - 1].Save("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\game_logs\\Spezial\\log" + ((mybitmap.Count - 1) / 4+1).ToString() + ".gif", System.Drawing.Imaging.ImageFormat.Gif);

            /*
            if (ToFile == true)
            {
                for (int i = 0; i < mybitmap.Count; i++)
                {
                    mybitmap[i].Save("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\game_logs\\log" + i.ToString() + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                }
            }
            

            FileStream str = new FileStream("C:\\Users\\Daniel\\Dropbox\\Work\\Programmieren\\C#\\AI Challenge\\tools\\Log.xml", FileMode.OpenOrCreate);

            XmlSerializer serGame = new XmlSerializer(typeof(GameState));
            serGame.Serialize(str, this);

            str.Close();
            
             StreamReader sr = new StreamReader("\\Log.xml");

                    XmlSerializer serGame = new XmlSerializer(typeof(List<Game>));
                    GameList = (List<Game>)serGame.Deserialize(sr);

                    sr.Close();*/
        }
        #endif
	}
}

