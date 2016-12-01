
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class Explosion
    {
        private Animation animation;

        public bool Active => animation.Active;
        public int Width => animation.FrameWidth;
        public int Height => animation.FrameHeight;

        public void Init(Animation animation)
        {
            this.animation = animation;
        }

        public void Draw(SpriteBatch sb)
        {
            animation.Draw(sb);
        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }
    }
}
