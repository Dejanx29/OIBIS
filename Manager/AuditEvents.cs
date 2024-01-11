using System.Reflection;
using System.Resources;

namespace Manager
{
    public enum AuditEventTypes
	{
		AuthenticationSuccess = 0,
		AuthorizationSuccess = 1,
        AuthorizationFailure = 2,
		UserBanned = 3,
		UserUnbanned = 4,
	}

	public class AuditEvents
	{
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
                        resourceManager = new ResourceManager
                            (typeof(AuditEventFile).ToString(),
                            Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string AuthenticationSuccess
		{
			get
			{
                // TO DO
                return ResourceMgr.GetString(AuditEventTypes.AuthenticationSuccess.ToString()); 
			}
		}

		public static string AuthorizationSuccess
		{
			get
			{
                //TO DO
                return ResourceMgr.GetString(AuditEventTypes.AuthorizationSuccess.ToString());
            }
		}

		public static string AuthorizationFailed
		{
			get
			{
                //TO DO
                return ResourceMgr.GetString(AuditEventTypes.AuthorizationFailure.ToString());
            }
        }

		public static string UserBanned
		{
			get
			{
				//TO DO
				return ResourceMgr.GetString(AuditEventTypes.UserBanned.ToString());
			}
		}

		public static string UserUnbanned
        {
			get
			{
				//TO DO
				return ResourceMgr.GetString(AuditEventTypes.UserUnbanned.ToString());
			}
		}
	}
}
