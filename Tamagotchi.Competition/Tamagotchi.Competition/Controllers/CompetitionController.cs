using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tamagotchi.Competition.API;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Consts;
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
        private ILogger<CompetitionController> _logger;

        public CompetitionController(IScoreProvider scoreProvider, IOptions<AppConfig> appConfig, ILogger<CompetitionController> logger)
        {
            _scoreProvider = scoreProvider;
            if (appConfig != null)
                _appConfig = appConfig;
            if (logger != null)
                _logger = logger;
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
        public async Task<dynamic> GetScore()
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
                var apiResult = await _scoreProvider.GetScoreAsync(userId == default ? 0 : userId);
                var data = apiResult?.Data?.Value;
                return new JsonResult(new { Data = data });
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
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured: {topPlayers}. {ex}");
                HttpContext.Response.StatusCode = 500;
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
                {
                    HttpContext.Response.StatusCode = 401;
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
                }  
                var claim = User.Claims
                                .Where(_ => _.Type.Equals(AppConsts.USER_ID))
                                .Select(_ => _.Value)
                                .FirstOrDefault();
                if (string.IsNullOrWhiteSpace(claim))
                {
                    HttpContext.Response.StatusCode = 401;
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.UNAUTHORIZED } } };
                }
                if (long.TryParse(claim, out long userId))
                    model.UserId = userId;
                else
                {
                    HttpContext.Response.StatusCode = 400;
                    return new ApiResult<SuccessResult>() { Errors = new List<Error> { new Error { Message = ErrorCodes.PROTOCOL_INCORRECT } } };
                }
                var result = await _scoreProvider.UpdateScoreAsync(model);
                return result;
            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResult<SuccessResult> { Errors = new List<Error> { new Error { Message = ErrorCodes.PROTOCOL_INCORRECT } } };
            }
        }

    }
}