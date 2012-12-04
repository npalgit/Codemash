﻿using Caliburn.Micro;
using Codemash.Phone.Core;
using Codemash.Phone.Data.Repository;
using Codemash.Phone.Shared.DataModels;
using Ninject;

namespace Codemash.Phone7.App.ViewModels
{
    public class SessionViewModel : ViewModelBase
    {
        private SessionDetailView _sessionDetailView;

        public long IncomingSession { get; set; }

        [Inject]
        public ISessionRepository SessionRepository { get; set; }

        [Inject]
        public ISpeakerRepository SpeakerRepository { get; set; }

        public SessionViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        // attributes
        public SessionDetailView Session
        {
            get
            {
                if (_sessionDetailView == null)
                {
                    var session = SessionRepository.Get(IncomingSession);
                    _sessionDetailView = new SessionDetailView
                                             {
                                                 Title = session.Title,
                                                 Technology = session.Technology,
                                                 Starts = session.Starts.AsDateTime().AsBlockDisplay(),
                                                 Duration = session.Duration.AsFullDurationString(),
                                                 Difficulty = session.Difficulty,
                                                 Room = session.Room,
                                                 Abstract = session.Abstract,
                                                 Speaker = new SpeakerDetailView(SpeakerRepository.Get(session.SpeakerId)),
                                                 IsFavorite = session.IsFavorite
                                             };
                }

                return _sessionDetailView;
            }
        }

        public bool CanMakeFavorite { get { return !Session.IsFavorite; } }
        public bool CanMakeNotFavorite { get { return Session.IsFavorite; } }

        // behaviors
        public void ShowMap()
        {
            NavigationService.UriFor<MapViewModel>().Navigate();
        }

        public void MakeFavorite()
        {
            UpdateFavorite(true);
        }

        public void MakeNotFavorite()
        {
            UpdateFavorite(false);
        }

        private void UpdateFavorite(bool favoriteStatus)
        {
            Session.IsFavorite = favoriteStatus;
            NotifyOfPropertyChange("CanMakeFavorite");
            NotifyOfPropertyChange("CanMakeNotFavorite");
            SessionRepository.UpdateFavoriteStatus(IncomingSession, favoriteStatus);
        }
    }
}
