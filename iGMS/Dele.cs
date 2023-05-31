using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner
{
    public class Dele 
    {    
        public static void DeleteUsers(int idUser)
        {
            Entities db = new Entities();
            var user = db.Users.Find(idUser);
            db.Users.Remove(user);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteFXconnects(string idFXconnects)
        {
            Entities db = new Entities();
            var countUser = db.Users.Where(x => x.IdFX == idFXconnects).ToList();
            db.Users.RemoveRange(countUser);
            var fXconnect = db.FXconnects.Find(idFXconnects);
            db.FXconnects.Remove(fXconnect);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        public static void DeleteReports(string idReports)
        {
            Entities db = new Entities();
            var countData = db.Data.Where(x => x.IdReports == idReports).ToList();
            db.Data.RemoveRange(countData);
            var countDataSP = db.DataScanPhysicals.Where(x => x.IdReports == idReports).ToList();
            db.DataScanPhysicals.RemoveRange(countDataSP);
            var countGenerals = db.Generals.Where(x => x.IdReports == idReports).ToList();
            db.Generals.RemoveRange(countGenerals);
            var countDiscrepancies = db.Discrepancies.Where(x => x.IdReports == idReports).ToList();
            db.Discrepancies.RemoveRange(countDiscrepancies);
            var report = db.Reports.Find(idReports);
            db.Reports.Remove(report);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteDiscrepancies(string idReports)
        {
            Entities db = new Entities();
            var countDiscrepancies = db.Discrepancies.Where(x => x.IdReports == idReports).ToList();
            db.Discrepancies.RemoveRange(countDiscrepancies);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteGenerals(string idReports)
        {
            Entities db = new Entities();
            var countGenerals = db.Generals.Where(x => x.IdReports == idReports).ToList();
            db.Generals.RemoveRange(countGenerals);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteData(string idData)
        {
            Entities db = new Entities();
            var datum = db.Data.Find(idData);
            db.Data.Remove(datum);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteDataSP(int idDataSP)
        {
            Entities db = new Entities();
            var dataScanPhysical = db.DataScanPhysicals.Find(idDataSP);
            db.DataScanPhysicals.Remove(dataScanPhysical);
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
        public static void DeleteDetailEPCs(string FX)
        {
            Entities db = new Entities();
            var countDeEpc = db.DetailEpcs.Where(x => x.IdEPC.Length > 0 && x.IdFX == FX).ToList();
                db.DetailEpcs.RemoveRange(countDeEpc);
                db.SaveChanges();
                bool saveFailed;
                do
                {
                    saveFailed = false;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Update the values of the entity that failed to save from the store
                        ex.Entries.Single().Reload();
                    }

                } while (saveFailed);
        }


    }
}