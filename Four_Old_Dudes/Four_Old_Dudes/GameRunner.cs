using Four_Old_Dudes.Utils;
using System;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        [STAThreadAttribute]
        static void Main() => new GameMaster();
    }
}
