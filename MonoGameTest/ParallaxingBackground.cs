
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class ParallaxingBackground
    {
        private Texture2D texture;
        private Vector2[] positions;
        private int speed;
        private int bgWidth;
        private int bgHeight;

        public void Initialize(ContentManager content,
            string texturePath,
            int screenWidth,
            int screenHeight,
            int speed)
        {
            this.bgWidth = screenWidth;
            this.bgHeight = screenHeight;
            this.speed = speed;
            this.texture = content.Load<Texture2D>(texturePath);

            positions = new Vector2[screenWidth/texture.Width + 1];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(i * texture.Width, 0);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].X += this.speed;

                if (this.speed < 0)
                {
                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width*(positions.Length - 1);
                    }
                }
                else
                {
                    if (positions[i].X >= texture.Width*(positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                sb.Draw(texture,
                    new Rectangle(
                        (int)positions[i].X,
                        (int)positions[i].Y,
                        bgWidth,
                        bgHeight),
                    Color.White);
            }
        }
    }
}
