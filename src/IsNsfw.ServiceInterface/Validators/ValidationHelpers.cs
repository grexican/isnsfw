using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack;
using ServiceStack.FluentValidation;

namespace IsNsfw.ServiceInterface.Validators
{
    public static class ValidationHelpers
    {
        public static IRuleBuilderOptions<T, string> MustBeAUrl<T>(this IRuleBuilder<T, string> builder)
        {
            return builder.Must(ValidationHelpers.Url).WithMessage("Invalid URL");
        }

        public static IRuleBuilderOptions<T, string> MustBeValidKey<T>(this IRuleBuilder<T, string> builder)
        {
            return builder.Must(KeyValidator.ValidateKey).WithMessage("Invalid Key");
        }

        public static bool Url(string arg)
        {
            return Uri.TryCreate(arg, UriKind.Absolute, out _);
        }
    }
}
