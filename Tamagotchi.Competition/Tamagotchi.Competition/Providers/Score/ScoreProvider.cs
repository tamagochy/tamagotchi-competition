using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Consts;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Helpers.Rest;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Event;
using SuccessResult = Tamagotchi.Competition.Helpers.API.SuccessResult;

namespace Tamagotchi.Competition.Providers.Score
{
    public class ScoreProvider : IScoreProvider
    {
        private readonly TamagotchiCompetitionContext _ctx;
        private readonly IEventProvider _eventProvider;
        private readonly IOptions<AppConfig> _appConfig;
        private ILogger<ScoreProvider> _logger;

        public ScoreProvider(TamagotchiCompetitionContext ctx, IEventProvider eventProvider, IOptions<AppConfig> appConfig, ILogger<ScoreProvider> logger)
        {
            _ctx = ctx;
            _eventProvider = eventProvider;
            _appConfig = appConfig;
            if (logger != null)
                _logger = logger;
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
            .OrderByDescending(x => x.Value)
            .Take(_appConfig.Value.CountTopPlayers);
            if (!await topPlayers.AnyAsync())
                return new ApiResult<IEnumerable<ScoreViewModel>> { Data = new List<ScoreViewModel>() };
            var url = _appConfig.Value.AuthBaseUrl;
            url += "getUserLogins";
            _logger.LogDebug(url);
            var ids = await topPlayers.Select(___ => ___.UserId).ToArrayAsync(CancellationToken.None);

            _logger.LogDebug($"player ids that: {string.Join(",", ids)}");
            var request = await RequestExecutor.ExecuteRequest(url, new RestRequest(url, Method.POST)
                                        .AddHeader("Content-type", "application/json")
                                        .AddJsonBody(ids.Where(_ => _.HasValue).Select(_ => _.Value).ToArray()));
            _logger.LogDebug($"Response from Auth: {request}");
            var logins = JsonConvert.DeserializeObject<BaseAuthEntity>(request);
            if (!logins.Users.Any())
                throw new Exception(ErrorCodes.SERVER_ERROR);
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

        public async Task<dynamic> UpdateScoreAsync(ScoreParam model)
        {
            var validateErrors = ValidateUpdateScoreModel(model);
            if (validateErrors.Errors != null && validateErrors.Errors.Any())
                return validateErrors;
            var @event = await _eventProvider.GetEvent(model);
            if (@event == null || @event.Data == null)
                return new ApiResult<SuccessResult> { Errors = new List<Error> { new Error { Message = ErrorCodes.BUSSINESS_CODE_EVENT_NOT_FOUND } } };
            var score = await _ctx.Score.Where(x => x.UserId == model.UserId).FirstOrDefaultAsync();
            if (score == null)
                return new ApiResult<SuccessResult> { Errors = new List<Error> { new Error { Message = ErrorCodes.BUSSINESS_CODE_SCORE_NOT_FOUND } } };
            score.Value += @event.Data.Value;// += model.Value;
            await _ctx.SaveChangesAsync(CancellationToken.None);
            return new ApiResult<SuccessResult> { Data = new SuccessResult { Succeed = true } };
        }

        private ApiResult<ScoreViewModel> ValidateUpdateScoreModel(ScoreParam model)
        {
            var errorResults = new List<Error>();
            var apiResult = new ApiResult<ScoreViewModel>() { Errors = new List<Error>(errorResults) };
            if (string.IsNullOrWhiteSpace(model.ActionCode))
                errorResults.Add(new Error { Attribute = "actionCode", Message = ErrorCodes.VALIDATION_MISSING });
            if (string.IsNullOrWhiteSpace(model.RoomCode))
                errorResults.Add(new Error { Attribute = "roomCode", Message = ErrorCodes.VALIDATION_MISSING });
            if (string.IsNullOrWhiteSpace(model.Time))
                errorResults.Add(new Error { Attribute = "time", Message = ErrorCodes.VALIDATION_MISSING });
            if (errorResults.Any())
                return new ApiResult<ScoreViewModel>() { Errors = new List<Error>(errorResults) };
            if (!TimeSpan.TryParse(model.Time, out TimeSpan result))
                errorResults.Add(new Error { Message = ErrorCodes.PROTOCOL_INCORRECT });
            else
                model.EventTime = result;
            return new ApiResult<ScoreViewModel>() { Errors = new List<Error>(errorResults) };
        }

    }
}
