using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.API;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Consts;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Event;

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
            var baseUrl = _appConfig.Value.AuthBaseUrl;
            baseUrl += "getUserLogins";
            _logger.LogDebug(baseUrl);          
            var ids = await topPlayers
                .Where(x => x.UserId.HasValue)
                .Select(___ => ___.UserId.Value)
                .ToArrayAsync(CancellationToken.None);           
            _logger.LogDebug($"player ids that: {string.Join(",", ids)}");
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = JArray.Parse(JsonConvert.SerializeObject(ids));
            HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(baseUrl, json);          
            BaseAuthEntity logins = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                if(content != null)
                    logins = JsonConvert.DeserializeObject<BaseAuthEntity>(content);
            }           
            if (!logins.Users.Any())
                throw new Exception(ErrorCodes.SERVER_ERROR);
            var result = await topPlayers.ToListAsync(CancellationToken.None);
            foreach (var score in result)
            {
                var currentLogin = logins.Users.FirstOrDefault(_ => _.UserId == score.UserId);                
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

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }

}
