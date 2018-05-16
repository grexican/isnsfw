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
        public string Key { get; set; }
        public string Url { get; set; }
        public int? UserId { get; set; }
        public HashSet<int> TagIds { get; set; }

        // out parameter
        public int Id { get; set; }
    }

    public class CreateLinkCommandValidator : CommandValidatorBase<CreateLinkCommand>
    {
        private readonly ILinkRepository _linkRepo;

        public CreateLinkCommandValidator(ILinkRepository linkRepo)
        {
            _linkRepo = linkRepo;
            RuleFor(m => m.Key).NotEmpty();
            RuleFor(m => m.Url).NotEmpty().MustBeAUrl();
        }

        public override ValidationResult Validate(ValidationContext<CreateLinkCommand> context)
        {
            var ret = base.Validate(context);

            if(ret.IsValid)
            {
                if(_linkRepo.KeyExists(context.InstanceToValidate.Key))
                    ret.Errors.Add(new ValidationFailure(nameof(context.InstanceToValidate.Key), $"Key '{context.InstanceToValidate.Key}' already exists."));
            }

            return ret;
        }
    }
}
