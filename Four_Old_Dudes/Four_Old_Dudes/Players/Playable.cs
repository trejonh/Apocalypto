using System;
using System.Collections.Generic;

namespace Four_Old_Dudes.Players
{
    class Playable:IPlayer
    {
        public string Name { get; set; }
        public double Health { get; set; }
        //public List<Item> MyItems;
        public Playable(string name)
        {
            Name = name;
        }

        public void Update()
        {
            
        }

    }
}
