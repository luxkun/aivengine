/*

Luciano Ferraro

*/

namespace Aiv.Engine
{
    public class Camera : GameObject
    {
        public GameObject LinkedObject { get; set; } = null;

        public float X { get; set; }

        public float Y { get; set; }

        public override void Update()
        {
            base.Update();
            if (LinkedObject != null)
            {
                X = LinkedObject.X;
                Y = LinkedObject.Y;
            }
        }
    }
}