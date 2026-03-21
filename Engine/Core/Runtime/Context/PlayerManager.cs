using Shayou.Engine.Core.Domain.Entities;
using System.Collections;

namespace Shayou.Engine.Core.Runtime.Context
{
    public class PlayerManager : IEnumerable<Player>
    {
        private readonly List<Player> _players;

        public int Count => _players.Count;
        public Player? CurrentPlayer { get; private set; }

        public PlayerManager()
        {
            _players = new List<Player>();
            CurrentPlayer = null;
        }

        public void Add(Player player)
        {
            _players.Add(player);
        }

        public bool Remove(Player player)
        {
            if (ReferenceEquals(CurrentPlayer, player))
            {
                CurrentPlayer = null;
            }

            return _players.Remove(player);
        }

        public void Clear()
        {
            _players.Clear();
            CurrentPlayer = null;
        }

        public Player? GetByPosition(int position)
        {
            return _players.FirstOrDefault(player => player.Position == position);
        }

        public void SetCurrentPlayer(Player? player)
        {
            CurrentPlayer = player;
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return _players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
