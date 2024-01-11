using System;
using System.Collections.Generic;
using Contracts;
using System.Threading;
using System.ServiceModel;
using Common;
using Manager;
using System.IO;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;

namespace ServiceApp
{
    public class WCFService : IWCFContract
    {
        private string secretKeyPath = "key";
        /// <summary>
        /// Lista svih poruka koje su studenti poslali
        /// </summary>
        private static List<Message> messages = new List<Message>();
        /// <summary>
        /// Ban manager zaduzen za banovanje korisnika i upis u fajl
        /// </summary>
        private static BanManager BanManager = BanManager.Instance;

        /// <summary>
        /// Banuje studenta sa zadatim imenom
        /// Samo za asistente
        /// </summary>
        /// <param name="name">Ime studenta</param>
        [PrincipalPermission(SecurityAction.Demand, Role = "Asistent")]
        public void BanUser(string name)
        {
            try
            {
                Audit.UserBanned(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            BanManager.Ban(name);
        }
        /// <summary>
        /// Metoda koja vraca sve poruke klinetu
        /// Samo za asistente
        /// </summary>
        /// <returns>Sve poruke koje su do sada poslali studenti</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = "Asistent")]
        public List<Message> GetAllMessages()
        {
            return messages;
        }
        /// <summary>
        /// Metoda koja prihvata poslatu poruku i smesta je u listu poruka
        /// Samo za studente
        /// </summary>
        /// <param name="msg">Poruka studenta</param>
        [PrincipalPermission(SecurityAction.Demand, Role = "Student")]
        public void SendMessage(byte[] msg)
        {
            string key = String.Empty;
            if (File.Exists(secretKeyPath))
            {
                key = SecretKey.LoadKey(secretKeyPath);
            }
            else
            {
                key = SecretKey.GenerateKey(AlgorithmType.AES);
                SecretKey.StoreKey(key, secretKeyPath);
            }
            // decrypt message
            var decryptedMsg = AESManager.Decrypt(msg, key);
            var message = new Message();
            message.Student = Thread.CurrentPrincipal.Identity.Name;
            message.Text = decryptedMsg;
            messages.Add(message);
        }
        /// <summary>
        /// Metoda koja odblokira korisnika sa zadatim imenom
        /// Samo za asistente
        /// </summary>
        /// <param name="name">Ime studenta</param>
        [PrincipalPermission(SecurityAction.Demand, Role = "Asistent")]
        public void UnbanUser(string name)
        {
            try
            {
                Audit.UserUnbanned(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            BanManager.Unban(name);
        }
        /// <summary>
        /// Dobavlja sve poruke cija velicina prelazi 50
        /// Samo za asistente
        /// </summary>
        /// <returns>Sve poruke sa vise od 50 karaktera</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = "Asistent")]
        public List<Message> GetAllMessagesThatExceedInSize()
        {
            return messages.FindAll(i => i.Text.Length > 50);
        }
        /// <summary>
        /// Dobijanje liste svih banovanih(blokiranih) studenata
        /// Samo za asistente
        /// </summary>
        /// <returns>Vraca sve blokirane studente</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = "Asistent")]
        public List<string> GetAllBanedStudents()
        {
            return BanManager.BannedList;
        }
    }
}
