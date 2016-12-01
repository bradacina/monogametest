using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;
        private float playerMoveSpeed;

        private KeyboardState currentKeyboardState;
        private KeyboardState prevKeyboardState;
        private GamePadState currentGamePadState;
        private GamePadState prevGamePadState;
        private MouseState currentMouseState;
        private MouseState prevMouseState;

        //background
        private ParallaxingBackground bg1Background;
        private ParallaxingBackground bg2Background;
        private Texture2D mainBackground;
        private Rectangle backgroundRect;

        //enemies
        private Texture2D enemyTexture;
        private List<Enemy> enemies;
        private TimeSpan enemySpawnTime;
        private TimeSpan enemyPrevSpawnTime;

        // laser
        private Texture2D laserTexture;
        private List<Laser> laserBeams;
        private TimeSpan laserSpawnTime;
        private TimeSpan laserPrevSpawnTime;

        // explosions
        private Texture2D explosionTexture;
        private List<Explosion> explosions;

        // sounds
        private SoundEffect laserSoundEffect;
        private SoundEffectInstance laserSoundEffectInstance;

        private Random rand;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            player = new Player();
            playerMoveSpeed = 8f;

            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            this.IsMouseVisible = true;

            this.enemies = new List<Enemy>();
            this.rand = new Random();
            this.enemyPrevSpawnTime = TimeSpan.Zero;

            this.enemySpawnTime = TimeSpan.FromSeconds(1);

            this.laserBeams = new  List<Laser>();

            const float SECONDS_IN_MINUTE = 60f;
            const int RATE_OF_FIRE = 200;
            this.laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE/RATE_OF_FIRE);
            this.laserPrevSpawnTime = TimeSpan.Zero;

            this.explosions = new List<Explosion>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y +
                GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            //player.Init(Content.Load<Texture2D>("Graphics\\player"), playerPosition);

            var playerAnimation = new Animation();
            var playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");

            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 40, Color.White, 1f, true);

            player.Init(playerAnimation, playerPosition);

            bg1Background = new ParallaxingBackground();
            bg2Background = new ParallaxingBackground();

            bg1Background.Initialize(Content, "Graphics\\bgLayer1",
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);

            bg2Background.Initialize(Content, "Graphics\\bgLayer2",
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);

            mainBackground = Content.Load<Texture2D>("Graphics\\mainbackground");

            backgroundRect = new Rectangle(0,0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            enemyTexture = Content.Load<Texture2D>("Graphics\\mineAnimation");

            laserTexture = Content.Load<Texture2D>("Graphics\\laser");

            explosionTexture = Content.Load<Texture2D>("Graphics\\explosion");

            laserSoundEffect = Content.Load<SoundEffect>("Sounds\\laserFire");

            laserSoundEffectInstance = laserSoundEffect.CreateInstance();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevGamePadState = currentGamePadState;
            prevKeyboardState = currentKeyboardState;

            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentKeyboardState = Keyboard.GetState();

            UpdatePlayer(gameTime);

            UpdateEnemies(gameTime);

            UpdateLasers(gameTime);

            UpdateExplosions(gameTime);

            UpdateCollisions();

            bg1Background.Update(gameTime);
            bg2Background.Update(gameTime);

            base.Update(gameTime);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Update(gameTime);

                if (!explosions[i].Active)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void UpdateLasers(GameTime gameTime)
        {
            for (int i = 0; i < laserBeams.Count; i++)
            {
                laserBeams[i].Update(gameTime);

                if (!laserBeams[i].Active
                    || laserBeams[i].Position.X > GraphicsDevice.Viewport.Width)
                {
                    laserBeams.RemoveAt(i);
                }
            }    
        }

        private void FireLaser(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - laserPrevSpawnTime > laserSpawnTime)
            {
                laserPrevSpawnTime = gameTime.TotalGameTime;
                AddLaser();
                laserSoundEffectInstance.Play();
            }
        }

        private void AddExplosion(Vector2 position)
        {
            var explosionAnimation = new Animation();

            explosionAnimation.Initialize(explosionTexture, position, 134, 134, 12, 30, Color.White, 1f, false);

            var explosion = new Explosion();

            explosion.Init(explosionAnimation);

            explosions.Add(explosion);
        }

        private void AddLaser()
        {
            var laserAnimation = new Animation();
            laserAnimation.Initialize(laserTexture, player.Position, 46, 16, 1, 30, Color.White, 1f, true);

            var laserPosition = player.Position;
            laserPosition.X += 70;
            laserPosition.Y += 37;

            var laser = new Laser();
            laser.Init(laserAnimation, laserPosition);
            
            laserBeams.Add(laser);
        }

        private void AddEnemy()
        {
            var enemyAnimation = new Animation();

            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            var position = new Vector2(GraphicsDevice.Viewport.Width + enemyAnimation.FrameWidth/2,
                rand.Next(100, GraphicsDevice.Viewport.Height - 100));

            var enemy = new Enemy();

            enemy.Init(enemyAnimation, position);

            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - enemyPrevSpawnTime > enemySpawnTime)
            {
                enemyPrevSpawnTime = gameTime.TotalGameTime;

                AddEnemy();
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (!enemies[i].Active)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            var playerPosition = player.Position;

            playerPosition.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            playerPosition.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            if (currentKeyboardState.IsKeyDown(Keys.Left)
                || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                playerPosition.X -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right)
                || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                playerPosition.X += playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up)
                || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                playerPosition.Y -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down)
                || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                playerPosition.Y += playerMoveSpeed;
            }

            playerPosition.X = MathHelper.Clamp(playerPosition.X, 0,
                GraphicsDevice.Viewport.Width - player.Width);
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, 0,
                GraphicsDevice.Viewport.TitleSafeArea.Height - player.Height);

            player.Position = playerPosition;

            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                FireLaser(gameTime);
            }

        }

        private void UpdateCollisions()
        {
            var playerRect = new Rectangle((int)player.Position.X, (int)player.Position.Y,
                player.Width, player.Height);

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemyRect = new Rectangle(
                    (int)enemies[i].Position.X,
                    (int)enemies[i].Position.Y,
                    enemies[i].Width,
                    enemies[i].Height);

                for (int j = 0; j < laserBeams.Count; j++)
                {
                    var laserRect = new Rectangle(
                        (int)laserBeams[j].Position.X,
                        (int)laserBeams[j].Position.Y,
                        laserBeams[j].Width,
                        laserBeams[j].Height);

                    if (laserRect.Intersects(enemyRect))
                    {
                        AddExplosion(enemies[i].Position + new Vector2(-40, -30));
                        enemies[i].Health = 0;
                        laserBeams[j].Active = false;
                    }
                }

                if (playerRect.Intersects(enemyRect))
                {
                    AddExplosion(enemies[i].Position + new Vector2(-40,-30));
                    enemies[i].Health = 0;
                    player.Health -= enemies[i].Damage;
                }
            }

            if (player.Health <= 0)
            {
                player.Active = false;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            
            spriteBatch.Draw(mainBackground, backgroundRect, Color.White);
            bg1Background.Draw(spriteBatch);
            bg2Background.Draw(spriteBatch);

            player.Draw(spriteBatch);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            for (int i = 0; i < laserBeams.Count; i++)
            {
                laserBeams[i].Draw(spriteBatch);
            }

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
