﻿namespace AHHA.Core.Common
{
    public class SqlResponce
    {
        public Int64 Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Int64 TotalRecords { get; set; } = 0;
    }
}