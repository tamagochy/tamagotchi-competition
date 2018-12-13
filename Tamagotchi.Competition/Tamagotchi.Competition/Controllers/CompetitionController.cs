using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Consts;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Score;

namespace Tamagotchi.Competition.Controllers
{
    [EnableCors(ConfigSections.CORS_POLICY)]
    [Route("api/[controller]")]
    [Produces("application/json")]
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

        [HttpGet(nameof(Version))]
        public ApiResult<VersionViewModel> Version()
        {
            var user = User;
            return new ApiResult<VersionViewModel> { Data = new VersionViewModel { Version = _appConfig.Value.ProjectVersion } };
        }

        [Authorize]
        [HttpPost(nameof(VersionPost))]
        public ApiResult<VersionViewModel> VersionPost() => new ApiResult<VersionViewModel> { Data = new VersionViewModel { Version = _appConfig.Value.ProjectVersion } };

        [HttpGet(nameof(State))]
        public bool State() => true;

        [HttpGet(nameof(GetScore))]
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

        [HttpGet(nameof(GetTopPlayers))]
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
        [HttpPut(nameof(ChangeScore))]
        public async Task<ApiResult<ScoreViewModel>> ChangeScore([FromBody]ScoreParam model)
        {
            var result = await _scoreProvider.UpdateScoreAsync(model);
            return result;
        }

    }
}