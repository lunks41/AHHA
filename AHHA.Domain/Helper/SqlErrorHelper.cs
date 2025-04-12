﻿using AHHA.Core.Common;

namespace AHHA.Core.Helper
{
    public static class SqlErrorHelper
    {
        public static string GetErrorMessage(int errorCode)
        {
            return errorCode switch
            {
                SqlErrorCodes.PrimaryKeyViolation => SqlErrorCodes.PrimaryKeyViolationMessage,
                SqlErrorCodes.UniqueConstraintViolation => SqlErrorCodes.UniqueConstraintViolationMessage,
                SqlErrorCodes.ForeignKeyViolation => SqlErrorCodes.ForeignKeyViolationMessage,
                SqlErrorCodes.DeadlockVictim => SqlErrorCodes.DeadlockVictimMessage,
                SqlErrorCodes.Timeout => SqlErrorCodes.TimeoutMessage,
                SqlErrorCodes.LoginFailed => SqlErrorCodes.LoginFailedMessage,
                SqlErrorCodes.SyntaxError => SqlErrorCodes.SyntaxErrorMessage,
                SqlErrorCodes.InvalidColumnName => SqlErrorCodes.InvalidColumnNameMessage,
                SqlErrorCodes.InvalidColumnMatch => SqlErrorCodes.InvalidColumnMatchMeasage,
                SqlErrorCodes.InvalidObjectName => SqlErrorCodes.InvalidObjectNameMessage,
                _ => "An unknown error occurred."
            };
        }
    }
}