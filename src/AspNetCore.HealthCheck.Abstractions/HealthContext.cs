﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AspNetCore.HealthCheck
{
    public class HealthContext
    {
        private bool _failCalled;
        private bool _succeedCalled;
        private bool _warnCalled;

        public HealthContext(IWatchSettings settings)
        {
            Settings = settings;
        }

        public bool HasSucceeded
        {
            get
            {
                return !_failCalled && _succeedCalled;
            }
        }

        public bool HasWarned
        {
            get
            {
                return !_failCalled && _warnCalled;
            }
        }

        public string Message { get; private set; }

        public IWatchSettings Settings { get; }

        public IDictionary<string, object> Properties { get; private set; }
        
        public long Elapsed { get; set; }

        public void Fail(string message = null, IDictionary<string, object> properties = null)
        {
            _failCalled = true;
            Message = message;
            Properties = properties;
        }

        public void Succeed(string message = null, IDictionary<string, object> properties = null)
        {
            _succeedCalled = true;
            Message = Message;
            Properties = properties;
        }

        public void Warn(string message = null, IDictionary<string, object> properties = null)
        {
            _warnCalled = true;
            Message = Message;
            Properties = properties;
        }
    }
}