using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;
using Tamagotchi.Competition.Providers.Score;

namespace Tamagotchi.Competition.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly IScoreProvider _scoreProvider;        

        public CompetitionController(IScoreProvider scoreProvider)
        {
            _scoreProvider = scoreProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        public async Task<ApiResult<ScoreViewModel>> GetScore([FromHeader]long userId)
        {
            var score = await _scoreProvider.GetScoreAsync(userId);
            return score;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        public async Task<ApiResult<IEnumerable<ScoreViewModel>>> GetTopPlayers()
        {
            var topPlayers = await _scoreProvider.GetTopPlayersAsync(10);
            return topPlayers;
        }

        [Consumes("application/json")]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 400)]
        [ProducesResponseType(typeof(ApiResult<ScoreViewModel>), 200)]
        public async Task<ApiResult<ScoreViewModel>> ChangeScore([FromBody]ScoreParam model)
        {
            var result = await _scoreProvider.UpdateScoreAsync(model);
            return result;
        }

    }
}