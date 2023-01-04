using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Animations;
namespace SAE_DEV_TEST
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _positionPerso;
        private AnimatedSprite _perso;
        private KeyboardState _keyboardState;
        private int _sensPersoX;
        private int _sensPersoY;
        private int _vitessePerso;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _positionPerso = new Vector2(20, 340);
            _vitessePerso = 1;
            base.Initialize();
           
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("PersoAnimation.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _keyboardState = Keyboard.GetState();

            
            _perso.Update(deltaTime); // time écoulé

            // si fleche droite
            if (_keyboardState.IsKeyDown(Keys.Right)) //&& !(_keyboardState.IsKeyDown(Keys.Up)) && !(_keyboardState.IsKeyDown(Keys.Down))) 
            {
                _perso.Play("walkingRight");
                //_sensPersoX = 1;
                _positionPerso.X += _vitessePerso;
                
            }
            // si fleche gauche
            if (_keyboardState.IsKeyDown(Keys.Left))
            {
                _perso.Play("walkingLeft");
                //_sensPersoX = -1;
                _positionPerso.X += -_vitessePerso;
            }
            // si fleche haute
            if (_keyboardState.IsKeyDown(Keys.Up))
            {
                _perso.Play("walkingUp");
                //_sensPersoY = -1;
                _positionPerso.Y += -_vitessePerso;
            }
            // si fleche bas
            if (_keyboardState.IsKeyDown(Keys.Down))
            {
                _perso.Play("walkingDown");
                //_sensPersoY = 1;
                _positionPerso.Y += _vitessePerso;
            }
            
            
            if(!(_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.Up) || _keyboardState.IsKeyDown(Keys.Down) || _keyboardState.IsKeyDown(Keys.Left)))
            {
                _sensPersoX = 0;
                _sensPersoY = 0;
                _perso.Play("idle"); // une des animations définies dans « animation.sf »
            }

            _positionPerso.X += _sensPersoX * _vitessePerso * deltaTime;
            _positionPerso.Y += _sensPersoY * _vitessePerso * deltaTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Red);
            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            _spriteBatch.Draw(_perso, _positionPerso);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}