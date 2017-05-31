using Four_Old_Dudes.System;
using System.Collections.Generic;

namespace Four_Old_Dudes.Menus
{
    public abstract class Menu : Drawable
    {
        protected List<MenuItem> _menuItems { get; set; }

        public void AddMenuItem(MenuItem item) => _menuItems.Add(item);

        public override void Draw() => _menuItems.ForEach(delegate(MenuItem item) { item.Draw(); });
    }
}
