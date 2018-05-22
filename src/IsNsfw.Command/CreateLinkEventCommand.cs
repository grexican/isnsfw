using IsNsfw.Command.Interface;
using IsNsfw.Model;
using ServiceStack.FluentValidation;

namespace IsNsfw.Command
{
    public class CreateLinkEventCommand : ICommand
    {
        public int LinkId { get; set; }
        public string SessionId { get; set; }
        public LinkEventType LinkEventType { get; set; }
    }

    public class CreateLinkEventCommandValidator : CommandValidatorBase<CreateLinkEventCommand>
    {
        public CreateLinkEventCommandValidator()
        {
            RuleFor(m => m.SessionId).NotEmpty();
            RuleFor(m => m.LinkId).NotEmpty();
        }
    }
}
