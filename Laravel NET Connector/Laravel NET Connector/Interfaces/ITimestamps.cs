using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Interfaces
{
    public interface ITimestamps
    {
        DateTime? DeletedAt { get; set; }
    }
}
