using System.Threading.Tasks;
using Tamagotchi.Competition.Helpers.API;
using Tamagotchi.Competition.Models;

namespace Tamagotchi.Competition.Providers.Event
{
    public interface IEventProvider
    {
        Task<ApiResult<EventViewModel>> GetEvent(ScoreParam model);
    }
}
