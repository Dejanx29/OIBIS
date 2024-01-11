using Manager;
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace ServiceApp
{
    internal class X509Principal: IPrincipal
    {
        private IIdentity identity;

        public X509Principal(IIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }
        /// <summary>
        /// Proverava da li korisnik ima zadatu rolu u okviru OU dela u sertifikatu
        /// </summary>
        /// <param name="role">Rola korisnika</param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            var userName = Formatter.ParseNameFromCert(identity.Name);
            // check is user banned
            var banManager = BanManager.Instance;
            if (banManager.BannedList.Contains(Identity.Name))
            {
                try
                {
                    Audit.AuthorizationFailed(
                            userName,
                            OperationContext.Current.IncomingMessageHeaders.Action,
                            "User is banned"
                            );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return false;
            }
            var isInRole = identity.Name.Contains(role);
            try
            {
                if (isInRole)
                {
                    Audit.AuthorizationSuccess(userName, OperationContext.Current.IncomingMessageHeaders.Action);
                } else
                {
                    Audit.AuthorizationFailed(
                        userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, 
                        $"Method need {role} permission"
                        );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return identity.Name.Contains(role);
        }
    }
}