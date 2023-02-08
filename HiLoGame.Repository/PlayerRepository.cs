using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Entities;
using HiLoGame.Repository.Storage;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository;
[ExcludeFromCodeCoverage]
public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    public PlayerRepository(Context context) : base(context) { }
}