﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codemash.Api.Data;
using Codemash.Api.Data.Entities;
using Codemash.Api.Data.Extensions;
using Codemash.Api.Data.Parsing.Impl;
using Codemash.Api.Data.Provider;
using Codemash.Api.Data.Repositories;
using Codemash.Server.Core.Extensions;
using Moq;
using Newtonsoft.Json.Linq;

namespace Server.CoreTests.Factory
{
    public static class MoqPollerWorkerProcessTestFactory
    {
        private static IList<Session> _sessionRepository;
        private static IList<SessionChange> _sessionChangeRepository;

        public static ISessionRepository GetStandardSessionRepository()
        {
            var mock = new Mock<ISessionRepository>();
            mock.Setup(m => m.GetAll()).Returns(GetStandardSessions);

            return mock.Object;
        }

        public static ISessionRepository GetNonStandardSessionRepository()
        {
            var mock = new Mock<ISessionRepository>();
            mock.Setup(m => m.GetAll()).Returns(() =>
                {
                    var sessionList = GetStandardSessions();
                    var rand = new Random(DateTime.Now.Second);
                    var randomNumber = rand.Next(0, sessionList.Count - 1);

                    sessionList[randomNumber].Title = "This is a Test";
                    sessionList[randomNumber].Start = sessionList[randomNumber].Start.AddHours(-1);
                    sessionList[randomNumber].Abstract = "My new abstract";

                    randomNumber = rand.Next(0, sessionList.Count - 1);
                    sessionList[randomNumber].Title = "This is another test";
                    sessionList[randomNumber].End = sessionList[randomNumber].End.AddHours(1);

                    sessionList.Apply(s => s.MarkUnmodified());
                    return sessionList;
                });

            return mock.Object;
        }

        public static IMasterDataProvider GetStandardMasterDataProvider()
        {
            var mock = new Mock<IMasterDataProvider>();
            mock.Setup(m => m.GetAllSessions()).Returns(GetStandardSessions);

            return mock.Object;
        }

        public static ISessionChangeRepository GetSessionChangeRepository()
        {
            var mock = new Mock<ISessionChangeRepository>();
            mock.Setup(m => m.GetAll()).Returns(() => _sessionChangeRepository ?? (_sessionChangeRepository = new List<SessionChange>()));
            mock.Setup(m => m.AddRange(It.IsAny<IEnumerable<SessionChange>>())).Callback((IEnumerable<SessionChange> changes) =>
                {
                    if (_sessionChangeRepository == null)
                        _sessionChangeRepository = new List<SessionChange>();

                    foreach (var sessionChange in changes)
                        _sessionChangeRepository.Add(sessionChange);
                });

            mock.Setup(m => m.Save()).Callback(() => _sessionChangeRepository.Apply(sc => sc.MarkUnmodified()));

            return mock.Object;
        }

        private static IList<Session> GetStandardSessions()
        {
            using (var reader = new StreamReader("Factory/StandardSession.json"))
            {
                var jsonString = reader.ReadToEnd();
                var array = JArray.Parse(jsonString);
                var trackParser = new TrackParse();
                var roomParser = new RoomParse();

                var sessions = (from it in array.AsJEnumerable()
                                select new Session
                                           {
                                               SessionId = it["SessionId"].ToString().AsInt(),
                                               Title = it["Title"].ToString(),
                                               Abstract = it["Abstract"].ToString(),
                                               Level = it["Level"].ToString().AsLevel(Level.Unknown),
                                               Track = trackParser.Parse(it["Track"].ToString(), Track.Unknown),
                                               Room = roomParser.Parse(it["Room"].ToString(), Room.Unknown),
                                               Start = it["StartTime"].ToString().AsDateTime(),
                                               End = it["EndTime"].ToString().AsDateTime(),
                                               SpeakerId = it["Speaker"]["SpeakerId"].ToString().AsInt()
                                           }).ToList();
                sessions.Apply(s => s.MarkUnmodified());
                return sessions;
            }
        }

        public static ISessionRepository GetOneLessSessionRepository()
        {
            var mock = new Mock<ISessionRepository>();
            _sessionRepository = null;

            mock.Setup(m => m.GetAll()).Returns(() =>
                                                    {
                                                        if (_sessionRepository == null)
                                                        {
                                                            _sessionRepository = GetStandardSessions();
                                                            _sessionRepository.RemoveAt(0);
                                                        }

                                                        return _sessionRepository;
                                                    });

            mock.Setup(m => m.ApplyRange(It.IsAny<IEnumerable<Session>>()))
                .Callback((IEnumerable<Session> sessions) =>
                              {
                                  foreach (var session in sessions)
                                  {
                                      if (!_sessionRepository.Select(s => s.SessionId).Contains(session.SessionId))
                                      {
                                          _sessionRepository.Add(session);
                                      }
                                  }
                              });

            mock.Setup(m => m.Save()).Callback(() => _sessionChangeRepository.Apply(sc => sc.MarkUnmodified()));

            return mock.Object;
        }

        public static ISessionRepository GetSessionRepositoryWithAddApplyRange()
        {
            var mock = new Mock<ISessionRepository>();
            _sessionRepository = null;

            mock.Setup(m => m.GetAll()).Returns(() =>
            {
                if (_sessionRepository == null)
                {
                    _sessionRepository = GetStandardSessions();
                }

                return _sessionRepository;
            });

            mock.Setup(m => m.ApplyRange(It.IsAny<IEnumerable<Session>>()))
                .Callback((IEnumerable<Session> sessions) =>
                {
                    foreach (var session in _sessionRepository)
                    {
                        if (!sessions.Select(s => s.SessionId).Contains(session.SessionId))
                        {
                            _sessionRepository.First(s => s.SessionId == session.SessionId).MarkRemoved();
                        }
                    }
                });

            mock.Setup(m => m.Save()).Callback(() =>
                                                   {
                                                       var sessionToRemove = _sessionRepository.First(s => s.CurrentState == EntityState.Removed);
                                                       _sessionRepository.Remove(sessionToRemove);
                                                   });

            return mock.Object;
        }

        public static IMasterDataProvider GetOneLessMasterDataProvider()
        {
            var mock = new Mock<IMasterDataProvider>();
            mock.Setup(m => m.GetAllSessions()).Returns(() =>
                                                            {
                                                                var sessionList = GetStandardSessions();
                                                                sessionList.RemoveAt(0);

                                                                return sessionList;
                                                            });

            return mock.Object;
        }
    }
}
