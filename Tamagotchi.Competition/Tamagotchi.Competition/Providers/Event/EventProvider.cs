using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Models;

namespace Tamagotchi.Competition.Providers.Event
{
    public class EventProvider : IEventProvider
    {
        private readonly TamagotchiCompetitionContext _ctx;

        public EventProvider(TamagotchiCompetitionContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<EventViewModel> GetEvent(ScoreParam model)
        {
            IQueryable<Events> evt = _ctx.Events.Where(x => x.ActionCode == model.ActionCode && x.RoomCode == model.RoomCode);
            if (!string.IsNullOrEmpty(model.DeseaseCode))
                evt = evt.Where(x => x.DeseaseCode == model.DeseaseCode);
            evt = evt.Where(x => x.Start >= model.Time && x.Finish <= model.Time);
            return await evt
                .Select(x => new EventViewModel
                {
                    EventId = x.Id,
                    ActionCode = x.ActionCode,
                    DeseaseCode = x.DeseaseCode,
                    EndDate = x.Finish,
                    StartDate = x.Start,
                    Value = x.Value
                }).FirstOrDefaultAsync(CancellationToken.None);
        }
    }
}
