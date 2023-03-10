using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
namespace SAE_DEV_TEST
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _positionPerso;
        private AnimatedSprite _perso;
        private Vector2 _positionArme;
        private AnimatedSprite _arme;
        private KeyboardState _keyboardState;
        private int _sensPersoX;
        private int _sensPersoY;
        private int _vitessePerso;
        private int _vitesseArme;

        private int _dernierePositionPerso;

        private Matrix _tileMapMatrix;
        public const float SCALE = 2;
        private Vector2 _scalePerso;
        private Vector2 _scaleArme;

        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledMapRenderer;
        public const int TAILLE_FENETRE_X = (1720);
        public const int TAILLE_FENETRE_Y = (880);

        private TiledMapTileLayer mapLayer;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _positionPerso = new Vector2(200 * SCALE, 200 * SCALE);
            _positionArme = new Vector2(200 * SCALE, 200 * SCALE);
            _vitessePerso = 2;
            _vitesseArme = 4;



            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _graphics.PreferredBackBufferWidth = TAILLE_FENETRE_X;
            _graphics.PreferredBackBufferHeight = TAILLE_FENETRE_Y;

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tiledMap = Content.Load<TiledMap>("principale");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

            _tileMapMatrix = Matrix.CreateScale(SCALE);
            _scalePerso = new Vector2(SCALE - 1, SCALE - 1);
            _scaleArme = new Vector2(SCALE - 1, SCALE - 1);

            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("PersoAnimation.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);

            SpriteSheet spriteSheetArme = Content.Load<SpriteSheet>("ArmeAnimation.sf", new JsonContentLoader());
            _arme = new AnimatedSprite(spriteSheetArme);

            mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("Maison");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _keyboardState = Keyboard.GetState();

            _tiledMapRenderer.Update(gameTime);
            _perso.Update(deltaTime); // time écoulé
            _arme.Update(deltaTime);

            // si fleche droite
            if (_keyboardState.IsKeyDown(Keys.Right)) //&& !(_keyboardState.IsKeyDown(Keys.Up)) && !(_keyboardState.IsKeyDown(Keys.Down))) 
            {
                _dernierePositionPerso = 1;
                
                _perso.Play("walkingRight");
                ushort tx = (ushort)((_positionPerso.X / _tiledMap.TileWidth + 1) / SCALE);
                ushort ty = (ushort)((_positionPerso.Y / _tiledMap.TileHeight) / SCALE);

                if (!IsCollision(tx, ty))
                    _positionPerso.X += _vitessePerso;

            }
            // si fleche gauche
            if (_keyboardState.IsKeyDown(Keys.Left))
            {
                _dernierePositionPerso = -1;

                _perso.Play("walkingLeft");
                ushort tx = (ushort)((_positionPerso.X / _tiledMap.TileWidth - 1) / SCALE);
                ushort ty = (ushort)((_positionPerso.Y / _tiledMap.TileHeight) / SCALE);

                if (!IsCollision(tx, ty))
                    _positionPerso.X += -_vitessePerso;
            }
            // si fleche haute
            if (_keyboardState.IsKeyDown(Keys.Up))
            {
                _dernierePositionPerso = 2;

                _perso.Play("walkingUp");
                ushort tx = (ushort)((_positionPerso.X / _tiledMap.TileWidth) / SCALE);
                ushort ty = (ushort)((_positionPerso.Y / _tiledMap.TileHeight - 1) / SCALE);

                if (!IsCollision(tx, ty))
                    _positionPerso.Y += -_vitessePerso;
            }
            // si fleche bas
            if (_keyboardState.IsKeyDown(Keys.Down))
            {
                _dernierePositionPerso = -2;

                _perso.Play("walkingDown");
                ushort tx = (ushort)((_positionPerso.X / _tiledMap.TileWidth) / SCALE);
                ushort ty = (ushort)((_positionPerso.Y / _tiledMap.TileHeight + 1) / SCALE);

                if (!IsCollision(tx, ty))
                    _positionPerso.Y += _vitessePerso;
            }


            if (!(_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.Up) || _keyboardState.IsKeyDown(Keys.Down) || _keyboardState.IsKeyDown(Keys.Left)))
            {
                _sensPersoX = 0;
                _sensPersoY = 0;
                _perso.Play("idle"); // une des animations définies dans « animation.sf »
            }

            _positionPerso.X += _sensPersoX * _vitessePerso * deltaTime;
            _positionPerso.Y += _sensPersoY * _vitessePerso * deltaTime;


            // si bouton F n'est PAS appuyer alors arme suivre le joueur
            if (_keyboardState.IsKeyUp(Keys.F))
            {
                _arme.Color = Color.Transparent;
                _positionArme = _positionPerso;
            }

            // bouton f pour attaquer/lancer shuriken à droite
            if (_keyboardState.IsKeyDown(Keys.F) && _dernierePositionPerso == 1) 
            {
                _arme.Color = Color.White;
                _arme.Play("animation0");
                ushort tx = (ushort)((_positionArme.X / _tiledMap.TileWidth + 1) / SCALE);
                ushort ty = (ushort)((_positionArme.Y / _tiledMap.TileHeight) / SCALE);

                if (!IsCollision(tx, ty))
                {
                    _positionArme.X += _vitesseArme;
                }
                if (IsCollision(tx, ty))
                {
                    _positionArme = _positionPerso;
                }
            }
            // bouton f pour attaquer/lancer shuriken à gauche
            if (_keyboardState.IsKeyDown(Keys.F) && _dernierePositionPerso == -1)
            {
                _arme.Color = Color.White;
                _arme.Play("animation0");
                ushort tx = (ushort)((_positionArme.X / _tiledMap.TileWidth - 1) / SCALE);
                ushort ty = (ushort)((_positionArme.Y / _tiledMap.TileHeight) / SCALE);

                if (!IsCollision(tx, ty))
                {
                    _positionArme.X -= _vitesseArme;
                }
                if (IsCollision(tx, ty))
                {
                    _positionArme = _positionPerso;
                }

            }
            // bouton f pour attaquer/lancer shuriken en haut
            if (_keyboardState.IsKeyDown(Keys.F) && _dernierePositionPerso == 2)
            {
                _arme.Color = Color.White;
                _arme.Play("animation0");
                ushort tx = (ushort)((_positionArme.X / _tiledMap.TileWidth) / SCALE);
                ushort ty = (ushort)((_positionArme.Y / _tiledMap.TileHeight - 1) / SCALE);

                if (!IsCollision(tx, ty))
                {
                    _positionArme.Y += -_vitesseArme;
                }
                if (IsCollision(tx, ty))
                {
                    _positionArme = _positionPerso;
                }
            }
            // bouton f pour attaquer/lancer shuriken en bas
            if (_keyboardState.IsKeyDown(Keys.F) && _dernierePositionPerso == -2)
            {
                _arme.Color = Color.White;
                _arme.Play("animation0");
                    ushort tx = (ushort)((_positionArme.X / _tiledMap.TileWidth) / SCALE);
                    ushort ty = (ushort)((_positionArme.Y / _tiledMap.TileHeight + 1) / SCALE);

               if (!IsCollision(tx, ty))
               {
                   _positionArme.Y += _vitesseArme;
               }
               if (IsCollision(tx, ty))
               {
                   _positionArme = _positionPerso;
               }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            _tiledMapRenderer.Draw(viewMatrix: _tileMapMatrix);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_perso, _positionPerso, 0, _scalePerso);
            _spriteBatch.Draw(_arme, _positionArme, 0, _scaleArme);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private bool IsCollision(ushort x, ushort y)
        {
            // définition de tile qui peut être null (?)
            TiledMapTile? tile;
            if (mapLayer.TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }

        

    }


}