using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.Model;
using BatailleNavale.View;

namespace BatailleNavale.Controller
{
    public interface IGameController
    {
        GameState GameState { get; set; }

        GameWindow GameView { get; set; }
        GridModel PlayerGrid { get; set; }
        GridModel EnemyGrid { get; set; }

        void SetReady();
        void ProcessPlayerHit(Vector2 pos);
        void PlayerHit(Vector2 pos);
        void EnemyHit(Vector2 pos);


    }

    public enum GameState
    {
        PlayersChooseBoatsLayout,
        Player1Turn,
        Player2Turn,
        GameEnded
    }
}