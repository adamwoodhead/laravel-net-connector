using System;
using System.Collections.Generic;
using System.Text;

namespace LaravelNETConnector.Interfaces
{
    public interface ISoftDelete
    {
        DateTime? CreatedAt { get; set; }

        DateTime? UpdatedAt { get; set; }
    }
}
