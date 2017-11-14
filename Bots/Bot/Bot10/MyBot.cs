using System;
using System.Collections.Generic;
using System.Linq;

namespace Ants
{

    class MyBot : Bot
    {
        // DoTurn is run once per turn
        Defender def = new Defender();
        Explorer exp = new Explorer();
        public override void DoTurn(IGameState state)
        {
            def.Handler(state);
            exp.Handler(state);
            def.DoTurn(state);
            exp.DoTurn(state);
        }
        public static void Main(string[] args)
        {
            new Ants().PlayGame(new MyBot());
        }

    }

}