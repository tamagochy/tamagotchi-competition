using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;

namespace Tamagotchi.Competition.Providers.Score
{
    public interface IScoreProvider
    {
        Task<ApiResult<ScoreViewModel>> GetScoreAsync(long userId);
        Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayersAsync();
        Task<dynamic> UpdateScoreAsync(ScoreParam model);
    }
}
