using System;
using UniRx;

namespace CasualCivilization.GameplayScene.Session
{
    public class SessionModel 
        : IDisposable
    {
        private readonly IntReactiveProperty _turn = new();
        
        public IReadOnlyReactiveProperty<int> Turn => _turn;

        public SessionModel()
        {
        }

        public void EndTurn()
        {
            _turn.Value += 1;
        }

        public void Dispose()
        {
        }
    }
}