using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Consts;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Score;

namespace Tamagotchi.Competition.Controllers
{
    [EnableCors(ConfigSections.CORS_POLICY)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CompetitionController : ControllerBase
    {
        private readonly IScoreProvider _scoreProvider;
        private readonly IOptions<AppConfig> _appConfig;

        public CompetitionController(IScoreProvider scoreProvider, IOptions<AppConfig> appConfig)
        {
            _scoreProvider = scoreProvider;
            if (appConfig != null)
                _appConfig = appConfig;
        }

        [HttpGet("version")]
        [AllowAnonymous]
        public ApiResult<VersionViewModel> Version()
        {
            return new ApiResult<VersionViewModel> { Data = new VersionViewModel { Version = _appConfig.Value.ProjectVersion } };
        }

        [HttpGet("getScore")]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        [Authorize]
        public async Task<ApiResult<ScoreViewModel>> GetScore()
        {
            if (User == null)
                return new ApiResult<ScoreViewModel>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
            var claim = User.Claims
                            .Where(_ => _.Type.Equals(AppConsts.USER_ID))
                            .Select(_ => _.Value)
                            .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(claim))
                return new ApiResult<ScoreViewModel>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
            if (long.TryParse(claim, out long userId))
            {
                return await _scoreProvider.GetScoreAsync(userId == default ? 0 : userId);
            }
            else
            {
                return new ApiResult<ScoreViewModel>() { Errors = new List<Error> { new Error { Message = ErrorCodes.PROTOCOL_INCORRECT } } };
            }
        }

        [HttpGet("getTopPlayers")]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        [Authorize]
        public async Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayers()
        {
            ApiResult<IEnumerable<ScoreViewModel>> topPlayers = null;
            try
            {
                topPlayers = await _scoreProvider.GetTopPlayersAsync();
            }
            catch (Exception)
            {
                return new ApiResult<IEnumerable<ScoreViewModel>>() { Errors = new List<Error> { new Error { Message = ErrorCodes.SERVER_ERROR } } };
            }
            return topPlayers;
        }

        [Consumes("application/json")]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        [Authorize]
        [HttpPut("changeScore")]
        public async Task<dynamic> ChangeScore([FromBody]ScoreParam model)
        {
            try
            {
                if (User == null)
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
                var claim = User.Claims
                                .Where(_ => _.Type.Equals(AppConsts.USER_ID))
                                .Select(_ => _.Value)
                                .FirstOrDefault();
                if (string.IsNullOrWhiteSpace(claim))
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
                if (long.TryParse(claim, out long userId))
                    model.UserId = userId;
                else
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.PROTOCOL_INCORRECT } } };
                var result = await _scoreProvider.UpdateScoreAsync(model);
                return result;
            }
            catch (Exception)
            {
                return new ApiResult<SuccessResult> { Errors = new List<Error> { new Error { Message = ErrorCodes.PROTOCOL_INCORRECT } } };
            }
        }

    }
}