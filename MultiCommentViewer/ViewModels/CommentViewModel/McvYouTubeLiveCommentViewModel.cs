﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SitePlugin;
using System.Windows.Media;
using System.Windows;

namespace MultiCommentViewer
{
    public class McvYouTubeLiveCommentViewModel : IMcvCommentViewModel
    {
        private readonly YouTubeLiveSitePlugin.IYouTubeLiveMessage _message;
        private readonly IMessageMetadata _metadata;
        private readonly IMessageMethods _methods;
        private readonly IOptions _options;

        private void SetNickname(IUser user)
        {
            if (!string.IsNullOrEmpty(user.Nickname))
            {
                _nickItems = new List<IMessagePart> { Common.MessagePartFactory.CreateMessageText(user.Nickname) };
            }
            else
            {
                _nickItems = null;
            }
        }
        private McvYouTubeLiveCommentViewModel(IMessageMetadata metadata, IMessageMethods methods, ConnectionName connectionName, IOptions options)
        {
            _metadata = metadata;
            _methods = methods;

            ConnectionName = connectionName;
            _options = options;
            ConnectionName.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ConnectionName.Name):
                        RaisePropertyChanged(nameof(ConnectionName));
                        break;
                }
            };
            _metadata.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_metadata.BackColor):
                        RaisePropertyChanged(nameof(Background));
                        break;
                    case nameof(_metadata.ForeColor):
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                    case nameof(_metadata.FontFamily):
                        RaisePropertyChanged(nameof(FontFamily));
                        break;
                    case nameof(_metadata.FontStyle):
                        RaisePropertyChanged(nameof(FontStyle));
                        break;
                    case nameof(_metadata.FontWeight):
                        RaisePropertyChanged(nameof(FontWeight));
                        break;
                    case nameof(_metadata.FontSize):
                        RaisePropertyChanged(nameof(FontSize));
                        break;
                    case nameof(_metadata.IsNameWrapping):
                        RaisePropertyChanged(nameof(UserNameWrapping));
                        break;
                }
            };
            if (_metadata.User != null)
            {
                var user = _metadata.User;
                user.PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(user.Nickname):
                            SetNickname(user);
                            RaisePropertyChanged(nameof(NameItems));
                            break;
                    }
                };
                SetNickname(user);
            }
        }
        public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveComment comment, IMessageMetadata metadata, IMessageMethods methods, ConnectionName connectionName, IOptions options)
            : this(metadata, methods, connectionName, options)
        {
            _message = comment;

            _nameItems = comment.NameItems;
            MessageItems = comment.CommentItems;
            Thumbnail = comment.UserIcon;
            Id = comment.Id.ToString();
            PostTime = comment.PostTime;
        }
        public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveSuperchat item, IMessageMetadata metadata, IMessageMethods methods, ConnectionName connectionName, IOptions options)
            : this(metadata, methods, connectionName, options)
        {
            var comment = item;
            _message = comment;

            _nameItems = comment.NameItems;
            MessageItems = comment.CommentItems;
            Thumbnail = comment.UserIcon;
            Id = comment.Id.ToString();
            PostTime = comment.PostTime;
        }
        public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveConnected connected, IMessageMetadata metadata, IMessageMethods methods, ConnectionName connectionName, IOptions options)
            : this(metadata, methods, connectionName, options)
        {
            _message = connected;
            MessageItems = connected.CommentItems;
        }
        public McvYouTubeLiveCommentViewModel(YouTubeLiveSitePlugin.IYouTubeLiveDisconnected disconnected, IMessageMetadata metadata, IMessageMethods methods, ConnectionName connectionName, IOptions options)
            : this(metadata, methods, connectionName, options)
        {
            _message = disconnected;
            MessageItems = disconnected.CommentItems;
        }

        public ConnectionName ConnectionName { get; }

        private IEnumerable<IMessagePart> _nickItems { get; set; }
        private IEnumerable<IMessagePart> _nameItems { get; set; }
        public IEnumerable<IMessagePart> NameItems
        {
            get
            {
                if (_nickItems != null)
                {
                    return _nickItems;
                }
                else
                {
                    return _nameItems;
                }
            }
        }

        public IEnumerable<IMessagePart> MessageItems { get; private set; }

        public SolidColorBrush Background
        {
            get
            {
                if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(_options.YouTubeLiveBackColor);
                }
                else
                {
                    return new SolidColorBrush(_metadata.BackColor);
                }
            }
        }

        public ICommentProvider CommentProvider => _metadata.CommentProvider;

        public FontFamily FontFamily => _metadata.FontFamily;

        public int FontSize => _metadata.FontSize;

        public FontStyle FontStyle => _metadata.FontStyle;

        public FontWeight FontWeight => _metadata.FontWeight;

        public SolidColorBrush Foreground
        {
            get
            {
                if (_options.IsEnabledSiteConnectionColor && _options.SiteConnectionColorType == SiteConnectionColorType.Site)
                {
                    return new SolidColorBrush(_options.YouTubeLiveForeColor);
                }
                else
                {
                    return new SolidColorBrush(_metadata.ForeColor);
                }
            }
        }

        public string Id { get; private set; }

        public string Info { get; private set; }

        public bool IsVisible => true;

        public string PostTime { get; private set; }

        public IMessageImage Thumbnail { get; private set; }

        public string UserId => _metadata.User?.UserId;

        public TextWrapping UserNameWrapping
        {
            get
            {
                if (_metadata.IsNameWrapping)
                {
                    return TextWrapping.Wrap;
                }
                else
                {
                    return TextWrapping.NoWrap;
                }
            }
        }

        public Task AfterCommentAdded()
        {
            return Task.CompletedTask;
        }
        #region INotifyPropertyChanged
        [NonSerialized]
        private System.ComponentModel.PropertyChangedEventHandler _propertyChanged;
        /// <summary>
        /// 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            _propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dt.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}

