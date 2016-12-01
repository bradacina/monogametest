
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    class Animation
    {
        private Texture2D spriteStrip;
        private int elapsedTime;
        private int currentFrame;
        private float scale;
        private int frameTime;
        private int frameCount;
        private Color color;
        private Rectangle sourceRectangle;
        private Rectangle destRectangle;
        public int FrameWidth;
        public int FrameHeight;
        public bool Active;
        public bool Looping;
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight,
            int frameCount, int frameTime, Color color, float scale, bool looping)
        {
            this.spriteStrip = texture;
            this.Position = position;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            this.color = color;
            this.scale = scale;
            this.Looping = looping;

            this.Active = true;
            this.elapsedTime = 0;
            this.currentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }

            elapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                elapsedTime = 0;

                currentFrame++;

                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (!Looping)
                    {
                        Active = false;
                    }
                }
            }

            sourceRectangle = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            destRectangle = new Rectangle((int)(Position.X),
                (int)(Position.Y),
                (int)(FrameWidth * scale),
                (int)(FrameHeight * scale));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(spriteStrip, destRectangle, sourceRectangle, color);
        }
    }
}
