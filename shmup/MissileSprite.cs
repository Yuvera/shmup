using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shmup
{
    class MissileSprite : Sprite
    {
        float missileSpeed = 2;
        bool dead = false;
        public MissileSprite(Texture2D newTxr, Vector2 newPos) : base(newTxr, newPos)
        {

        }

        public override void Update(GameTime gameTime, Point screenSize)
        {
            spritePos.X -= missileSpeed;
        }
    }
}
