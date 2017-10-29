using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckContext
    {
        private bool _failCalled;
        private bool _succeedCalled;
        private bool _warnCalled;

        public HealthCheckContext(IHealthCheckSettings settings)
        {
            Settings = settings;
            Stopwatch = new Stopwatch();
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

        public IHealthCheckSettings Settings { get; }

        public IDictionary<string, object> Properties { get; private set; }
        
        public Stopwatch Stopwatch { get; private set; }

        public long ElapsedMilliseconds { get { return Stopwatch.ElapsedMilliseconds; } }

        public CancellationToken CancellationToken { get; set; }

        public void Fail(string message = null, IDictionary<string, object> properties = null)
        {
            _failCalled = true;
            Message = message;
            Properties = properties;
        }

        public void Succeed(string message = null, IDictionary<string, object> properties = null)
        {
            _succeedCalled = true;
            Message = message;
            Properties = properties;
        }

        public void Warn(string message = null, IDictionary<string, object> properties = null)
        {
            _warnCalled = true;
            Message = message;
            Properties = properties;
        }
    }
}