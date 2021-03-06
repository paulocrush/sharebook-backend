using Microsoft.Extensions.Options;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using ShareBook.Domain;
using ShareBook.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareBook.Service.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationSettings _settings;
        private readonly OneSignalClient _oneSignalClient;
        private NotificationCreateOptions _notificationCreateOptions;
        public NotificationService(IOptions<NotificationSettings> notificationSettings)
        {
            _settings = notificationSettings.Value;
            _oneSignalClient = new OneSignalClient(_settings.ApiKey);
            _notificationCreateOptions = new NotificationCreateOptions
            {
                AppId = new Guid(_settings.AppId)
            };
        }

        public string SendNotificationSegments(NotificationOnesignal onesignal)
        {
            try
            {
                _notificationCreateOptions.IncludedSegments = new List<string>()
                {
                    GetSegments(onesignal.TypeSegments)
                };

                _notificationCreateOptions.Headings.Add(LanguageCodes.Portuguese, onesignal.Title);
                _notificationCreateOptions.Contents.Add(LanguageCodes.Portuguese, onesignal.Content);

                _oneSignalClient.Notifications.Create(_notificationCreateOptions);
            }
            catch (Exception ex)
            {
                new Exception($"Error executing SendNotificationSegments. Exception: {ex.Message}. StackTrace: {ex.StackTrace}");
            }

            return "Enviado com sucesso";
        }

        public string SendNotificationByKey(NotificationOnesignal onesignal)
        {
            try
            {
                _notificationCreateOptions.Filters = new List<INotificationFilter>
                {
                    new NotificationFilterField { Field = NotificationFilterFieldTypeEnum.Tag, Key = onesignal.Key, Value = onesignal.Value}
                };

                _notificationCreateOptions.Headings.Add(LanguageCodes.English, onesignal.Title);
                _notificationCreateOptions.Contents.Add(LanguageCodes.English, onesignal.Content);

                _notificationCreateOptions.Headings.Add(LanguageCodes.Portuguese, onesignal.Title);
                _notificationCreateOptions.Contents.Add(LanguageCodes.Portuguese, onesignal.Content);

                _oneSignalClient.Notifications.Create(_notificationCreateOptions);

            }
            catch (Exception ex)
            {
                new Exception($"Error executing SendNotificationByUserId. Exception: {ex.Message}. StackTrace: {ex.StackTrace}");
            }

            return $"Notification enviado para o {onesignal.Value} com sucesso";
        }


        public string SendNotificationByEmail(string email, string title, string content)
        {
            try
            {
                _notificationCreateOptions.Filters = new List<INotificationFilter>
                {
                        new NotificationFilterField { Field = NotificationFilterFieldTypeEnum.Tag, Key = "email", Value = email}
                };

                _notificationCreateOptions.Headings.Add(LanguageCodes.English, title);
                _notificationCreateOptions.Contents.Add(LanguageCodes.English, content);

                _notificationCreateOptions.Headings.Add(LanguageCodes.Portuguese, title);
                _notificationCreateOptions.Contents.Add(LanguageCodes.Portuguese, content);

                _oneSignalClient.Notifications.Create(_notificationCreateOptions);
            }
            catch (Exception ex)
            {
                new Exception($"Error executing SendNotificationByUserId. Exception: {ex.Message}. StackTrace: {ex.StackTrace}");
            }

            return $"Notification enviado para o {email} com sucesso";
        }

        private string GetSegments(TypeSegments typeSegments)
        {
            switch (typeSegments)
            {
                case TypeSegments.Inactive:
                    return "Inactive Users";
                case TypeSegments.Engaged:
                    return "Engaged Users";
                case TypeSegments.All:
                    return "Subscribed Users";
                case TypeSegments.Active:
                    return "Active Users";
                default:
                    return "";
            }
        }
    }
}
