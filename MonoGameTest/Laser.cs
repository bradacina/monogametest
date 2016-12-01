
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class Laser
    {
        private Animation LaserAnimation;
        public int Width => LaserAnimation.FrameWidth;
        public int Height => LaserAnimation.FrameHeight;

        private int damage = 10;
        private float speed = 30f;
        private int range;
        public bool Active;

        public Vector2 Position;

        public void Init(Animation animation, Vector2 position)
        {
            this.LaserAnimation = animation;
            this.Position = position;
            this.Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += speed;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            LaserAnimation.Draw(sb);   
        }
    }
}
