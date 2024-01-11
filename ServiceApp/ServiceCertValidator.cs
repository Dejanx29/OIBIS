using System;
using System.Data;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using Manager;

namespace ServiceApp
{
    public class ServiceCertValidator : X509CertificateValidator
	{
		/// <summary>
		/// Implementation of a custom certificate validation on the service side.
		/// Service should consider certificate valid if its issuer is the same as the issuer of the service.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		public override void Validate(X509Certificate2 certificate)
		{
			/// This will take service's certificate from storage
			var name = "contactAsistentService";//Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, name);

			if (!certificate.Issuer.Equals(srvCert.Issuer))
			{
				throw new Exception("Certificate is not from the valid issuer.");
			}

            var userName = Formatter.ParseNameFromCert(certificate.SubjectName.Name);
            try
            {
                Audit.AuthenticationSuccess(userName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
	}
}
