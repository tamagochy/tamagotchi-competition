using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.AppSettings;
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
        private readonly IOptions<AppConfig> _appConfig;

        public ScoreProvider(TamagotchiCompetitionContext ctx, IEventProvider eventProvider, IOptions<AppConfig> appConfig)
        {
            _ctx = ctx;
            _eventProvider = eventProvider;
            _appConfig = appConfig;
        }

        public async Task<ApiResult<ScoreViewModel>> GetScoreAsync(long userId)
        {
            var scores = await _ctx.Score.Where(_ => _.UserId == userId)
                .Select(_ => new ScoreViewModel
                {
                    ScoreId = default(long?),
                    UserId = default(long?),
                    Value = _.Value
                })
                .ToListAsync(cancellationToken: CancellationToken.None);
            if (scores == null || !scores.Any())
            {
                await CreateUserScoreAsync(userId: userId);
                return await GetScoreAsync(userId);
            }
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

        public async Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayersAsync()
        {
            var topPlayers = _ctx.Score.Select(_ => new ScoreViewModel
            {
                Login = default,
                ScoreId = default(long?),
                UserId = _.UserId,
                Value = _.Value
            })
            .OrderBy(x => x.Value)
            .Take(_appConfig.Value.CountTopPlayers);
            if (!await topPlayers.AnyAsync())
                return new ApiResult<IEnumerable<ScoreViewModel>> { Data = new List<ScoreViewModel>() };
            var url = _appConfig.Value.AuthBaseUrl;
            url += "getUserLogins";
            var ids = await topPlayers.Select(___ => ___.UserId).ToArrayAsync(CancellationToken.None);
            var jarr = JArray.FromObject(ids);
            var request = await RequestExecutor.ExecuteRequest(url, new RestRequest(url, Method.POST)
                                        .AddHeader("Content-type", "application/json")
                                        .AddJsonBody(ids.Where(_ => _.HasValue).Select(_ => _.Value).ToArray()));
            var logins = JsonConvert.DeserializeObject<BaseAuthEntity>(request);
            if (!logins.Users.Any())
                return new ApiResult<IEnumerable<ScoreViewModel>> { Data = new List<ScoreViewModel>() };
            var result = await topPlayers.ToListAsync(CancellationToken.None);
            foreach (var score in result)
            {
                var currentLogin = logins.Users.FirstOrDefault(_ => _.UserId == score.UserId);
                if (currentLogin == null)
                    return new ApiResult<IEnumerable<ScoreViewModel>> { Errors = new List<Error> { new Error { Message = "" } } };
                score.Login = currentLogin.Login;
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
