using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// simple collision detection
// player lives / death
// counter
// UI (font, imported)


namespace shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D saucerTxr, missileTxr, backgroundTxr, bgDeathTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;

        Sprite backgroundDeath;
        Sprite backgroundSprite;
        PlayerSprite PlayerSprite;
        List<MissileSprite> missileList = new List<MissileSprite>();
        SpriteFont uiFont;
        SpriteFont bigFont;

        // this is a test comment
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            saucerTxr = Content.Load<Texture2D>("saucer");
            missileTxr = Content.Load<Texture2D>("missile");
            backgroundTxr = Content.Load<Texture2D>("background");
            uiFont = Content.Load<SpriteFont>("UI Font");
            bigFont = Content.Load<SpriteFont>("Big Font");
            bgDeathTxr = Content.Load<Texture2D>("backgroundDeath");
            backgroundSprite = new Sprite(backgroundTxr, new Vector2(0, 0));
            backgroundDeath = new Sprite(bgDeathTxr, new Vector2(0, 0));
            PlayerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
            //testMissile = new MissileSprite(missileTxr, new Vector2(340,120));
        }

        protected override void Update(GameTime gameTime)
        {
            Random RNG = new Random();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (spawnCooldown > 0)
            {
                spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            else if (PlayerSprite.playerLives > 0 && missileList.Count < 5)
            {
                missileList.Add(new MissileSprite(
                    missileTxr, 
                    new Vector2(screenSize.X, RNG.Next(0, screenSize.Y - missileTxr.Height))
                   ));

                spawnCooldown = (float)(RNG.NextDouble() * 0.8) + 0.2f;
            }

            if (PlayerSprite.playerLives > 0) PlayerSprite.Update(gameTime, screenSize);

            foreach (MissileSprite missile in missileList)
            {
                missile.Update(gameTime, screenSize);

                if (PlayerSprite.playerLives > 0 && PlayerSprite.isColliding(missile))
                {
                    missile.dead = true;
                    PlayerSprite.playerLives--;

                }
            }

            missileList.RemoveAll(missile => missile.dead);

            // testMissile.Update(gameTime, screenSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);
            if (PlayerSprite.playerLives > 0) PlayerSprite.Draw(_spriteBatch);

            foreach (MissileSprite missile in missileList)
            {
                missile.Draw(_spriteBatch);
            }

            _spriteBatch.DrawString(uiFont, ("Lives: " + PlayerSprite.playerLives), new Vector2(10, 10), Color.White);
            //testMissile.Draw(_spriteBatch);

            if (PlayerSprite.playerLives <=0)
            {
                Vector2 textSize = bigFont.MeasureString("GAME OVER");
                backgroundDeath.Draw(_spriteBatch);
                _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X /2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
