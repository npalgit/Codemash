﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Codemash.Phone.Data.Common;
using Codemash.Phone.Data.Entities;
using Codemash.Phone.Data.Extensions;
using Codemash.Phone.Data.Repository;
using Microsoft.Phone.Shell;
using Ninject;
using RestSharp;

namespace Codemash.Phone.Data.Provider.Impl
{
    public class SessionSpeakerChangeProvider : IChangeProvider
    {
        [Inject]
        public ISessionRepository SessionRepository { get; set; }

        [Inject]
        public ISpeakerRepository SpeakerRepository { get; set; }

        [Inject]
        public ISettingsRepository SettingsRepository { get; set; }

        #region Implementation of IChangeProvider

        /// <summary>
        /// Apply changes
        /// </summary>
        /// <param name="changeList"></param>
        public void ApplyChanges(IList<Change> changeList)
        {
            // separate out the changes
            var speakerChanges = changeList.Where(c => string.Compare("speaker", c.EntityType, StringComparison.OrdinalIgnoreCase) == 0);
            var sessionChanges = changeList.Where(c => string.Compare("session", c.EntityType, StringComparison.OrdinalIgnoreCase) == 0);

            // apply the changes
            ApplySpeakerChanges(speakerChanges);
            ApplySessionChanges(sessionChanges);

            // save the changes
            SpeakerRepository.SaveChanges();
            SessionRepository.SaveChanges();

            // indicate to the service that we are up to date (we will not work on the response to this)
            if (changeList.Count > 0)
            {
                int changeSet = changeList.Max(c => c.Changeset);
                UpdateClientChangesetToLatest(changeSet);
            }
        }

        #endregion

        #region Speaker Change Application Methods

        /// <summary>
        /// Apply the changes for Speakers as dicated
        /// </summary>
        /// <param name="speakerChanges"></param>
        private void ApplySpeakerChanges(IEnumerable<Change> speakerChanges)
        {
            var speakerChangesList = speakerChanges.ToList();
            foreach (var change in speakerChangesList)
            {
                switch (change.Action)
                {
                    case ActionType.Delete:
                        DeleteSpeaker(change.EntityId);
                        break;
                    case ActionType.Modify:
                        ModifySpeaker(change.EntityId, change.Key, change.Value);
                        break;
                    case ActionType.Add:
                        var entityId = change.EntityId;
                        AddSpeaker(entityId, speakerChangesList.Where(c => c.EntityId == entityId));
                        break;
                }
            }
        }

        private void DeleteSpeaker(long entityId)
        {
            var speaker = SpeakerRepository.Get(entityId);
            if (speaker != null)
                speaker.MarkAsDeleted();
        }

        private void ModifySpeaker(long entityId, string propertyName, string value)
        {
            var speaker = SpeakerRepository.Get(entityId);
            if (speaker != null)
                speaker.UpdateProperty(propertyName, value);
        }

        private void AddSpeaker(long entityId, IEnumerable<Change> changes)
        {
            // ensure that the entityId does NOT already exist in the Repository
            var speaker = SpeakerRepository.Get(entityId);
            if (speaker == null)
            {
                // this speaker does not exist - create it
                speaker = new Speaker {SpeakerId = entityId};
                foreach (var change in changes)
                    speaker.UpdateProperty(change.Key, change.Value);
            }
        }

        #endregion

        #region Session Change Application Methods

        private void ApplySessionChanges(IEnumerable<Change> sessionChanges)
        {
            var sessionChangesList = sessionChanges.ToList();
            foreach (var change in sessionChangesList)
            {
                switch (change.Action)
                {
                    case ActionType.Delete:
                        DeleteSession(change.EntityId);
                        break;
                    case ActionType.Modify:
                        ModifySession(change.EntityId, change.Key, change.Value);
                        break;
                    case ActionType.Add:
                        var entityId = change.EntityId;
                        AddSession(entityId, sessionChangesList.Where(c => c.EntityId == entityId));
                        break;
                }
            }
        }

        private void AddSession(long entityId, IEnumerable<Change> changes)
        {
            // ensure that the entityId does NOT already exist in the Repository
            var session = SessionRepository.Get(entityId);
            if (session == null)
            {
                // this speaker does not exist - create it
                session = new Session() { SessionId = entityId };
                foreach (var change in changes)
                    session.UpdateProperty(change.Key, change.Value);
            }
        }

        private void ModifySession(long entityId, string key, string value)
        {
            var session = SpeakerRepository.Get(entityId);
            if (session != null)
                session.UpdateProperty(key, value);
        }

        private void DeleteSession(long entityId)
        {
            var session = SessionRepository.Get(entityId);
            if (session != null)
                session.MarkAsDeleted();
        }

        #endregion

        private void UpdateClientChangesetToLatest(int changeset)
        {
            var client = new RestClient("http://192.168.1.4/DeltaApi/api");
            var request = new RestRequest("Change/Update", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("ChannelUri", SettingsRepository.PushChannelUri);
            request.AddParameter("Changeset", changeset);

            client.ExecuteAsync(request, resp =>
                                             {
                                                 return;
                                             });
        }
    }
}
