﻿using System;
using System.Windows;
using Caliburn.Micro;
using Codemash.Phone.Data.Repository;
using Codemash.Phone.Shared.Common;
using Codemash.Phone.Shared.Services;
using Ninject;

namespace Codemash.Phone7.App.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        [Inject]
        public ISessionRepository SessionRepository { get; set; }

        [Inject]
        public ISpeakerRepository SpeakerRepository { get; set; }

        [Inject]
        public IAppService ApplicationService { get; set; }

        public SplashViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        // behaviors
        public void PageLoaded()
        {
            ApplicationService.PushChannelInitialized += ApplicationService_PushChannelInitialized;
            ApplicationService.InitializePushChannel(PhoneClientType.WinPhone7);
        }

        void ApplicationService_PushChannelInitialized(object sender, EventArgs e)
        {
            SessionRepository.LoadCompleted += SessionRepository_LoadCompleted;
            SessionRepository.Load();
        }

        // the load of the Session Repository completed
        private void SessionRepository_LoadCompleted(object sender, EventArgs e)
        {
            SpeakerRepository.LoadCompleted += SpeakerRepository_LoadCompleted;
            SpeakerRepository.Load();
        }

        // the load of the Speaker Repository completed
        private void SpeakerRepository_LoadCompleted(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.UriFor<MainViewModel>().Navigate());
        }
    }
}
