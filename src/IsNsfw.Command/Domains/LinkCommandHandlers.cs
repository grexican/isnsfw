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
    }
}
