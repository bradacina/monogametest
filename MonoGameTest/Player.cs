
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class Player
    {
        public Animation PlayerAnimation { get; set; }
        public Vector2 Position { get; set; }
        public int Health { get; set; }
        public bool Active { get; set; }
        public int Width => PlayerAnimation.FrameWidth;
        public int Height => PlayerAnimation.FrameHeight;

        public void Init(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;
            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            PlayerAnimation.Draw(sb);
        }
    }
}
