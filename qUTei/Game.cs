using System.Collections.Generic;

namespace qUTei
{
    public class Game
    {
        public string Password;
        public bool DemoRec;
        public string FormName;
        public qUTeiChoice MapChoice;
        public qUTeiChoice TypeChoice;
        public List<qUTeiChoice> MoreChoices;
        public List<string> Switches;

        public Game()
        {
            MapChoice = new qUTeiChoice();
            TypeChoice = new qUTeiChoice();
            MoreChoices = new List<qUTeiChoice>();
            Switches = new List<string>();
        }
    }
}
