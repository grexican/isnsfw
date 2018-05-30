﻿using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceModel;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;

namespace IsNsfw.ServiceInterface.Validators
{
    public class CreateLinkRequestValidator : AbstractValidator<CreateLinkRequest>
    {
        private readonly ILinkRepository _linkRepo;

        public CreateLinkRequestValidator(ILinkRepository linkRepo, ITagValidator tagValidator)
        {
            _linkRepo = linkRepo;
            RuleFor(m => m.Key).NotEmpty();
            RuleFor(m => m.Url).NotEmpty().MustBeAUrl();
            RuleFor(m => m.Tags).NotEmpty();
            RuleForEach(m => m.Tags).Must(tagValidator.ValidateTagKey).WithMessage(m => $"Tag '{m}' not found.");
        }
        
        public override ValidationResult Validate(ValidationContext<CreateLinkRequest> context)
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