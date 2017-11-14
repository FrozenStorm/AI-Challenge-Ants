using System.Collections.Generic;

namespace Ants 
{

	public interface IGameState 
    {

        /// <summary>
        /// Map
        /// </summary>
        int[,,] map { get; set; }

		/// <summary>
		/// Gets the width of the map.
		/// </summary>
        int Width { get; set; }

		/// <summary>
		/// Gets the height of the map.
		/// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets the actual turn number.
        /// </summary>
        int TurnNumber{ get; set; }

		/// <summary>
		/// Gets the allowed load time in milliseconds.
		/// </summary>
        int LoadTime { get; set; }

		/// <summary>
		/// Gets the allowed turn time in milliseconds.
		/// </summary>
        int TurnTime { get; set; }

		/// <summary>
		/// Gets the allowed turn time remaining in milliseconds.
		/// </summary>
        int TimeRemaining { get;}

		/// <summary>
		/// Gets the ant view range radius squared.
		/// </summary>
        int ViewRadius2 { get; set; }

		/// <summary>
		/// Gets the ant attack range radius squared.
		/// </summary>
        int AttackRadius2 { get; set; }

		/// <summary>
		/// Gets the ant hill spawn radius squared.
		/// </summary>
        int SpawnRadius2 { get; set; }

		/// <summary>
		/// Gets a list of your currently visible hills.
		/// </summary>
        List<AntHill> MyHills { get; set; }

		/// <summary>
		/// Gets a list of your ants.
		/// </summary>
        List<Ant> MyAnts { get; set; }

		/// <summary>
		/// Gets a list of currently visible enemy ants.
		/// </summary>
        List<Ant> EnemyAnts { get; set; }

		/// <summary>
		/// Gets a list of currently visible enemy hills.
		/// </summary>
        List<AntHill> EnemyHills { get; set; }

		/// <summary>
		/// Gets a list of currently-visible locations where ants died last turn.
		/// </summary>
        List<Location> DeadTiles { get; set; }

		/// <summary>
		/// Gets a list of food tiles visible this turn.
		/// </summary>
        List<Target> FoodTargets { get; set; }

        /// <summary>
        /// Gets a list of enemy ants to attack.
        /// </summary>
        List<Target> EnemyAntTargets { get; set; }

        /// <summary>
        /// Gets a list of enemy hills to attack.
        /// </summary>
        List<Target> EnemyHillTargets { get; set; }

        /// <summary>
        /// Gets a list of points for preparation for a fight.
        /// </summary>
        List<Target> AttackBorder { get; set; }

		/// <summary>
		/// Gets whether <paramref name="location"/> is passable or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
		/// <seealso cref="GameState.GetIsUnoccupied"/>
		bool GetIsPassable (Location location);

		/// <summary>
		/// Gets whether <paramref name="location"/> is occupied or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
		bool GetIsUnoccupied (Location location);

		/// <summary>
		/// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
		/// </summary>
		/// <param name="location">The starting location.</param>
		/// <param name="direction">The direction to move.</param>
		/// <returns>The new location, accounting for wrap around.</returns>
		Location GetDestination (Location location, Direction direction);

		/// <summary>
		/// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The first location to measure with.</param>
		/// <param name="loc2">The second location to measure with.</param>
		/// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
		int GetDistance (Location loc1, Location loc2);

		/// <summary>
		/// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The location to start from.</param>
		/// <param name="loc2">The location to determine directions towards.</param>
		/// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
		Direction GetDirections (Location loc1, Location loc2);

        #if DEBUG
        void SaveState(bool ToFile);
        #endif

        /// <summary>
        /// the radius in which the steps for exploring is increased
        /// </summary>
        int StepsIncreasRadius { get; set; }
        /// <summary>
        /// the amount of layers of the map
        /// </summary>
        int Layers { get; set; }
        /// <summary>
        /// the radius of danger which is around each enemy
        /// </summary>
        int DangerRadius { get; set; }
        /// <summary>
        /// the radius to wait for the ants befor attacking
        /// </summary>
        int AttackBorderRadius { get; set; }
        /// <summary>
        /// The amount of not moved ants per turn
        /// </summary>
        int NotMovedAnts { get; set; }

        int GetDistancePriority(Location loc1);

        void MoveAnt(Ant ant, Location newLoc);

		bool GetIsVisible(Location loc);

        void SendOrder(IGameState state, Ant ant, Direction direction, Location newLoc);
        void IssueOrder(Location loc, Direction direction);

        int test_y(int y);
        int test_x(int x);
	}
}