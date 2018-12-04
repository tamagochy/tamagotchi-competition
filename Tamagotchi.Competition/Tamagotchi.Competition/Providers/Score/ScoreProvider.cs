using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;

namespace Tamagotchi.Competition.Providers.Score
{
    public class ScoreProvider : IScoreProvider
    {
        private readonly TamagotchiCompetitionContext _ctx;

        public ScoreProvider(TamagotchiCompetitionContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ApiResult<ScoreViewModel>> GetScore(long userId)
        {
            var scores = await _ctx.Score.Where(_ => _.UserId == userId)
                .Select(_ => new ScoreViewModel
                {
                    ScoreId = _.Id,
                    UserId = _.UserId,
                    Value = _.Value
                })
                .ToListAsync(cancellationToken: CancellationToken.None);
            if (scores == null || !scores.Any())
                return new ApiResult<ScoreViewModel> { Error = new Error { Message = "" } };
            return new ApiResult<ScoreViewModel> { Data = scores.FirstOrDefault() };
        }

        public async Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayers(int takeCount)
        {
            var topPlayers = _ctx.Score.Select(_ => new ScoreViewModel
            {
                Login = default,
                ScoreId = _.Id,
                UserId = _.UserId,
                Value = _.Value
            })
            .OrderBy(x => x.Value)
            .Take(takeCount);
            if (!await topPlayers.AnyAsync())
                return new ApiResult<IEnumerable<ScoreViewModel>> { Error = new Error { } };
            //apply logins
            return new ApiResult<IEnumerable<ScoreViewModel>> { Data = topPlayers };
        }

    }
}
