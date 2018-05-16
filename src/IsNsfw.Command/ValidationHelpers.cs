using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.FluentValidation;

namespace IsNsfw.Command
{
    public static class ValidationHelpers
    {
        public static IRuleBuilderOptions<T, string> MustBeAUrl<T>(this IRuleBuilderOptions<T, string> builder)
        {
            return builder.Must(ValidationHelpers.Url).WithMessage("Invalid URL");
        }

        public static bool Url(string arg)
        {
            return Uri.TryCreate(arg, UriKind.Absolute, out _);
        }
    }
}
