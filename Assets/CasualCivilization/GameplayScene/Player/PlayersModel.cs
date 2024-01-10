using System.Collections.Generic;
using UniRx;

namespace CasualCivilization.GameplayScene.Player
{
    public class PlayerEntitiesModel
    {
    }

    public class PlayerModel
    {
        private IntReactiveProperty _currency = new();
        public IReadOnlyReactiveProperty<int> Currency => _currency;

        public PlayerEntitiesModel Entities { get; }
    }

    public class PlayersModel
    {
        public PlayerModel UserModel;
        public List<PlayerModel> Bots;
        
        
    }
}