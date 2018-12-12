using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Helpers.Rest;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Event;

namespace Tamagotchi.Competition.Providers.Score
{
    public class ScoreProvider : IScoreProvider
    {
        private readonly TamagotchiCompetitionContext _ctx;
        private readonly IEventProvider _eventProvider;

        public ScoreProvider(TamagotchiCompetitionContext ctx, IEventProvider eventProvider)
        {
            _ctx = ctx;
            _eventProvider = eventProvider;
        }

        public async Task<ApiResult<ScoreViewModel>> GetScoreAsync(long userId)
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
                await CreateUserScoreAsync(userId: userId);
            return new ApiResult<ScoreViewModel> { Data = scores.FirstOrDefault() };
        }

        private async Task<ApiResult<ScoreViewModel>> CreateUserScoreAsync(long userId)
        {
            var scoreEntity = new Context.Score
            {
                UserId = userId,
                Value = default
            };
            _ctx.Score.Add(scoreEntity);
            await _ctx.SaveChangesAsync(CancellationToken.None);
            return new ApiResult<ScoreViewModel>
            {
                Data = new ScoreViewModel
                {
                    UserId = userId,
                    Value = scoreEntity.Value
                }
            };
        }

        public async Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayersAsync(int takeCount)
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
                return new ApiResult<IEnumerable<ScoreViewModel>> { Errors = new List<Error> { new Error { Message = "" } } };
            var request = await RequestExecutor.ExecuteRequest("/address", new RestRequest("/address", Method.GET)
                                        .AddHeader("Content-type", "application/json")
                                        .AddJsonBody(topPlayers.Select(x => x.UserId).ToArrayAsync(CancellationToken.None)));
            var logins = JsonConvert.DeserializeObject<List<UserViewModel>>(request);
            if (!logins.Any())
                return new ApiResult<IEnumerable<ScoreViewModel>> { Errors = new List<Error> { new Error { Message = "" } } };
            var result = await topPlayers.ToListAsync(CancellationToken.None);
            foreach (var score in result)
            {
                var currentLogin = logins.FirstOrDefault(_ => _.UserId == score.UserId);
                if (currentLogin == null)
                    return new ApiResult<IEnumerable<ScoreViewModel>> { Errors = new List<Error> { new Error { Message = "" } } };
                score.Login = currentLogin.UerName;
            }
            return new ApiResult<IEnumerable<ScoreViewModel>> { Data = result };
        }

        public async Task<ApiResult<ScoreViewModel>> UpdateScoreAsync(ScoreParam model)
        {
            var @event = await _eventProvider.GetEvent(model);
            var score = await _ctx.Score.Where(x => x.UserId == model.UserId).FirstOrDefaultAsync();
            if (score == null)
                return new ApiResult<ScoreViewModel> { Errors = new List<Error> { new Error { Message = "" } } };
            @event.Value += model.Value;
            await _ctx.SaveChangesAsync(CancellationToken.None);
            return new ApiResult<ScoreViewModel> { Data = { } };
        }

    }
}
