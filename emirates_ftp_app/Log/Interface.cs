using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Log
{
    internal interface Interface<T>
    {
        bool Update(T t);
        bool DeleteAsLOVSTATUS(T t);
        bool Delete(T t);
        T GetbyID(string USER_ID);
        IEnumerable<T> GetAll();
    }

    internal interface ILog
    {
        void Debug(string message, string arg = null);
        void Info(string message, string arg = null);
        void Warning(string message, string arg = null);
        void Error(string message, string arg = null);
    }
}
