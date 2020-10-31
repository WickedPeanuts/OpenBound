using Avatar_API.Data.Models;
using Castle.Core.Logging;
using Microsoft.Data.SqlClient;
using OpenBound_Network_Object_Library.Database.Context;
using OpenBound_Network_Object_Library.Database.Controller;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avatar_API.Data.Services
{
    public class DonationTransactionService
    {
        private readonly OpenBoundDatabaseContext _odc;
        public DonationTransactionService(OpenBoundDatabaseContext odc)
        {
            _odc = odc;
        }

        public bool CheckConnection()
        {
            try
            {
                _odc.Database.CanConnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private Player GetDonationplayer(string email, string nickname)
        {
            return _odc.Players.FirstOrDefault((x) => x.Email.ToLower() == email &&
                                                      x.Nickname.ToLower() == nickname);
        }

        public List<DonationTransaction> GetPendingTransactions()
        {
            return _odc.DonationTransactions
                    .Where(x => x.TransactionStatus == TransactionStatus.Pending)
                    .ToList();
        }

        public void Create(DonationTransaction dt, string email, string nickname)
        {
            Player p = GetDonationplayer(email, nickname);
            dt.Player = p;
            _odc.DonationTransactions.Add(dt);
            _odc.SaveChanges();
        }

        public void Deliver(DonationTransaction dt)
        {
            if (dt == null || dt.TransactionStatus == TransactionStatus.Delivered)
                return;
            Player p = dt.Player;
            p.Cash += dt.CashAmount;
            dt.TransactionStatus = TransactionStatus.Delivered;
            dt.UpdatedAt = DateTime.Now;
            _odc.SaveChanges();
            Console.WriteLine($"Delivered Cash {dt}");
        }

        public void DeliverByPaypalId(string paypalTransactionId)
        {
            DonationTransaction dt = _odc.DonationTransactions.Where(x => x.GatewayTransactionId == paypalTransactionId).FirstOrDefault();
            Deliver(dt);
        }

        public void CreateAndDeliver(DonationTransaction dt, string email, string nickname)
        {
            Player p = GetDonationplayer(email, nickname);
            dt.Player = p;
            p.Cash += dt.CashAmount;
            dt.TransactionStatus = TransactionStatus.Delivered;
            dt.UpdatedAt = DateTime.Now;
            _odc.DonationTransactions.Add(dt);
            _odc.SaveChanges();
        }
    }
}
