﻿namespace NewDocManagement.Core.Services
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
}
