﻿using System.Linq;
using Codemash.Api.Data.Repositories.Impl;
using Codemash.Notification.Manager;
using Ninject;

namespace Codemash.Poller.Process
{
    public class NotificationCheckProcess : IProcess
    {
        [Inject]
        public INotificationManagerResolver NotificationManagerResolver { get; set; }

        #region Implementation of IProcess

        /// <summary>
        /// Execute the represented process
        /// </summary>
        public void Execute()
        {
            // check each client to make sure they are at the current highest changeset
            using (var context = new CodemashContext())
            {
                var changeList = context.Changes.ToList();
                if (changeList.Count > 0)
                {
                    int currentChangeset = changeList.Max(c => c.Changeset);
                    foreach (var client in context.Clients.Where(c => c.CurrentChangeSet < currentChangeset))
                    {
                        int changesetDifference = currentChangeset - client.CurrentChangeSet;
                        var manager = NotificationManagerResolver.Resolve(client.ClientType);

                        manager.SendTileNotification(client.ChannelUri, changesetDifference);
                        manager.SendToastNotification(client.ChannelUri, changesetDifference);
                    }
                }
            }
        }

        #endregion
    }
}
