using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LaravelNETConnector.Models;

namespace LaravelNETConnector.Interfaces
{
    public interface IToken
    {
        string Token { get; set; }

        void BeginAutoRefreshAsync();
    }
}
