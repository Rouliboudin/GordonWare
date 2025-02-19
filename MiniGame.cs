﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace GordonWare
{
    public class MiniGame
    {
        internal Sprite background; // background's sprite, should be 1280.720
        private Texture2D bar; // Used to display the time left at the bottom of the minigame
        internal string name, author, description; // Name and autoher are not currently displayed, will the description will be during the whole minigame.
        internal Color description_color; // Color of the description, should be either Color.Black or Color.White
        internal SoundEffect win, lose;
        public float timer; // Time since the minigame started in milliseconds.
        private float time_limit; // Time until the moment the player automaticcaly lose. Will decrease with each minigame.
        float outro_time; // Time since the outro animation (either win or lose) started
        public enum GameStatus { Win, Lose, Pending }; // Used to describe the current state of the game. Pending means the game is currently being player, waiting win, lose or time-out
        internal GameStatus game_status; // Used to get the current game status in order to change the draw or the game logic.
        internal SpriteFont Roulifont; // Font used for the description display

        public MiniGame()
        {

        }

        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Roulifont = Content.Load<SpriteFont>("Rouli");
            bar = Content.Load<Texture2D>("white_fullscreen");
            win = Content.Load<SoundEffect>("music/win");
            lose = Content.Load<SoundEffect>("music/lose");
        }

        public virtual void Update(GameTime gameTime)
        {
            if (game_status == GameStatus.Pending) timer += gameTime.ElapsedGameTime.Milliseconds;
            else outro_time += gameTime.ElapsedGameTime.Milliseconds;

            if (timer > time_limit)
                this.Lose();

            if (outro_time > 800 && game_status == GameStatus.Win) MiniGameManager.Win(); // Will call the next game
            if (outro_time > 800 && game_status == GameStatus.Lose) MiniGameManager.Lose(); // Will call the next game

        }
        public virtual void Draw(SpriteBatch spriteBatch)
           // This is drawn over the minigame : instruction of the minigame and timer.
        {
            spriteBatch.DrawString(Roulifont, author, new Vector2(1280 - Roulifont.MeasureString(author).X - 10, 5), description_color * 0.2f, 0f, new Vector2(0,0), 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Roulifont, description,new Vector2(1280/2,25), description_color, 0f, Roulifont.MeasureString(description)/2, 1.6f + 0.1f * (float)Math.Cos(timer/200f),SpriteEffects.None, 0f);
            float time_left = time_limit - timer;
            string seconds_left = Convert.ToString((time_left - time_left % 1000) / 1000);
            string milliseconds_left = game_status == GameStatus.Lose?"0":Convert.ToString(999 - timer % 1000);
            //spriteBatch.DrawString(Roulifont, seconds_left + "." + milliseconds_left, new Vector2(10,5), description_color, 0f, new Vector2(), 2, SpriteEffects.None, 0);
            float current_evolution = (time_limit - time_left) / time_limit; // from 1 to 0 with the time passing
            Color bar_color = new Color(2 * current_evolution, 2 * (1 - current_evolution), 0f);
            spriteBatch.Draw(bar, new Vector2(- 1280 * current_evolution, 690), bar_color);
        }

        public virtual void Reset()
            // This function is called before the Minigame is played, used to reset usefull variables.
            // Feel free to add any random generation of variable in the Reset() function of your minigame's class
        {
            game_status = GameStatus.Pending;
            timer = 0;
            outro_time = 0;
            time_limit = MiniGameManager.time_limit;
        }

        internal void Win()
        {
            MediaPlayer.Stop();
            if (game_status != GameStatus.Win) win.Play();
            game_status = GameStatus.Win;
        }

        internal void Lose()
        {
            MediaPlayer.Stop();
            if (game_status != GameStatus.Lose) lose.Play();
            game_status = GameStatus.Lose;
        }


    }
}
