using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface.Events;

namespace IsNsfw.ServiceInterface.EventHandlers
{
    class Link_LinkEventCreatedEventHandler : IEventHandler<LinkEventCreatedEvent>
    {
        private readonly ILinkRepository _linkRepo;

        public Link_LinkEventCreatedEventHandler(ILinkRepository linkRepo)
        {
            _linkRepo = linkRepo;
        }

        public void Handle(LinkEventCreatedEvent evt)
        {
            var c = evt.LinkEvent;

            switch(c.LinkEventType)
            {
                case LinkEventType.View:
                    _linkRepo.IncrementTotalViews(c.LinkId);
                    break;

                case LinkEventType.ClickThrough:
                    _linkRepo.IncrementClickThroughs(c.LinkId);
                    break;

                case LinkEventType.Preview:
                    _linkRepo.IncrementPreviews(c.LinkId);
                    break;

                case LinkEventType.TurnBack:
                    _linkRepo.IncrementTurnBacks(c.LinkId);
                    break;

                default:
                    throw new System.ArgumentException($"Unknown {nameof(LinkEventType)} value '{c.LinkEventType}'", nameof(c.LinkEventType));
            }
        }
    }
}
