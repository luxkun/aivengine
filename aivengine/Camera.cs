namespace Aiv.Engine
{
    public class Camera : GameObject
    {

        public bool IsDrawable(GameObject gameObject)
        {
            // TODO: do it
            return true;
        }

        public override void Update()
        {
            base.Update();
            if (LinkedObject != null)
            {
                X = LinkedObject.x;
                Y = LinkedObject.y;
            }
        }

        public GameObject LinkedObject
        {
            get; set;
        } = null;

        public int X { get; set; }

        public int Y { get; set; }
    }
}