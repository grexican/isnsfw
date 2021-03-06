﻿using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Repository.Interface;
using ServiceStack.FluentValidation;

namespace IsNsfw.ServiceInterface.Validators
{
    public interface ITagValidator
    {
        bool ValidateTagExists(string key);
    }

    public class TagValidator : ITagValidator
    {
        private readonly ITagRepository _tagRepo;

        public TagValidator(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        public bool ValidateTagExists(string key)
        {
            return _tagRepo.KeyExists(key);
        }
    }
}
