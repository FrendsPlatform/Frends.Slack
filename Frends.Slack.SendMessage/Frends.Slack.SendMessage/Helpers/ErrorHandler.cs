﻿using System;
using Frends.Slack.SendMessage.Definitions;

namespace Frends.Slack.SendMessage.Helpers
{
    /// <summary>
    /// Handles error with usage of a standard ThrowOnFailure Frends flag
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Handler for exceptions
        /// </summary>
        /// <param name="exception">Caught exception</param>
        /// <param name="throwOnFailure">Frends flag</param>
        /// <param name="errorMessage">Message to throw in error event</param>
        /// <returns>Throw exception if a flag is true, else return Result with Error info</returns>
        public static Result Handle(Exception exception, bool throwOnFailure, string errorMessage)
        {
            if (throwOnFailure)
            {
                throw new Exception($"{errorMessage}\n{exception.Message}");
            }

            return new Result(null, false, new Error
            {
                Message = $"{errorMessage}\n{exception.Message}",
                AdditionalInfo = exception,
            });
        }
    }
}
