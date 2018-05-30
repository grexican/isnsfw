using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Command.Interface;
using IsNsfw.Repository.Interface;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;

namespace IsNsfw.Command
{
    public class CreateLinkCommand : ICommandWithIntResult
    {
        public string SessionId { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public int? UserId { get; set; }
        public HashSet<int> TagIds { get; set; }

        // out parameter
        public int Id { get; set; }
    }
}
