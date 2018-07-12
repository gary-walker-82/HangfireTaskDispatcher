using System;
using System.Collections.Generic;
using System.Linq;
using BLS.MsExchange.Subscription;
using BLS.SharePoint.Enums;
using Microsoft.Ajax.Utilities;
using Microsoft.Exchange.WebServices.Data;
using ExchangeVersion = BLS.MsExchange.Enums.ExchangeVersion;

namespace BLS.EmailSubscriptions.HangfireExtension.Data
{		public interface ISubscriptionIdentifier
    {
        string MailBox { get;  }
        string SubscriptionName { get;  }
    }

    public interface ISubscriptionExchangeFolderConfiguration
    {
        string RootFolderName { get;  }
        string SharePointSiteCollectionName { get; }
    }

    public interface IBenoyEmailFilingSubscription : ISubscriptionIdentifier, ISubscriptionExchangeFolderConfiguration
    {
        ExchangeVersion ExchangeVersion { get; }
        SharePointVersion SharePointVersion { get; }
        string EmailFilerName { get; }
        StreamingSubscriptionConnection Connection { get; }
        List<SubscriptionChangeCollectionData> SubscriptionChangeCollectionData { get; }
        DateTime LastActivity { get; set; }

        void LockForFolderModification();
        void FolderModificationComplete();

    }
    public static class ActiveSubscriptions
    {
        private static List<IBenoyEmailFilingSubscription> _subscriptions;

        private static List<IBenoyEmailFilingSubscription> Subscriptions =>
            _subscriptions ?? (_subscriptions = new List<IBenoyEmailFilingSubscription>());

        public static List<IBenoyEmailFilingSubscription> ListSubscriptions()
        {
            return Subscriptions;
        }

        public static bool AddSubscription(IBenoyEmailFilingSubscription subscription)
        {
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));
            var benoyEmailFilingSubscription = GetByIdentifier(subscription);
            if (benoyEmailFilingSubscription == null)
            {
                Subscriptions.Add(subscription);
                return true;
            }

            RemoveSubscription(benoyEmailFilingSubscription);
            Subscriptions.Add( subscription);
            return false;
        }

        public static void ClearSubscriptions()
        {
          Subscriptions.SelectMany(x => x.Connection.CurrentSubscriptions)
                .ForEach(x => { x.Unsubscribe(); });
           Subscriptions.ForEach(x => { x.Connection.Close(); });

            Subscriptions.Clear();
        }

        public static void RemoveSubscription(IBenoyEmailFilingSubscription subscription)
        {
            var benoyEmailFilingSubscription = GetByIdentifier(subscription);
            Subscriptions.Remove(benoyEmailFilingSubscription);
        }

        public static IBenoyEmailFilingSubscription GetByIdentifier(ISubscriptionIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));
            return GetByIdentifier(identifier.SubscriptionName, identifier.MailBox);
        }

        public static IBenoyEmailFilingSubscription GetByIdentifier(NotificationDetails identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));
            return GetByIdentifier(identifier.SubscriptionName, identifier.MailBox);
        }

        public static IBenoyEmailFilingSubscription GetByIdentifier(string subscriptionName, string mailBox )
        {
            return  Subscriptions.FirstOrDefault(x =>
                x.MailBox.Equals(mailBox,StringComparison.InvariantCultureIgnoreCase) && x.SubscriptionName == subscriptionName);
        }
      
        public static void SetLastActionTimeOnSubscription(NotificationDetails notificationDetails)
        {
            Subscriptions.FirstOrDefault(x =>
                x.MailBox.Equals(notificationDetails.MailBox,StringComparison.InvariantCultureIgnoreCase) 
                && x.SubscriptionName == notificationDetails.SubscriptionName).LastActivity = DateTime.UtcNow;
        }
    }
}
