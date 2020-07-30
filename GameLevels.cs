using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;

namespace Assignment
{
    class GameLevels
    {
    }
    class Dir
    {
        public static string dir = "";
        public static Random rnd = new Random();

        public static ImageBackground background;
        public static Texture2D texBackground;
        public static int leftBoundary = 0;
        public static int rightBoundary = 1395;
        public static int topBoundary = 5;
        public static int bottomBoundary = 949;

        public static int enemy1Count = 0;
        public static int enemy2Count = 0;
        public static int enemy3Count = 0;
    }

    // -------------------------------------------------------- Game State Start Screen -----------------------------------------------------------------------
    class GameLevel_0_Default : RC_GameStateParent
    {
        SpriteFont titleFont;
        Texture2D texTitle;
        Sprite3 title;

        Texture2D texpressStart;
        Sprite3 pressStart;

        SoundEffect introSound;
        SoundEffectInstance intro;

        Texture2D texStartSub;
        Sprite3 startSub;
        int movingXLocation = -20;

        public override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphicsDevice);

            // put a font to tell play to press space to start
            //Dir.texBackground = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Press_Space.png");

            float xLocation = Dir.rightBoundary / 2;

            texpressStart = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Press_Space.png");
            texStartSub = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\navio-hi.png");
            titleFont = Content.Load<SpriteFont>("display");
            texTitle = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\title.png");
            introSound = Content.Load<SoundEffect>("intro_theme");
            intro = introSound.CreateInstance();

            pressStart = new Sprite3(true, texpressStart, xLocation-302, Dir.topBoundary+400);
            startSub = new Sprite3(true, texStartSub, movingXLocation, Dir.bottomBoundary / 2);
            title = new Sprite3(true, texTitle, xLocation - 300, 50);

            pressStart.setWidthHeight(pressStart.getWidth()*2, pressStart.getHeight()*2);
            title.setWidthHeight((title.getWidth() * 3)/2, (title.getHeight() * 3)/2);
            startSub.setWidthHeight(startSub.getWidth() / 2, startSub.getWidth() / 2);

            pressStart.setColor(Color.Aqua);
            startSub.setColor(Color.GreenYellow);
        }

        public override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                gameStateManager.setLevel(1);
                intro.Stop();
            }

            startSub.setPosX(startSub.getPosX() + 5);
            if (startSub.getPosX() > Dir.rightBoundary)
                startSub.setPosX(-265);

            intro.Play();
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            Dir.background.Draw(spriteBatch);
            title.Draw(spriteBatch);
            pressStart.Draw(spriteBatch);
            startSub.Draw(spriteBatch);
            spriteBatch.End();
        }
    }

    // -------------------------------------------------------- Game level 1 ----------------------------------------------------------------------------------
    class GameLevel_1 : RC_GameStateParent
    {
        public bool showBoundary = false;
        public static int timer = 0;

        public SpriteList enemyList;

        public static Sprite3 player;
        public static Texture2D texPlayer;
        public static int playerXLocation;
        public static int playerYLocation;

        public static Sprite3 enemy1;
        public static Texture2D texEnemy1;
        public static int enemyXLocation = 0;
        public static int enemyYLocation;
        public static bool enemy1Collision;
        
        public static Sprite3 enemy2;
        public static Texture2D texEnemy2;
        public static int enemy2XLocation;
        public static bool enemy2Collision;

        public static Sprite3 enemy3;
        public static int enemy3YLocation;
        public static bool enemy3Collision;

        public Sprite3 missile;
        public Texture2D texMissile;
        public int missileYDirection = 30;
        public int missileCount = 5;
        public bool missileMoving = false;
        public bool drawMissile = false;

        public Texture2D texMissileSmoke;
        public Texture2D tex;
        public ParticleSystem missileSmoke;
        public Rectangle rectangle;
        public int smokeTimer = 0;

        public Sprite3 explosionAnimation;
        public Texture2D texExplostionAnimation;
        public Vector2[] anim = new Vector2[16];
        public bool collision1 = false;
        public bool collision2 = false;
        public bool collision3 = false;
        public float currXMissile;
        public float currYMissile;
        public int explostionTimer = 0;

        public SoundEffect explostionSound;
        public LimitSound exLimSound;

        public int score = 0;
        public SpriteFont scoreFont;
        public String status;
        public SpriteFont helpFont;

        public override void LoadContent()
        {
            enemyList = new SpriteList(20);

            playerXLocation = Dir.rnd.Next(0, 1000);
            playerYLocation = 880;
            enemyXLocation = -20;
            enemyYLocation = Dir.rnd.Next(213, 600);
            enemy2XLocation = Dir.rnd.Next(Dir.rightBoundary - 300, Dir.rightBoundary);
            enemy3YLocation = enemyYLocation + Dir.rnd.Next(100, 200);

            // Image Locations
            Dir.texBackground = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Sea04.png");
            texPlayer = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\ship-md.png");
            texEnemy1 = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Merchant1.png");
            texEnemy2 = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Merchant2.png");
            texMissile = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Torpedo5up.png");
            texMissileSmoke = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Particle1.png");
            texExplostionAnimation = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\explode1.png");
            
            scoreFont = Content.Load<SpriteFont>("display");
            helpFont = Content.Load<SpriteFont>("helpText");
            explostionSound = Content.Load<SoundEffect>("bazooka_fire");
            exLimSound = new LimitSound(explostionSound, 3);

            // Set Sprites
            Dir.background = new ImageBackground(Dir.texBackground, Color.White, graphicsDevice);
            player = new Sprite3(true, texPlayer, playerXLocation, playerYLocation);
            enemy1 = new Sprite3(true, texEnemy1, enemyXLocation, enemyYLocation);
            enemy2 = new Sprite3(true, texEnemy2, enemy2XLocation, enemyYLocation);
            enemy3 = new Sprite3(true, texEnemy1, enemyXLocation, enemy3YLocation);            
            missile = new Sprite3(true, texMissile, playerXLocation, playerYLocation);
            explosionAnimation = new Sprite3(true, texExplostionAnimation, 0, playerYLocation);

            // Adjusting sprite image sizes to suit my taste
            player.setWidthHeight(210, 64);
            enemy1.setWidthHeight(256, 64);
            enemy2.setWidthHeight(enemy1.getWidth() / 2, 64);
            enemy3.setWidthHeight(256, 64);
            missile.setWidthHeight(missile.getWidth() / 2, missile.getHeight() / 2);

            HealthBarAttached h1 = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            h1.offset = new Vector2(0, -1); // one pixel above the bounding box
            h1.gapOfbar = 2;
            enemy1.hitPoints = 10;
            enemy1.maxHitPoints = 10;
            enemy1.attachedRenderable = h1;

            HealthBarAttached h2 = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            h2.offset = new Vector2(0, -1); // one pixel above the bounding box
            h2.gapOfbar = 2;
            enemy2.hitPoints = 20;
            enemy2.maxHitPoints = 20;
            enemy2.attachedRenderable = h2;

            HealthBarAttached h3 = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            h3.offset = new Vector2(0, -1); // one pixel above the bounding box
            h3.gapOfbar = 2;
            enemy3.hitPoints = 10;
            enemy3.maxHitPoints = 10;
            enemy3.attachedRenderable = h3;

            // Add to sprite list
            enemyList.addSpriteReuse(enemy1);
            enemyList.addSpriteReuse(enemy2);
            enemyList.addSpriteReuse(enemy3);

            explosionAnimation.setWidthHeightOfTex(1024, 64);
            explosionAnimation.setXframes(16);
            explosionAnimation.setYframes(1);
            explosionAnimation.setWidthHeight(90, 90);
            for (int moveX = 0; moveX <= 15; moveX++)
            {
                anim[moveX].X = moveX; anim[moveX].Y = 0;
            }
            explosionAnimation.setAnimationSequence(anim, 0, 15, 4);
            explosionAnimation.setAnimFinished(0);
            explosionAnimation.animationStart();

            drawSmoke();
        }

        public override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            timer += 1;
            explosionAnimation.animationTick(gameTime);
            missileSmoke.Update(gameTime);

            if (keyState.IsKeyDown(Keys.Left))
            {
                if (player.getPosX() > Dir.leftBoundary)
                {
                    player.setPosX(player.getPosX() - 4);                   // Moves player 4 units left
                    player.setFlip(SpriteEffects.FlipHorizontally);
                }
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                if (player.getPosX() < (Dir.rightBoundary - (player.getWidth() - 5)))
                {
                    player.setPosX(player.getPosX() + 4);                   // Moves player 4 units right
                    player.setFlip(SpriteEffects.None);
                }
            }
            enemy1.setPosX(enemy1.getPosX() + Dir.rnd.Next(1, 3));
            enemy2.setPosX(enemy2.getPosX() - Dir.rnd.Next(1, 3));
            enemy3.setPosX(enemy3.getPosX() + Dir.rnd.Next(2, 4));

            // If enemy runs off screen, reset position to random x postion
            if (enemy1.getPosX() > Dir.rightBoundary + 60)
            {
                enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
            }
            if (enemy2.getPosX() < Dir.leftBoundary - (enemy2.getWidth() + 50))
            {
                enemy2.setPos(Dir.rnd.Next(Dir.rightBoundary, Dir.rightBoundary + 200), Dir.rnd.Next(200, 700));
            }
            if (enemy3.getPosX() > Dir.rightBoundary + 60)
            {
                enemy3.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
            }

            if (keyState.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space))
            {
                missile.setPos(player.getPosX() + player.getWidth() / 2, player.getPosY() - missile.getHeight());
                missile.active = true;
                drawMissile = true;
                drawSmoke();
                if (missileCount > 0)
                {
                    missileCount--;
                }
            }
            
            if (missileCount == 0)
            {
                missileCount = 5;
            }
            
            if (missile.getPosY() < 185 + missile.getHeight())
            {
                missile.setPos(player.getPos());
                drawMissile = false;
                missileSmoke.deActivate();
            }

            enemy1Collision = missile.collision(enemy1);
            enemy2Collision = missile.collision(enemy2);
            enemy3Collision = missile.collision(enemy3);

            explostionTimer += 1;
            if (enemy1Collision)
            {
                enemy1.hitPoints = enemy1.hitPoints - 10;
                currXMissile = enemy1.getPosX() + enemy1.getWidth() / 2;
                currYMissile = enemy1.getPosY() - enemy1.getHeight() / 2;
                if (enemy1.hitPoints <= 0)
                {
                    collision1 = true;
                    Dir.enemy1Count++;
                    enemy1.hitPoints = 10;
                }
                getSetMissile();
            }
            if (enemy2Collision)
            {
                missileSmoke.deActivate();
                enemy2.hitPoints = enemy2.hitPoints - 10;
                drawMissile = false;
                if (enemy2.hitPoints <= 0)
                {
                    currXMissile = enemy2.getPosX() + enemy2.getWidth() / 2;
                    currYMissile = enemy2.getPosY() - enemy2.getHeight() / 2;
                    collision2 = true;
                    Dir.enemy2Count++;
                    enemy2.hitPoints = 20;
                }
                getSetMissile();
            }
            if (enemy3Collision)
            {
                enemy3.hitPoints = enemy3.hitPoints - 10;
                currXMissile = enemy3.getPosX() + enemy3.getWidth() / 2;
                currYMissile = enemy3.getPosY() - enemy3.getHeight() / 2;
                if (enemy3.hitPoints <= 0)
                {
                    collision3 = true;
                    Dir.enemy3Count++;
                    enemy3.hitPoints = 10;
                }
                getSetMissile();
            }

            // Toggle showing Boundary Box
            if (keyState.IsKeyDown(Keys.B) && prevKeyState.IsKeyUp(Keys.B))
            {
                showBoundary = !showBoundary;
            }

            if (keyState.IsKeyDown(Keys.P) && !prevKeyState.IsKeyDown(Keys.P))
            {
                gameStateManager.pushLevel(3);
            }

            if (keyState.IsKeyDown(Keys.F1) && !prevKeyState.IsKeyDown(Keys.F1))
            {
                gameStateManager.pushLevel(5);
            }

            if (score >= 300)
            {
                if (timer % 181 == 180)
                {
                    gameStateManager.setLevel(2);
                }
            }
            exLimSound.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (score >= 300)
            {
                status = "NEXT LEVEL!";
            } 
            else
            {
                status = "Missile Left: " + missileCount;
            }

            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin();

            Dir.background.Draw(spriteBatch);
            player.draw(spriteBatch);

            enemyList.Draw(spriteBatch);
            missileSmoke.Draw(spriteBatch);
            spriteBatch.DrawString(scoreFont, "Score: " + score + "/300", new Vector2(20, 20), Color.Black);     // Draw Score on top left
            spriteBatch.DrawString(scoreFont, status, new Vector2(1100, 20), Color.Black);              // Draw missile count

            if (drawMissile == true  && missileCount != 0)
            {
                missile.draw(spriteBatch);
                missile.setPosY(missile.getPosY() - 3);
                missileSmoke.sysPos = new Vector2(missile.getPosX()+missile.getWidth()/2, missile.getPosY()+missile.getHeight());
            }
            
            if (collision1 == true)
            {
                drawMissile = false;
                drawExplosion(currXMissile, currYMissile);
                enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                if (explostionTimer == 110)
                {
                    collision1 = false;
                    score += 50;
                    explostionTimer = 0;
                    missileSmoke.deActivate();
                }
            }
            if (collision2 == true)
            {
                drawMissile = false;
                drawExplosion(currXMissile, currYMissile);
                enemy2.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                if (explostionTimer == 80)
                {
                    score += 100;
                    collision2 = false;
                    explostionTimer = 0;
                    missileSmoke.deActivate();
                }
            }
            if (collision3 == true)
            {
                drawMissile = false;
                drawExplosion(currXMissile, currYMissile);
                enemy3.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                if (explostionTimer == 80)
                {
                    score += 50;
                    collision3 = false;
                    explostionTimer = 0;
                    missileSmoke.deActivate();
                }
            }

            if (showBoundary)
            {
                //LineBatch.drawLineRectangle(spriteBatch, new Rectangle(1, 0, rightBoundary+4, bottomBoundary), Color.Blue);
                player.drawInfo(spriteBatch, Color.Yellow, Color.Red);
                missile.drawInfo(spriteBatch, Color.Black, Color.Red);
                enemyList.drawInfo(spriteBatch, Color.Yellow, Color.Purple);
            }
            spriteBatch.End();
        }

        public void drawExplosion(float currXMissile, float currYMissile)
        {
            explosionAnimation.setPos(currXMissile, currYMissile);
            explosionAnimation.draw(spriteBatch);
        }

        public void getSetMissile()
        {
            missile.reset();
            missile.active = false;
            missile.setPos(player.getPosX(), player.getPosY());
            exLimSound.playSoundIfOk();
            explostionTimer = 0;
        }

        public void drawSmoke()
        {
            missileSmoke = new ParticleSystem(new Vector2(missile.getPosX(), missile.getPosY()+missile.getHeight()+60), 400, 999, 108);
            tex = texMissileSmoke;
            missileSmoke.setMandatory1(tex, new Vector2(15, 15), new Vector2(10, 10), Color.DimGray, new Color(200, 200, 200, 50));
            missileSmoke.setMandatory2(-1, 1, 1, 5, 0);
            rectangle = new Rectangle(0, 0, 1400, 900);
            missileSmoke.setMandatory3(120, rectangle);
            missileSmoke.setMandatory4(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            missileSmoke.randomDelta = new Vector2(0.01f, 0.01f);
            missileSmoke.setDisplayAngle = true;
            missileSmoke.Origin = 0;
        }
    }

    // -------------------------------------------------------- Game level 2 ----------------------------------------------------------------------------------
    class GameLevel_2 : RC_GameStateParent
    {
        public static Texture2D texMouse;

        public ImageBackground backgroundLevel2;
        public Texture2D texBackgroundLevel2;

        public static Sprite3 player;
        public static Texture2D texPlayer;
        public static int playerXLocation;
        public static int playerYLocation;

        public static Sprite3 enemy1;
        public static Texture2D texEnemy1;
        public static int enemyXLocation = 0;
        public static int enemyYLocation;
        public static bool enemy1Collision;

        public static Sprite3 enemy2;
        public static Texture2D texEnemy2;
        public static int enemy2XLocation;
        public static bool enemy2Collision;

        public Sprite3 missile;
        public Texture2D texMissile;
        public int missileYDirection = 30;
        public int missileCount = 5;
        public bool missileMoving = false;
        public bool drawMissile = false;

        public Sprite3 mine;
        public Texture2D texMine;
        public bool drawMines = false;
        public bool mineCollisionToggle = false;
        public bool mineCollisionToggle2 = false;
        bool mineCollission = false;
        bool mineCollission2 = false;
        public int mineCount = 0;
        public Sprite3 mineExplostionAnimation;
        public Texture2D texMineExplostion;
        public Vector2[] mineAnim = new Vector2[16];

        public Sprite3 explosionAnimation;
        public Texture2D texExplostionAnimation;
        public Vector2[] anim = new Vector2[16];
        public bool collision1 = false;
        public bool collision2 = false;
        public bool collision3 = false;
        public float currXMissile;
        public float currYMissile;
        public int explostionTimer = 0;

        public SoundEffect explostionSound;
        public LimitSound exLimSound;

        public Texture2D texMissileSmoke;
        public Texture2D tex;
        public ParticleSystem missileSmoke;
        public Rectangle rectangle;

        public int smokeTimer = 0;
        public Sprite3 anchor;
        public Texture2D texAnchor;
        public int anchorYLocation;
        public int anchorXLocation;
        public bool anchorCollision = false;
        public bool anchorDraw = false;

        public SpriteList weaponList;

        public Sprite3 life;
        public Texture2D texLife;
        public bool dol = false;
        public int dolTimer = 0;

        public bool showBB = false;
        public static int timer = 0;
        public int score = 0;
        public SpriteFont scoreFont;
        public String status;
        public SpriteFont helpFont;

        public Vector2 minePos;
        public Vector2 mineCurrPos;
        public override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphicsDevice);
            weaponList = new SpriteList(20);

            playerXLocation = Dir.rnd.Next(0, 1000);
            playerYLocation = 880;
            enemyXLocation = -20;
            enemyYLocation = Dir.rnd.Next(213, 600);
            enemy2XLocation = Dir.rnd.Next(Dir.rightBoundary - 300, Dir.rightBoundary);
            anchorXLocation = Dir.rnd.Next(Dir.leftBoundary + 100, Dir.rightBoundary - 100);
            anchorYLocation = 175;

            texBackgroundLevel2 = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Sea04.png");
            texPlayer = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\ship-md.png");
            texEnemy1 = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Merchant1.png");
            texEnemy2 = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Merchant2.png");
            texMissile = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Torpedo5up.png");
            texMissileSmoke = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Particle1.png");
            texExplostionAnimation = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\explodeWater1024x64.png");
            texMineExplostion = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\explode1.png");
            texMouse = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Mouse.png");
            texAnchor = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\anchor-th.png");
            texLife = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\life-preserver-th.png");
            texMine = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\water_mine.png");
            scoreFont = Content.Load<SpriteFont>("display");
            helpFont = Content.Load<SpriteFont>("helpText");
            explostionSound = Content.Load<SoundEffect>("bazooka_fire");
            exLimSound = new LimitSound(explostionSound, 3);

            backgroundLevel2 = new ImageBackground(texBackgroundLevel2, Color.White, graphicsDevice);
            player = new Sprite3(true, texPlayer, playerXLocation, playerYLocation);
            enemy1 = new Sprite3(true, texEnemy1, enemyXLocation, enemyYLocation);
            enemy2 = new Sprite3(true, texEnemy2, enemy2XLocation, enemyYLocation);
            explosionAnimation = new Sprite3(true, texExplostionAnimation, 0, playerYLocation);
            mineExplostionAnimation = new Sprite3(true, texMineExplostion, 0, playerYLocation);
            mine = new Sprite3(true, texMine, 0, 0);
            mine.setHSoffset(new Vector2(296, 313));
            weaponList.addSpriteReuse(mine);
            life = new Sprite3(true, texLife, Dir.rnd.Next(Dir.leftBoundary + 200, Dir.rightBoundary - 200), Dir.rnd.Next(300, 600));
            life.setActiveAndVisible(false);

            for (int i = 1; i < 3; i++)
            {
                anchor = new Sprite3(true, texAnchor, anchorXLocation, anchorYLocation);
                anchor.setWidthHeight(50, 50);
                weaponList.addSpriteReuse(anchor);
            }
            for (int m = 1; m < 6; m++)
            {
                missile = new Sprite3(true, texMissile, playerXLocation, playerYLocation);
                missile.setWidthHeight(missile.getWidth() / 2, missile.getHeight() / 2);
                weaponList.addSpriteReuse(missile);
            }

            player.setColor(Color.Red);

            // Adjusting sprite image sizes to suit my taste
            player.setWidthHeight(210, 64);
            enemy1.setWidthHeight(256, 64);
            enemy2.setWidthHeight(enemy1.getWidth() / 2, 64);
            life.setWidthHeight(48, 48);
            mine.setWidthHeight(40, 45);

            // animation for torpedo explosion
            explosionAnimation.setWidthHeightOfTex(1024, 64);
            explosionAnimation.setXframes(16);
            explosionAnimation.setYframes(1);
            explosionAnimation.setWidthHeight(80, 80);
            for (int moveX = 0; moveX <= 15; moveX++)
            {
                anim[moveX].X = moveX; anim[moveX].Y = 0;
            }
            explosionAnimation.setAnimationSequence(anim, 0, 15, 3);
            explosionAnimation.setAnimFinished(0);
            explosionAnimation.animationStart();

            // animation for mine explostion
            mineExplostionAnimation.setWidthHeightOfTex(1024, 64);
            mineExplostionAnimation.setXframes(16);
            mineExplostionAnimation.setYframes(1);
            mineExplostionAnimation.setWidthHeight(80, 80);
            for (int moveX = 0; moveX <= 15; moveX++)
            {
                mineAnim[moveX].X = moveX; mineAnim[moveX].Y = 0;
            }
            mineExplostionAnimation.setAnimationSequence(mineAnim, 0, 15, 4);
            mineExplostionAnimation.setAnimFinished(0);
            mineExplostionAnimation.animationStart();

            HealthBarAttached h1 = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            h1.offset = new Vector2(0, -1); // one pixel above the bounding box
            h1.gapOfbar = 2;
            enemy1.hitPoints = 10;
            enemy1.maxHitPoints = 10;
            enemy1.attachedRenderable = h1;

            HealthBarAttached h2 = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            h2.offset = new Vector2(0, -1); // one pixel above the bounding box
            h2.gapOfbar = 2;
            enemy2.hitPoints = 20;
            enemy2.maxHitPoints = 20;
            enemy2.attachedRenderable = h2;

            HealthBarAttached playerHP = new HealthBarAttached(Color.Aquamarine, Color.Green, Color.Red, 9, true);
            playerHP.offset = new Vector2(0, -1); // one pixel above the bounding box
            playerHP.gapOfbar = 2;
            player.hitPoints = 50;
            player.maxHitPoints = 50;
            player.attachedRenderable = playerHP;

            drawSmoke();
        }

        public override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            timer += 1;
            explostionTimer += 1;
            explosionAnimation.animationTick(gameTime);
            mineExplostionAnimation.animationTick(gameTime);
            missileSmoke.Update(gameTime);
            exLimSound.Update(gameTime);

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            mouse_x = currentMouseState.X;
            mouse_y = currentMouseState.Y;

            // Sets up mine
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                minePos.X = mouse_x;
                minePos.Y = mouse_y;
                drawMines = true;
                mine.active = true;
                mineCount = 1;
            }

            // Controls player movements
            if (keyState.IsKeyDown(Keys.Left))
            {
                if (player.getPosX() > Dir.leftBoundary)
                {
                    player.setPosX(player.getPosX() - 4);                   // Moves player 3 units left
                    player.setFlip(SpriteEffects.FlipHorizontally);
                }
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                if (player.getPosX() < (Dir.rightBoundary - (player.getWidth() - 5)))
                {
                    player.setPosX(player.getPosX() + 4);                   // Moves player 3 units right
                    player.setFlip(SpriteEffects.None);
                }
            }

            // Moves enemy sprite
            enemy1.setPosX(enemy1.getPosX() + Dir.rnd.Next(1, 3));
            enemy2.setPosX(enemy2.getPosX() - Dir.rnd.Next(1, 3));

            // If enemy runs off screen, reset position to random x postion
            if (enemy1.getPosX() > Dir.rightBoundary + 60)
            {
                enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
            }
            if (enemy2.getPosX() < Dir.leftBoundary - (enemy2.getWidth() + 50))
            {
                enemy2.setPos(Dir.rnd.Next(Dir.rightBoundary, Dir.rightBoundary + 200), Dir.rnd.Next(200, 700));
            }

            // Shoot missile
            if (keyState.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space))
            {
                missile.setPos(player.getPosX() + player.getWidth() / 2, player.getPosY() - missile.getHeight());
                missile.active = true;
                drawSmoke();
                drawMissile = true;
                if (missileCount > 0)
                {
                    missileCount--;
                }
            }

            if (missileCount == 0)
            {
                missileCount = 5;
            }

            if (missile.getPosY() < 185 + missile.getHeight())
            {
                missile.setPos(player.getPos());
                drawMissile = false;
                missileSmoke.deActivate();
            }

            if (timer % 361 == 360)
            {
                anchor.setPos(enemy1.getPosX() + enemy1.getWidth() / 2, enemy1.getPosY() + enemy1.getHeight() / 2);
                anchorDraw = true;
                
            }

            if (anchor.getPosY() > Dir.bottomBoundary + anchor.getHeight())
            {
                anchor.active = false;
                anchorDraw = false;
            }
            // Collision Control
            enemy1Collision = missile.collision(enemy1);
            enemy2Collision = missile.collision(enemy2);

            if (enemy1Collision)
            {
                enemy1.hitPoints = enemy1.hitPoints - 10;
                currXMissile = enemy1.getPosX() + enemy1.getWidth() / 2;
                currYMissile = enemy1.getPosY() - enemy1.getHeight() / 2;
                if (enemy1.hitPoints <= 0)
                {
                    collision1 = true;
                    enemy1.hitPoints = 10;
                    Dir.enemy1Count++;
                    missileSmoke.deActivate();
                }
                getSetMissile();
            }
            if (enemy2Collision)
            {
                missileSmoke.deActivate();
                enemy2.hitPoints = enemy2.hitPoints - 10;
                drawMissile = false;
                if (enemy2.hitPoints <= 0)
                {
                    currXMissile = enemy2.getPosX() + enemy2.getWidth() / 2;
                    currYMissile = enemy2.getPosY() - enemy2.getHeight() / 2;
                    collision2 = true;
                    Dir.enemy2Count++;
                    enemy2.hitPoints = 20;
                }
                getSetMissile();
            }

            // Gain 10 hp if missile hits the anchor preserve
            bool lifeGain = missile.collision(life);
            if (lifeGain == true)
            {
                if (player.hitPoints <50)
                    player.hitPoints = player.hitPoints + 10;
                lifeGain = false;
                drawMissile = false;
                life.active = false;
                life.setPos(new Vector2(Dir.rnd.Next(250,800), Dir.rnd.Next(400, 600)));
                missile.active = false;
                missile.setPos(player.getPosX(), player.getPosY());
                missileSmoke.deActivate();
            }

            bool anchorCollision = anchor.collision(player);
            if (anchorCollision)
            {
                anchorDraw = false;
                anchor.setActiveAndVisible(false);
                anchor.setPos(new Vector2(-100, -100));
                player.hitPoints = player.hitPoints - 20;
            }

            mineCollission = mine.collision(enemy1);
            if (mineCollission)
            {
                mine.active = false;
                drawMines = false;
                mineCurrPos = mine.getPos();
                mine.setPos(new Vector2(-100, 0));
                enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                mineCollisionToggle = true;
                score += 150;
                mineCount = 0;
                Dir.enemy1Count += 1;
                exLimSound.playSoundIfOk();
            }

            mineCollission2 = mine.collision(enemy2);
            if (mineCollission2)
            {
                mine.active = false;
                drawMines = false;
                mineCurrPos = mine.getPos();
                mine.setPos(new Vector2(-100, 0));
                enemy2.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                mineCollisionToggle2 = true;
                score += 200;
                mineCount = 0;
                Dir.enemy2Count += 1;
                exLimSound.playSoundIfOk();
            }
            // Toggle BB
            if (keyState.IsKeyDown(Keys.B) && prevKeyState.IsKeyUp(Keys.B))
            {
                showBB = !showBB;
            }

            // Pause
            if (keyState.IsKeyDown(Keys.P) && !prevKeyState.IsKeyDown(Keys.P))
            {
                gameStateManager.pushLevel(3);
            }

            // Help State
            if (keyState.IsKeyDown(Keys.F1) && !prevKeyState.IsKeyDown(Keys.F1))
            {
                gameStateManager.pushLevel(5);
            }

            if (player.hitPoints < 0)
            {
                player.hitPoints = 0;
            }

            // End State
            if (score >= 1000 || player.hitPoints <= 0)
            {
                if (timer % 181 == 180)
                {
                    gameStateManager.setLevel(4);
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if (score >= 1000)
            {
                status = "THE END!";
            } 
            else if (player.hitPoints == 0)
            {
                status = "YOU DEAD!";
            }
            else
            {
                status = "Missile Left: " + missileCount;
            }

            spriteBatch.Begin();

            backgroundLevel2.Draw(spriteBatch);
            spriteBatch.DrawString(scoreFont, "Score: " + score + "/1000", new Vector2(20, 20), Color.Black);     // Draw Score on top left
            spriteBatch.DrawString(scoreFont, status, new Vector2(1100, 20), Color.Black);              // Draw missile count
            spriteBatch.Draw(texMouse, new Rectangle((int)mouse_x, (int)mouse_y, 16, 16), Color.White); // draw the mouse here 
            enemy1.Draw(spriteBatch);
            enemy2.Draw(spriteBatch);
            player.Draw(spriteBatch);
            missileSmoke.Draw(spriteBatch);

            if (timer > 300)
            {
                life.setActiveAndVisible(true);
                life.draw(spriteBatch);
            }
            
            // If certain conditions are met, then draw these weapons
            if (drawMissile == true && missileCount != 0)
            {
                missile.draw(spriteBatch);
                missile.setPosY(missile.getPosY() - 3);
                missileSmoke.sysPos = new Vector2(missile.getPosX() + missile.getWidth() / 2, missile.getPosY() + missile.getHeight());
            }

            if (anchorDraw == true)
            {
                anchor.setActiveAndVisible(true);
                anchor.draw(spriteBatch);
                anchor.setPosY(anchor.getPosY() + 4);
            }

            if (drawMines && mineCount == 1)
            {
                mine.setPos(minePos.X, minePos.Y);
                mine.draw(spriteBatch);
            }

            // If there is a collision, draw explosion animation
            if (collision1 == true)
            {
                drawMissile = false;
                drawExplosion(currXMissile, currYMissile);
                enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                if (explostionTimer == 100)
                {
                    collision1 = false;
                    score += 50;
                    explostionTimer = 0;
                }
            }
            if (collision2 == true)
            {
                drawMissile = false;
                drawExplosion(currXMissile, currYMissile);
                enemy2.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                if (explostionTimer == 80)
                {
                    score += 100;
                    collision2 = false;
                    explostionTimer = 0;
                    missileSmoke.deActivate();
                }
            }

            if (mineCollisionToggle == true)
            {
                mineCollission = false;
                mineExplostionAnimation.setPos(mineCurrPos.X-mine.getWidth()/2, mineCurrPos.Y-mine.getHeight()/2);
                mineExplostionAnimation.draw(spriteBatch);
                if (explostionTimer == 100)
                {
                    mineExplostionAnimation.makeInactive = true;
                    enemy1.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                    mineCollisionToggle = false;
                    explostionTimer = 0;
                }
            }
            if (mineCollisionToggle2 == true)
            {
                mineCollission2 = false;
                mineExplostionAnimation.setPos(mineCurrPos.X - mine.getWidth() / 2, mineCurrPos.Y - mine.getHeight() / 2);
                mineExplostionAnimation.draw(spriteBatch);
                if (explostionTimer == 100)
                {
                    mineExplostionAnimation.makeInactive = true;
                    enemy2.setPos(Dir.rnd.Next(-400, -250), Dir.rnd.Next(200, 700));
                    mineCollisionToggle2 = false;
                    explostionTimer = 0;
                }
            }
            // Draw BB
            if (showBB)
            {
                player.drawInfo(spriteBatch, Color.White, Color.Yellow);
                enemy1.drawInfo(spriteBatch, Color.White, Color.Yellow);
                enemy2.drawInfo(spriteBatch, Color.White, Color.Yellow); ;
                missile.drawInfo(spriteBatch, Color.White, Color.Yellow);
                life.drawInfo(spriteBatch, Color.White, Color.Yellow);
                mine.drawInfo(spriteBatch, Color.White, Color.Yellow);
                anchor.drawInfo(spriteBatch, Color.White, Color.Yellow);
                //weaponList.drawInfo(spriteBatch, Color.White, Color.Yellow);
            }
            spriteBatch.End();
        }

        public void getSetMissile()
        {
            missile.active = false;
            missile.setPos(player.getPosX(), player.getPosY());
            exLimSound.playSoundIfOk();
            explostionTimer = 0;
        }
        public void drawExplosion(float currXMissile, float currYMissile)
        {
            explosionAnimation.setPos(currXMissile, currYMissile);
            explosionAnimation.draw(spriteBatch);
        }

        public void drawSmoke()
        {
            missileSmoke = new ParticleSystem(new Vector2(missile.getPosX(), missile.getPosY() + missile.getHeight() + 50), 400, 999, 108);
            tex = texMissileSmoke;
            missileSmoke.setMandatory1(tex, new Vector2(15, 15), new Vector2(10, 10), Color.DimGray, new Color(200, 200, 200, 50));
            missileSmoke.setMandatory2(-1, 1, 1, 5, 0);
            rectangle = new Rectangle(0, 0, 1400, 900);
            missileSmoke.setMandatory3(120, rectangle);
            missileSmoke.setMandatory4(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            missileSmoke.randomDelta = new Vector2(0.01f, 0.01f);
            missileSmoke.setDisplayAngle = true;
            missileSmoke.Origin = 0;
        }
    }
    // -------------------------------------------------------- Pause State ----------------------------------------------------------------------------------

    class GameLevel_Pause : RC_GameStateParent
    {
        RC_RenderableList ren = new RC_RenderableList();
        Texture2D texPauseScreen; //         

        Texture2D texBack;
        ImageBackground pauseBackground;
        Texture2D texPause;
        Sprite3 pause;
        SpriteFont pauseHelpText;
        public override void LoadContent()
        {
            pauseHelpText = Content.Load<SpriteFont>("helpText");
            texPauseScreen = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Pause.jpg");  //   
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Pause.jpg");  // 
            texPause = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\Pause2.jpg");  // 

            pauseBackground = new ImageBackground(texBack, Color.White, graphicsDevice);
            pause = new Sprite3(true, texPause, 444, 30);

            pause.setWidthHeight(pause.getWidth() / 2, pause.getHeight() / 2);
            pause.setColor(Color.Aquamarine);
        }

        public override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.R) && !prevKeyState.IsKeyDown(Keys.R))
            {
                gameStateManager.popLevel();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            pauseBackground.Draw(spriteBatch);
            pause.Draw(spriteBatch);

            spriteBatch.DrawString(pauseHelpText, "Level 1.", new Vector2(100, 150), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Shoot enemy ships down. Missile does 10 damage. Aim for 300 Points.", new Vector2(100, 200), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Pirate Merchant Ships has 10hp and gives you 50 Points.", new Vector2(100, 240), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Tug Boats has 20hp and gives you 100 Points.", new Vector2(100, 280), Color.White);

            spriteBatch.DrawString(pauseHelpText, "Level 2.", new Vector2(100, 330), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Blow UP the Enemy Ships. Missile does 10 damage. Aim for 1000 Points.", new Vector2(100, 370), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Pirate Merchant Ships has 10hp and gives you 150 Points.", new Vector2(100, 410), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Tug Boats has 20hp and gives you 200 Points.", new Vector2(100, 450), Color.White);
            spriteBatch.DrawString(pauseHelpText, "Mines instantly destroy the enemy.", new Vector2(100, 490), Color.White);

            spriteBatch.DrawString(pauseHelpText, "Press r to return", new Vector2(100, 850), Color.Yellow);

            spriteBatch.End();
        }
    }

    // -------------------------------------------------------- End State ------------------------------------------------------------------------------------

    class GameLevel_End : RC_GameStateParent
    {
        Texture2D texBack; //         

        SpriteFont font2;
        ImageBackground background;

        public override void LoadContent()
        {
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\End.jpg");
            font1 = Content.Load<SpriteFont>("GameOver");
            font2 = Content.Load<SpriteFont>("helpText");
            background = new ImageBackground(texBack, Color.White, graphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            int enemy1_3 = Dir.enemy1Count + Dir.enemy3Count;
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            background.Draw(spriteBatch);
            spriteBatch.DrawString(font1, "GAME OVER!!!", new Vector2(350, 70), Color.Brown);
            spriteBatch.DrawString(font2, "You have destroyed " + enemy1_3 + " Pirate Merchant Ships and " + Dir.enemy2Count 
                + " Tug Boat(s).", new Vector2(200, 220), Color.White);

            spriteBatch.End();
        }
    }

    // -------------------------------------------------------- Help State -----------------------------------------------------------------------------------

    class GameLevel_Instructions : RC_GameStateParent
    {
        Texture2D texBack; //         

        SpriteFont instructionFont;
        ImageBackground background;

        public override void LoadContent()
        {
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + @"Art\End.jpg");
            instructionFont = Content.Load<SpriteFont>("helpText");
            background = new ImageBackground(texBack, Color.White, graphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.F1) && !prevKeyState.IsKeyDown(Keys.F1))
            {
                gameStateManager.popLevel();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            background.Draw(spriteBatch);
            spriteBatch.DrawString(instructionFont, "Instructions!!!", new Vector2(350, 70), Color.Brown);
            spriteBatch.DrawString(instructionFont, "Press right arrow to move right and left arrow to move left.", new Vector2(150, 150), Color.Brown);
            spriteBatch.DrawString(instructionFont, "Press Space to Shoot.", new Vector2(150, 190), Color.Brown);
            spriteBatch.DrawString(instructionFont, "Press P to Pause Screen.", new Vector2(150, 230), Color.Brown);

            spriteBatch.DrawString(instructionFont, "Press F1 to return.", new Vector2(150, 850), Color.White);
            spriteBatch.End();
        }
    }
}
