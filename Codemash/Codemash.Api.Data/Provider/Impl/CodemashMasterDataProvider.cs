﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Codemash.Api.Data.Entities;
using Codemash.Api.Data.Extensions;
using Codemash.Api.Data.Parsing;
using Codemash.Api.Data.Parsing.Impl;
using Codemash.Server.Core;
using Codemash.Server.Core.Extensions;
using Newtonsoft.Json.Linq;
using Ninject;

namespace Codemash.Api.Data.Provider.Impl
{
    public class CodemashMasterDataProvider : IMasterDataProvider
    {
        [Inject]
        public ISessionEntityParser SessionEntityParser { get; set; }

        [Inject]
        public ISpeakerEntityParser SpeakerEntityParser { get; set; }

        #region Implementation of IMasterDataProvider

        /// <summary>
        /// Return all sessions from the Master Datasource
        /// </summary>
        public IList<Session> GetAllSessions()
        {
            const string downloadUrl = "http://dl.dropbox.com/u/13029365/codemash_sessions.json";
            var client = new WebClient();
            var jsonString = client.DownloadString(downloadUrl);
            var jsonArray = JArray.Parse(jsonString);

            return (from it in jsonArray.AsJEnumerable()
                    select SessionEntityParser.Parse(it.ToString())).ToList();
        }

        /// <summary>
        /// Return all speakers from the Master Datasource
        /// </summary>
        public IList<Speaker> GetAllSpeakers()
        {
            const string downloadUrl = "http://dl.dropbox.com/u/13029365/codemash_speakers.json";
            var client = new WebClient();
            var jsonString = client.DownloadString(downloadUrl);
            var jsonArray = JArray.Parse(jsonString);

            return (from it in jsonArray.AsJEnumerable()
                    select SpeakerEntityParser.Parse(it.ToString())).ToList();
        }

        #endregion
    }
}
