using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

// bigger particles
// particles when player dies
// sound
// missle accelleration
//progression / timer

//background music

namespace shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D saucerTxr, missileTxr, backgroundTxr, bgDeathTxr, particleTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;
        float playTime = 0;

        Sprite backgroundDeath;
        Sprite backgroundSprite;
        PlayerSprite PlayerSprite;
        List<MissileSprite> missileList = new List<MissileSprite>();
        List<particleSprite> particleList = new List<particleSprite>();
        SpriteFont uiFont;
        SpriteFont bigFont;
        SoundEffect shipExplodeSnd;
        SoundEffect missileExplodeSnd;
        Song BGmusic;

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
            particleTxr = Content.Load<Texture2D>("particle");
            uiFont = Content.Load<SpriteFont>("UI Font");
            bigFont = Content.Load<SpriteFont>("Big Font");
            bgDeathTxr = Content.Load<Texture2D>("backgroundDeath");
            backgroundSprite = new Sprite(backgroundTxr, new Vector2(0, 0));
            backgroundDeath = new Sprite(bgDeathTxr, new Vector2(0, 0));
            PlayerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
            shipExplodeSnd = Content.Load<SoundEffect>("shipExplode");
            missileExplodeSnd = Content.Load<SoundEffect>("missileExplode");
            BGmusic = Content.Load<Song>("music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGmusic);
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

            else if (PlayerSprite.playerLives > 0 && missileList.Count < (Math.Min(playTime, 120f) / 120f) * 20000f + 200f)
            {
                missileList.Add(new MissileSprite(
                    missileTxr, 
                    new Vector2(screenSize.X, RNG.Next(0, screenSize.Y - missileTxr.Height)),
                    (Math.Min(playTime, 120f)/120f) * 20000f + 200f
                   ));

                spawnCooldown = (float)(RNG.NextDouble() * 0.8) + 0.2f;
            }

            if (PlayerSprite.playerLives > 0)
            {
                PlayerSprite.Update(gameTime, screenSize);
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            foreach (MissileSprite missile in missileList)
            {
                missile.Update(gameTime, screenSize);

                if (PlayerSprite.playerLives > 0 && PlayerSprite.isColliding(missile))
                {
                    for(int i = 0; i < 16; i++) particleList.Add(new particleSprite(particleTxr, new Vector2(PlayerSprite.spritePos.X + (missileTxr.Width / 2) - (particleTxr.Width / 2), PlayerSprite.spritePos.Y + (missileTxr.Height / 2) - (particleTxr.Height / 2))));

                    missile.dead = true;
                    PlayerSprite.playerLives--;
                    missileExplodeSnd.Play();

                    if (PlayerSprite.playerLives == 0)
                    {
                        for (int i = 0; i < 16; i++) particleList.Add(new particleSprite(particleTxr, new Vector2(PlayerSprite.spritePos.X + (missileTxr.Width / 2) - (particleTxr.Width / 2), PlayerSprite.spritePos.Y + (missileTxr.Height / 2) - (particleTxr.Height / 2))));
                        shipExplodeSnd.Play();
                        MediaPlayer.Stop();
                    }

                }
            }

            foreach (particleSprite particle in particleList) particle.Update(gameTime, screenSize);

            missileList.RemoveAll(missile => missile.dead);
            particleList.RemoveAll(particle => particle.currentlife <= 0 );

            // testMissile.Update(gameTime, screenSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);
            if (PlayerSprite.playerLives > 0) PlayerSprite.Draw(_spriteBatch);

            foreach (MissileSprite missile in missileList)
            {
                missile.Draw(_spriteBatch);
            }

            foreach (particleSprite particle in particleList)
            {
                particle.Draw(_spriteBatch);
            }

            _spriteBatch.DrawString(
                uiFont, 
                "Lives: " + PlayerSprite.playerLives,
                new Vector2(14, 14), 
                Color.Black
                );

            _spriteBatch.DrawString(
           uiFont,
           "Lives: " + PlayerSprite.playerLives,
           new Vector2(10, 10),
           Color.White
           );

            _spriteBatch.DrawString(
                uiFont,
                "Time: " + Math.Round(playTime),
                new Vector2(114, 14),
                Color.Black
                );

            _spriteBatch.DrawString(
           uiFont,
           "Time: " + Math.Round(playTime),
           new Vector2(110, 10),
           Color.White
           );

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
