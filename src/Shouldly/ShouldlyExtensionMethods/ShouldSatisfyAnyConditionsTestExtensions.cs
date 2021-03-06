﻿using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using JetBrains.Annotations;

namespace Shouldly
{
    [ShouldlyMethods]
    public static class ShouldSatisfyAnyConditionsTestExtensions
    {
        public static void ShouldSatisfyAnyConditions(this object actual, [InstantHandle] params Action[] conditions)
        {
            ShouldSatisfyAnyConditions(actual, () => null, conditions);
        }
        public static void ShouldSatisfyAnyConditions(this object actual, string customMessage, [InstantHandle] params Action[] conditions)
        {
            ShouldSatisfyAnyConditions(actual, () => customMessage, conditions);
        }
        public static void ShouldSatisfyAnyConditions(this object actual, [InstantHandle] Func<string> customMessage, [InstantHandle] params Action[] conditions)
        {
            var errorMessages = new List<Exception>();
            foreach (var action in conditions) 
            {
                try
                {
                    action.Invoke();
                    return;
                }
                catch (Exception exc)
                {
                    errorMessages.Add(exc);
                }
            }

            var errorMessageString = BuildErrorMessageString(errorMessages);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(errorMessageString, actual, customMessage).ToString());
        }

        static string BuildErrorMessageString(IEnumerable<Exception> errorMessages)
        {
            var errorCount = 1;
            var sb = new StringBuilder();
            foreach (var errorMessage in errorMessages)
            {
                sb.AppendLine($"--------------- Error {errorCount} ---------------");
                var lines = errorMessage.Message.Replace("\r\n", "\n").Split('\n');
                var paddedLines = lines.Select(l => string.IsNullOrEmpty(l) ? l : "    " + l);
                var value = string.Join("\r\n", paddedLines.ToArray());
                sb.AppendLine(value);
                sb.AppendLine();
                errorCount++;
            }
            sb.AppendLine("-----------------------------------------");

            return sb.ToString().TrimEnd();
        }
    }
}
