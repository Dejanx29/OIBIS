using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			//System.Diagnostics.Debugger.Launch();
			/// Define the expected service certificate. It is required to establish cmmunication using certificates.
			string srvCertCN = "contactAsistentService";

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			/// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/test"),
									  new X509CertificateEndpointIdentity(srvCert));
            
            using (WCFClient proxy = new WCFClient(binding, address))
			{
				/// 1. Communication test
				var input = 0;
				do
                {
                    Console.WriteLine("1. Send message. (Only for student.)");
					Console.WriteLine("2. Get all Messages. (Only for asistent.)");
					Console.WriteLine("3. Get all Messages that exceed in size. (Only for asistent.)");
					Console.WriteLine("4. Ban student. (Only for asistent.)");
					Console.WriteLine("5. Unban student. (Only for asistent.)");
					Console.WriteLine("Enter option:");
					int.TryParse(Console.ReadLine(), out input);

                    switch (input)
                    {
						case 1:
							{
								Console.WriteLine("Enter message: ");
								proxy.SendMessage(Console.ReadLine());
								break;
							}
						case 2:
                            {
								Console.WriteLine("Messages");
								var list = proxy.GetAllMessages();
								if (list == null) break;
								if (list.Count == 0)
								{
									Console.WriteLine("No message.");
									break;
								}
								foreach (var item in list)
								{
									Console.WriteLine($"Student: {item.Student} Text: {item.Text}");
								}
								break;
							}
						case 3:
							{
								Console.WriteLine("Messages");
								var list = proxy.GetAllMessagesThatExceedInSize();
								if (list == null) break;
								if(list.Count == 0)
                                {
                                    Console.WriteLine("No message.");
									break;
                                }
								foreach (var item in list)
								{
									Console.WriteLine($"Student: {item.Student} Text: {item.Text}");
								}
								break;
							}
						case 4:
                            {
								Console.WriteLine("Messages");
								var list1 = proxy.GetAllMessagesThatExceedInSize();
								if (list1 == null) break;
								int i = 1;
								foreach (var item in list1)
								{
									Console.WriteLine($"{i++}.Student: {item.Student} Text: {item.Text}");
								}
								Console.WriteLine("Enter number: ");
								int num = -1;
								int.TryParse((string)Console.ReadLine(), out num);
								try
								{
									proxy.BanUser(list1[num - 1].Student);
								}
								catch (Exception ex) { }
								break;
							}
						case 5:
                            {
								Console.WriteLine("Messages");
								var list = proxy.GetAllBanedStudents();
								if (list == null) break;
								int i = 1;
								foreach (var item in list)
								{
									Console.WriteLine($"{i++}.Student: {item}");
								}
								Console.WriteLine("Enter number: ");
								int num = -1;
								int.TryParse((string)Console.ReadLine(), out num);
								try
								{
									proxy.UnbanUser(list[num - 1]);
								}
								catch (Exception ex) { }
								break;
							}
					}
				} while(input >=0);
				Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");
				Console.ReadLine();
            }
		}
	}
}
