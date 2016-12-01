
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class Enemy
    {
        private Animation EnemyAnimation;
        public int Width => EnemyAnimation.FrameWidth;
        public int Height => EnemyAnimation.FrameHeight;

        public int Health;
        public int Damage;
        private int value;
        private float speed;
        public bool Active;
        public Vector2 Position;

        public void Init(Animation animation, Vector2 position)
        {
            this.EnemyAnimation = animation;
            this.Position = position;
            this.Active = true;
            this.Health = 10;
            this.Damage = 10;
            this.speed = 6f;
            this.value = 100;
        }

        public void Update(GameTime gameTime)
        {
            this.Position.X -= this.speed;
            this.EnemyAnimation.Position = this.Position;
            this.EnemyAnimation.Update(gameTime);

            if (Health <= 0 || this.Position.X < -Width)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            EnemyAnimation.Draw(sb);
        }
    }
}
