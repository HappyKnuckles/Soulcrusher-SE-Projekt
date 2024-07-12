using System.Numerics;
using static Raylib_cs.Raylib;
using Soulcrusher.src.common;
using Raylib_cs;
using Soulcrusher.src.entities.obstacles;
using Soulcrusher.src.entities.enemies;
using Soulcrusher.src.entities.healthpack;
using Soulcrusher.src.levels;

namespace Soulcrusher.src.entities.player
{
    public class Player
    {
        public static Sound DamageSound = LoadSound(Path.Combine(RootDir.RootDirectory, "assets/sounds/hurt_c_08-102842.mp3"));
        // Player data related 
        public Vector2 Position;
        public float HealthPoints;
        bool isDamaged = false;

        // Sprite Related:

        public static Texture2D Sprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/player/DinoSprites - doux.png"));
        public static Texture2D DeathSprite = LoadTexture(Path.Combine(RootDir.RootDirectory, "assets/textures/player/dead.png"));
        public static Vector2 TextureOffset;
        static float CurrentFrame = 3f;  // Decides what sprite rectangle is used
        public bool Dead = false;
        static float DeathFrame = 0f;

        // Seperate timers for walking and idleing:
        static float frameTimer = 0.0f;
        static float frameRate = 0.1f;
        float idleTimer = 0.0f;
        static float damageFrameTimer = 0.0f;
        float deathTimer = 0.0f;

        // Speedboost related:
        public float Velocity = 4f;
        public float SpeedCooldown = 0f;
        float speedTimer = 0f;
        public bool IsSlowed = false;
        float baseSpeed = 4f;

        // For collision with obstacles:
        bool canMoveUp = true;
        bool canMoveDown = true;
        bool canMoveLeft = true;
        bool canMoveRight = true;

        // Shooting related:
        // Cooldown for shooting so you can't spam left click:
        public float ShootCooldown = 0.5f;
        float shootTimer = 0f;
        public float FireBallCooldown = 0f;
        public bool Dual = false;
        public List<Bullet> Bullets;
        public Fireball? Fireball;

        // Used for bullet collision:
        public RockObstacles RockList;
        public List<Enemy> EnemyList;
        public Puddle PuddleList;
        public Levels Lvl;  // Level reference

        public Player(List<Enemy> enemyList, RockObstacles rockObstacles, float hp, Levels lvl)
        {
            // Always spawn Player in the middle of the screen:
            Position = new Vector2(ScreenData.ScreenWidth / 2, ScreenData.ScreenHeight / 2);
            TextureOffset = new(Sprite.width / 24 / 2, Sprite.height / 2);
            HealthPoints = hp;
            EnemyList = enemyList;
            RockList = rockObstacles;
            Bullets = new List<Bullet>();
            DeathFrame = 0f;
            Dead = false;
            Lvl = lvl;
        }

        public void Draw(Vector2 position, Color color)
        {
            // If mouse is left to player position: fliptexture = true
            bool flipTexture = GetMouseX() < position.X;  

            Rectangle frameRec;  // First rectangle from spritsheet

            if (Dead)
            {
                frameRec = new(0.0f, 0.0f, Sprite.width / 5, Sprite.height)
                {
                    x = (float)DeathFrame * (float)Sprite.width / 5  // Adjust rectangle according to CurrentFrame
                };
            }

            else frameRec = new(0.0f, 0.0f, Sprite.width / 24, Sprite.height)
            {
                x = (float)CurrentFrame * (float)Sprite.width / 24  // Adjust rectangle according to CurrentFrame
            };

            float scaleX = 2.5f;
            float scaleY = 2.5f;

            // Flips according to mouse to player position ratio:
            if (flipTexture)
            {
                frameRec.width *= -1f;
                scaleX *= -1f;
            }

            // Set the origin based on flipping otherwise it is not seen because negative frameRed.width:
            Vector2 origin = flipTexture ? new Vector2(-frameRec.width / 2, 
                frameRec.height / 2) : new Vector2(frameRec.width / 2, frameRec.height / 2);

            DrawTexturePro(
                Sprite,
                frameRec,
                new Rectangle(position.X, position.Y, frameRec.width * scaleX, frameRec.height * scaleY),
                origin,
                0,
                color
            );
        }

        public void SpeedBoost()
        {

            if (IsKeyPressed(KeyboardKey.KEY_SPACE) && SpeedCooldown == 0)
            {
                // When pressing space double velocity for 5secs and set cooldown to 20secs:
                Velocity *= 2f;
                speedTimer = 5f;
                SpeedCooldown = 20f;
            }

            if (speedTimer > 0f)
            {
                speedTimer -= GetFrameTime();

                if (speedTimer <= 0f)
                {
                    // SpeedBoost is over, return to base speed or slowed speed:
                    if (IsSlowed)
                    {
                        Velocity = 2f;  // If slowed, set to 2 velocity
                    }

                    else
                    {
                        Velocity = baseSpeed;  // Otherwise, return to base speed
                    }

                    speedTimer = 0f;
                }
            }

            if (SpeedCooldown > 0f && speedTimer == 0f)
            {
                SpeedCooldown -= GetFrameTime();

                if (SpeedCooldown <= 0f)
                {
                    SpeedCooldown = 0f;
                }
            }
        }

        // Related to Obstructor:
        public async Task SlowDown()
        {
            Velocity -= 2f;
            IsSlowed = true;
            await Task.Delay(5000);

            // Check if SpeedBoost is still active, if not, return to base speed:
            Velocity = speedTimer > 0f ? Velocity : baseSpeed;
            IsSlowed = false;
        }

        public void Shoot()
        {
            shootTimer += GetFrameTime();

            // Direction of the bullet according to mousePosition:
            Vector2 mousePosition = GetMousePosition();
            Vector2 direction = Vector2.Normalize(mousePosition - Position - TextureOffset);

            // Used to offset the bullets by the Player texture size:
            Vector2 bulletStartPosition = Position + TextureOffset;

            if (!Dead)
            {

                if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && shootTimer >= ShootCooldown)
                {
                    // Create new bullet for every click:
                    Bullet bullet = new(direction, bulletStartPosition, 8.0f, "player");
                    Bullets.Add(bullet);

                    // For DualFire:
                    if (Dual)
                    {
                        Bullet secBullet = new(-direction, bulletStartPosition, 8.0f, "player");
                        Bullets.Add(secBullet);
                    }

                    shootTimer = 0f;  // Reset the timer for shooting
                }

                // On right click spawn fireball with 60 second cooldown:
                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT) && FireBallCooldown == 0f)
                {
                    Fireball = new(direction, bulletStartPosition, 5.0f);
                    FireBallCooldown = 40f;
                }

                if (FireBallCooldown > 0f)
                {
                    FireBallCooldown -= GetFrameTime();

                    if (FireBallCooldown <= 0f)
                    {
                        FireBallCooldown = 0f;
                    }
                }
            }
        }

        public void UpdateFireball(RockObstacles obstacles, List<Enemy> enemies)
        {
            Fireball?.Update(obstacles, enemies);
        }

        public void UpdateBullets()
        {
            // Prepare a list gathering all colliding bullets in order to delete them:
            List<Bullet> BulletsToRemove = new();

            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                // Update every bullet in the Bullet array (Movement and Collisions)
                Bullets[i].Update(RockList, this, EnemyList, BulletsToRemove);  

                // If bullet is outside playing area it gets deleted out of the list:
                if (Bullets[i].IsOutOfPlayArea())
                {
                    Bullets.RemoveAt(i);
                }
            }

            foreach (var bulletToRemove in BulletsToRemove)
            {
                Bullets.Remove(bulletToRemove);
            }
        }

        public void Movement(RockObstacles rock, Puddle puddle)
        {
            frameTimer += GetFrameTime();

            // Basic Movement limited to initiated ScreenData:
            if (!Dead)
            {

                if (IsKeyDown(KeyboardKey.KEY_D) && Position.X + Sprite.width / 24 * 2 < ScreenData.PlayareaX2 
                    && canMoveRight)
                {

                    if (rock.CheckPlayerCollision(Position, new Vector2(1, 0)))
                    {
                        canMoveRight = false;  // Handle collision: prevent movement to the right
                    }

                    else
                    {

                        if (puddle.PuddleCollision(Position))
                        {
                            Position.X += 0.6f * Velocity;

                            // Reset flags for other directions:
                            canMoveLeft = true;
                            canMoveUp = true;
                            canMoveDown = true;
                        }

                        else
                        {
                            Position.X += Velocity;

                            // Reset flags for other directions:
                            canMoveLeft = true;
                            canMoveUp = true;
                            canMoveDown = true;
                        }
                    }
                }

                if (IsKeyDown(KeyboardKey.KEY_A) && Position.X > ScreenData.PlayareaX1 && canMoveLeft)
                {

                    if (rock.CheckPlayerCollision(Position, new Vector2(-1f, 0f)))
                    {
                        canMoveLeft = false;  // Handle collision: prevent movement to the left
                    }

                    else
                    {

                        if (puddle.PuddleCollision(Position))
                        {
                            Position.X -= 0.6f * Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveUp = true;
                            canMoveDown = true;
                        }

                        else
                        {
                            Position.X -= Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveUp = true;
                            canMoveDown = true;
                        }
                    }
                }

                if (IsKeyDown(KeyboardKey.KEY_W) && Position.Y > ScreenData.PlayareaY1 && canMoveUp)
                {

                    if (rock.CheckPlayerCollision(Position, new Vector2(0f, -1f)))
                    {
                        canMoveUp = false;  // Handle collision: prevent movement upwards
                    }

                    else
                    {

                        if (puddle.PuddleCollision(Position))
                        {
                            Position.Y -= 0.6f * Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveLeft = true;
                            canMoveDown = true;
                        }

                        else
                        {
                            Position.Y -= Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveLeft = true;
                            canMoveDown = true;
                        }
                    }
                }

                if (IsKeyDown(KeyboardKey.KEY_S) && Position.Y + Sprite.height * 2 < ScreenData.PlayareaY2 
                    && canMoveDown)
                {

                    if (rock.CheckPlayerCollision(Position, new Vector2(0, 1)))
                    {
                        canMoveDown = false;  // Handle collision: prevent movement downwards
                    }

                    else
                    {
                        if (puddle.PuddleCollision(Position))
                        {
                            Position.Y += 0.6f * Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveLeft = true;
                            canMoveUp = true;
                        }

                        else
                        {
                            Position.Y += Velocity;

                            // Reset flags for other directions:
                            canMoveRight = true;
                            canMoveLeft = true;
                            canMoveUp = true;
                        }
                    }
                }

                if (!isDamaged)
                {
                    // The timers are used to slow down the movement, Currentframe++ alone is too fast:

                    // Check if any movement key is pressed:
                    if (!NoMovement())
                    {
                        // Update the frame when speedboost is activated:
                        if (speedTimer > 0f)
                        {

                            if (frameTimer > frameRate / Velocity * 2f)  // Same condition as below but faster
                            {
                                CurrentFrame++;
                                if (CurrentFrame < 9f || CurrentFrame > 24f)  // Changes rectangle range 17-24 (sprinting)
                                {
                                    CurrentFrame = 21.1f;
                                }
                                frameTimer = 0f;
                            }
                        }

                        else
                        {
                            // Update the frame if enough time has passed:
                            if (frameTimer >= frameRate / Velocity * 4f)
                            {
                                CurrentFrame++;  // Skips to next frame rectangle

                                if (CurrentFrame < 4f || CurrentFrame > 9f)
                                {
                                    CurrentFrame = 4f;  // Changes rectangle range 4-9 (walking)
                                }

                                frameTimer = 0f;  // Reset the frame timer
                            }
                        }

                        idleTimer = 0f;
                    }

                    else
                    {
                        // If no movement keys are pressed, update the idle animation:
                        idleTimer += GetFrameTime();

                        if (idleTimer >= frameRate / Velocity * 4f)
                        {
                            CurrentFrame++;

                            if (CurrentFrame > 3f) // Changes the rectangle range 0-3 (idle)
                            {
                                CurrentFrame = 0f;
                            }

                            idleTimer = 0f;  // Reset the idle timer
                        }

                        frameTimer = 0f;  // Reset the frame timer when no movement keys are pressed
                    }
                }
            }
        }

        public bool WillTakeLethalDamage(bool reduced)
        {
            float simulatedHealth = HealthPoints;  // Create a copy of the current health points

            // Simulate the damage without actually applying it:
            if (reduced)
            {
                simulatedHealth -= 0.25f;
            }

            else
            {
                simulatedHealth -= 1f;
            }

            return simulatedHealth <= 0f;  // Check if the simulated damage is lethal
        }

        public void TakeDamage(bool reduced)
        {
            isDamaged = true;
            PlaySound(DamageSound);

            if (!WillTakeLethalDamage(reduced))
            {
                CurrentFrame = 14f;
            }

            else
            {
                Sprite = DeathSprite;
                Dead = true;
            }

            if (reduced)
            {
                HealthPoints -= 0.25f;
            }

            else HealthPoints -= 1f;
        }
        public void Death(ref int nextScreen)
        {

            if (Dead)
            {
                deathTimer += GetFrameTime();

                if (deathTimer >= frameRate)
                {
                    DeathFrame++;
                    deathTimer = 0f;
                }

                if (DeathFrame > 4f)
                {
                    Sprite = LoadTexture("../../../assets/textures/player/DinoSprites - doux.png");
                    nextScreen = 0;
                }
            }
        }

        static bool NoMovement()
        {
            return !IsKeyDown(KeyboardKey.KEY_S) && !IsKeyDown(KeyboardKey.KEY_W) && !IsKeyDown(KeyboardKey.KEY_A) 
                && !IsKeyDown(KeyboardKey.KEY_D);
        }

        public void Update(RockObstacles rock, Healthpack healthpack, Puddle puddle)
        {
            // Check if 30 frames have passed since taking damage:
            if (isDamaged && !Dead)
            {
                damageFrameTimer += GetFrameTime();

                // Check if enough time has passed to increment the frame:
                if (damageFrameTimer >= frameRate)
                {
                    CurrentFrame++;

                    // Reset the frame range if it exceeds the damaged frames:
                    if (CurrentFrame > 15f)
                    {
                        isDamaged = false;
                    }
                    damageFrameTimer = 0f;  // Reset the frame increment timer
                }
            }

            SpeedBoost();
            Movement(rock, puddle);
            Shoot();
            UpdateFireball(RockList, EnemyList);
            UpdateBullets();
            healthpack.HPPlayerCollision(Position, ref HealthPoints);
        }
    }
}

