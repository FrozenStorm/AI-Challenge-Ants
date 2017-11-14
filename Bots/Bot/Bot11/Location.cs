using System;
using System.Collections.Generic;

namespace Ants {

	public class Location : IEquatable<Location> 
    {

		/// <summary>
		/// Gets the row of this location.
		/// </summary>
		public int Row { get; private set; }

		/// <summary>
		/// Gets the column of this location.
		/// </summary>
		public int Col { get; private set; }

		public Location (int row, int col) 
        {
			this.Row = row;
			this.Col = col;
		}

		public override bool Equals (object obj) 
        {
			if (ReferenceEquals (null, obj))
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType() != typeof (Location))
				return false;

			return Equals ((Location) obj);
		}

		public bool Equals (Location other) 
        {
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (this, other))
				return true;

			return other.Row == this.Row && other.Col == this.Col;
		}

		public override int GetHashCode()
		{
			unchecked 
            {
				return (this.Row * 397) ^ this.Col;
			}
		}
	}

	public class TeamLocation : Location, IEquatable<TeamLocation> 
    {
		/// <summary>
		/// Gets the team of this ant.
		/// </summary>
		public int Team { get; private set; }

		public TeamLocation (int row, int col, int team) : base (row, col) 
        {
			this.Team = team;
		}

		public bool Equals(TeamLocation other) 
        {
			return base.Equals (other) && other.Team == Team;
		}

		public override int GetHashCode()
		{
			unchecked 
            {
				int result = this.Col;
				result = (result * 397) ^ this.Row;
				result = (result * 397) ^ this.Team;
				return result;
			}
		}
	}
	
	public class Ant : TeamLocation, IEquatable<Ant> 
    {
        /// <summary>
        /// Gets if this ant has moved.
        /// </summary>
        public bool Moved { get; set; }

		public Ant (int row, int col, int team, bool Moved) : base (row, col, team) 
        {
            this.Moved = Moved;
		}

		public bool Equals (Ant other) 
        {
			return base.Equals (other) && other.Moved == Moved;
		}
	}

	public class AntHill : TeamLocation, IEquatable<AntHill> 
    {
		public AntHill (int row, int col, int team) : base (row, col, team) 
        {
		}

		public bool Equals (AntHill other) 
        {
			return base.Equals (other);
		}
	}

    public class Target : Location
    {
        /// <summary>
        /// Get the Priority of the Target.
        /// </summary>
        public int Prio { get; set; }

        public Target(int row, int col, int Prio) : base(row, col)
        {
            this.Prio = Prio;
        }

        public static int ComparePriority(Target p1, Target p2)
        {
            if (p1.Prio == p2.Prio) return 0;
            if (p1.Prio > p2.Prio) return 1;
            else return -1;
        }

        public bool Equals(Target other)
        {
            return base.Equals(other) && other.Prio == Prio;
        }
    }
}

