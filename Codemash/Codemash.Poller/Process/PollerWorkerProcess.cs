﻿using System.Threading.Tasks;
using System.Transactions;
using Codemash.Api.Data.Compare;
using Codemash.Api.Data.Provider;
using Codemash.Api.Data.Repositories;
using Ninject;

namespace Codemash.Poller.Process
{
    public class PollerWorkerProcess
    {
        [Inject]
        public IMasterDataProvider MasterDataProvider { get; set; }

        [Inject]
        public ISessionRepository SessionRepository { get; set; }

        [Inject]
        public SessionCompare SessionComparer { get; set; }

        [Inject]
        public ISessionChangeRepository SessionChangeRepository { get; set; }

        public void Execute()
        {
            // first step is to get the Session master data
            var masterSessionList = MasterDataProvider.GetAllSessions();

            // get the local session List for comparison
            var localSessionList = SessionRepository.GetAll();

            // perform the comparison
            var sessionDifferences = SessionComparer.CompareSessionLists(masterSessionList, localSessionList);
            if (sessionDifferences.Count > 0)
            {
                // session data has changed since last save
                using (var transaction = new TransactionScope())
                {
                    // add the items to the change repository
                    SessionChangeRepository.AddRange(sessionDifferences);
                    SessionChangeRepository.Save();

                    // add the master session data into the respository
                    SessionRepository.ApplyRange(masterSessionList);
                    SessionRepository.Save();

                    // commit the transaction
                    transaction.Complete();
                }
            }
        }
    }
}
