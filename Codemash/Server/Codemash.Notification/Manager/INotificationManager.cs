﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codemash.Notification.Manager
{
    public interface INotificationManager
    {
        /// <summary>
        /// Send a Tile notification
        /// </summary>
        /// <param name="channelUri"> </param>
        /// <param name="changeCount"> </param>
        void SendTileNotification(string channelUri, int changeCount);

        /// <summary>
        /// Send a notification to the client instructing the tile to revert to its pre notification state
        /// </summary>
        /// <param name="channelUri"></param>
        void SendClearTileNotification(string channelUri);

        /// <summary>
        /// Send a Toast notification
        /// </summary>
        /// <param name="channelUri"> </param>
        /// <param name="changesetCount"> </param>
        void SendToastNotification(string channelUri, int changesetCount);
    }
}
