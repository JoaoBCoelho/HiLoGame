using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Entities;
using HiLoGame.Repository.Storage;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.Repository;
[ExcludeFromCodeCoverage]
public class GamePlayerInfoRepository : BaseRepository<GamePlayerInfo>, IGamePlayerInfoRepository
{
    public GamePlayerInfoRepository(Context context) : base(context) { }
}

