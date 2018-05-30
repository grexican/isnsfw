using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Command.Interface;
using IsNsfw.Model;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Command.Domains
{
    public class LinkCommandHandlers : CommandHandlerBase
        , ICommandHandler<CreateLinkCommand>
        , ICommandHandler<CreateLinkEventCommand>
    {
        public LinkCommandHandlers(IDbConnectionFactory factory) : base(factory)
        {
        }

        public void Handle(CreateLinkCommand command)
        {
            var link = command.ConvertTo<Link>();

            UnitOfWork(db =>
            {
                db.Save(link);

                command.Id = link.Id;

                if(command.TagIds != null)
                {
                    foreach(var tagId in command.TagIds)
                    {
                        var linkTag = new LinkTag()
                        {
                            LinkId = command.Id,
                            TagId = tagId
                        };

                        db.Save(linkTag);
                    }
                }
            });
        }

        public void Handle(CreateLinkEventCommand command)
        {
            UnitOfWork(db =>
            {
                var c = command.ConvertTo<LinkEvent>();
                c.CreatedAt = DateTime.UtcNow;
                db.Save(c);

                switch(c.LinkEventType)
                {
                    case LinkEventType.View:
                        db.UpdateAdd(() => new Link { TotalViews = 1 }, where: m => m.Id == command.LinkId);
                        break;

                    case LinkEventType.ClickThrough:
                        db.UpdateAdd(() => new Link { TotalClickThroughs = 1 }, where: m => m.Id == command.LinkId);
                        break;

                    case LinkEventType.Preview:
                        db.UpdateAdd(() => new Link { TotalPreviews = 1 }, where: m => m.Id == command.LinkId);
                        break;

                    case LinkEventType.TurnBack:
                        db.UpdateAdd(() => new Link { TotalTurnBacks = 1 }, where: m => m.Id == command.LinkId);
                        break;

                    default:
                        throw new System.ArgumentException($"Unknown {nameof(LinkEventType)} value '{c.LinkEventType}'", nameof(command.LinkEventType));
                }
            });
        }
    }
}
