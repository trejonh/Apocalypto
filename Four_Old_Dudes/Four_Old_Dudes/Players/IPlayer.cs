using System;
using System.Security.Cryptography.X509Certificates;

namespace Four_Old_Dudes.Players
{
    public interface IPlayer
    {
        string Name { get; set; }
        void Update();
        

    }
}
