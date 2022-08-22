using System.Collections.Generic;

namespace qUTei
{
    public class qUTeiChoice
    {
        public string FormInstructions;
        public string CmdLineParamter;
        public List<CmdLineOption> CmdLineOptions;

        public qUTeiChoice()
        {
            CmdLineOptions = new List<CmdLineOption>();
        }
    }
}
