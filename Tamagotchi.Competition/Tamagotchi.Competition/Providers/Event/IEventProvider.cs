using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tamagotchi.Competition.Models;

namespace Tamagotchi.Competition.Providers.Event
{
    public interface IEventProvider
    {
        Task<EventViewModel> GetEvent(ScoreParam model);
    }
}
