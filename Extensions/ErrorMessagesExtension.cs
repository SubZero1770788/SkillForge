using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using quiz_project.Common;

namespace quiz_project.Extensions
{
    public static class ErrorMessagesExtension
    {
        public static string GetMessage(this ErrorMessages key) => key switch
        {
            ErrorMessages.QuizNotUsers => "This quiz does not belong to you",
            ErrorMessages.UserNameTaken => "The username is already taken",
            ErrorMessages.UserNotExisting => "The user does not exist",
            ErrorMessages.WrongSession => "Something's wrong with your session - please clear cache",
            ErrorMessages.AccountLocked => "The account got locked out after multiple unsuccessful login attempts",
            ErrorMessages.InvalidLoginAttempt => "Invalid login attempt.",
        };
    }
}