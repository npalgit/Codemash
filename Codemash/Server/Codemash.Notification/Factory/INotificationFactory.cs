﻿using Codemash.Api.Data;

namespace Codemash.Notification.Factory
{
    public interface INotificationFactory
    {
        /// <summary>
        /// Build up the data needed to send a notification
        /// </summary>
        /// <param name="channelUri">The channel Uri the notification will be sent to</param>
        /// <param name="clientType">the type of client we are creating the notification for</param>
        /// <param name="missingChangesetCount">The missing changeset count</param>
        /// <returns>Notification data object</returns>
        NotificationData BuildNotification(string channelUri, string clientType, int missingChangesetCount);
    }
}