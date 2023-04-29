﻿using MediatR;
using NewDocManagement.Core.Models;

namespace NewDocManagement.Core.Events
{
    /// <summary>
    /// Published when a new document was uploaded into the system.
    /// </summary>
    public record NewDocumentReceived(Document Document) : INotification;
}
