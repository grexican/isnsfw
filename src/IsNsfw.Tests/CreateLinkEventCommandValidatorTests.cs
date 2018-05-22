using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.Command;
using IsNsfw.Repository.Interface;
using Moq;
using NUnit.Framework;

namespace IsNsfw.Tests
{
    public class CreateLinkEventCommandValidatorTests
    {
        [Test]
        public void ErrorIfNullSessionId()
        {
            var sut     = new CreateLinkEventCommandValidator();
            var results = sut.Validate(new CreateLinkEventCommand() { });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkEventCommand.SessionId)));
        }
    }
}
