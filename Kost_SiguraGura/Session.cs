using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
    /// <summary>
    /// ✅ FIX Issue #5: Thread-safe Session class with locking mechanism
    /// Previous version had race conditions with static properties
    /// </summary>
    internal class Session
    {
        // ✅ Static lock object untuk thread-safety
        private static readonly object _lockObject = new object();

        private static long _userId;
        private static string _userRole;
        private static string _username;
        private static string _token;

        public static long UserId
        {
            get
            {
                lock (_lockObject)
                {
                    return _userId;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _userId = value;
                }
            }
        }

        public static string UserRole
        {
            get
            {
                lock (_lockObject)
                {
                    return _userRole;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _userRole = value;
                }
            }
        }

        public static string Username
        {
            get
            {
                lock (_lockObject)
                {
                    return _username;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _username = value;
                }
            }
        }

        public static string Token
        {
            get
            {
                lock (_lockObject)
                {
                    return _token;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _token = value;
                }
            }
        }

        /// <summary>
        /// Clear all session data safely
        /// </summary>
        public static void Clear()
        {
            lock (_lockObject)
            {
                _userId = 0;
                _userRole = null;
                _username = null;
                _token = null;
            }
        }
    }
}
