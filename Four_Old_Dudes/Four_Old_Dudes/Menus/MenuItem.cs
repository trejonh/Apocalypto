using SFML.Graphics;
namespace Four_Old_Dudes.Menus
{
    public abstract class MenuItem : System.Drawable
    {
        protected Text _itemText { get; set; }
        protected Shape _itemShape { get; set; }
        
        public override void Draw()
        {
            _winInstance.Draw(_itemShape);
            _winInstance.Draw(_itemText);
        }
    }
}
