using System.Collections.Generic;

namespace qUTei
{
    public class Protocol
    {
        public string Condition;
        public List<Game> Games;

        public Protocol()
        {
            Games = new List<Game>();
        }
    }
}
