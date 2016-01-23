using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WordsAppGame.Core
{
    public class GameObject
    {
        Texture2D texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color color;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    texture.Width,
                    texture.Height);
            }
        }

        public GameObject(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.Position = position;
        }

        public GameObject(Texture2D texture, Vector2 position, Color col)
        {
            this.texture = texture;
            this.Position = position;
            this.color = col;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, this.color);
        }

        public Texture2D getTexture()
        {
            if (this.texture != null)
                return this.texture;
            else
                return null;
        }
    }
}
