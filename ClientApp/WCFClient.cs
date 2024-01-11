using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Common;
using System.IO;

namespace ClientApp
{
	public class WCFClient : ChannelFactory<IWCFContract>,  IDisposable
	{
		IWCFContract factory;
		private string secretKeyPath = "key";

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			X509Certificate2 cert = null;
			do
			{
				Console.WriteLine("Enter your name:");
				string cltCertCN = Console.ReadLine();
				cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.CurrentUser, cltCertCN);
            } while (cert == null);
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			//string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            //Console.WriteLine(cltCertCN);
						
			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = cert;//CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.CurrentUser, cltCertCN);

			factory = this.CreateChannel();
		}

		public void SendMessage(string msg)
		{
			try
			{
				string key = String.Empty;
				if (File.Exists(secretKeyPath))
				{
                    key = SecretKey.LoadKey(secretKeyPath);
                }
				if (key == String.Empty)
				{
					throw new Exception("Key not provided");
				}
				// encrypt message
				var enctryptedMessage = AESManager.Encrypt(msg, key);
				Console.WriteLine(Encoding.ASCII.GetString(enctryptedMessage));
                factory.SendMessage(enctryptedMessage);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public List<Common.Message> GetAllMessages()
		{
			try
			{
				return factory.GetAllMessages();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}

		public void BanUser(string name)
		{
			try
			{
				factory.BanUser(name);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public void UnbanUser(string name)
		{
			try
			{

				factory.UnbanUser(name);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}
		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void SendMessage(string msg, byte[] sign)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetAllMessagesThatExceedInSize()
        {
			try
			{
				return factory.GetAllMessagesThatExceedInSize();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}

        public List<string> GetAllBanedStudents()
        {
			try
			{
				return factory.GetAllBanedStudents();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}
    }
}
