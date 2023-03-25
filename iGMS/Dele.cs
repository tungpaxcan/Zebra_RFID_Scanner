using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using CollTex.Models;

namespace CollTex
{
    public class Dele 
    {
        public static void DeleteGoods(string idGoods)
        {   
            ColltexEntities db = new ColltexEntities();
            var good = db.Goods.Find(idGoods);
            db.Goods.Remove(good);
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
        public static void DeleteUsers(int idUser)
        {
            ColltexEntities db = new ColltexEntities();
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
        public static void DeleteColors(string idColor)
        {
            ColltexEntities db = new ColltexEntities();
            var countGood = db.Goods.Where(x => x.IdColor == idColor).Count();
            for (int i = 0; i < countGood; i++)
            {
                var idGood = db.Goods.OrderBy(x => x.IdColor == idColor).ToList().LastOrDefault().Id;
                DeleteGoods(idGood);
            }
            var color = db.Colors.Find(idColor);
            db.Colors.Remove(color);
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
        public static void DeleteSizes(string idSize)
        {
            ColltexEntities db = new ColltexEntities();
            var countGood = db.Goods.Where(x => x.IdSize == idSize).Count();
            for (int i = 0; i < countGood; i++)
            {
                var idGood = db.Goods.OrderBy(x => x.IdSize == idSize).ToList().LastOrDefault().Id;
                DeleteGoods(idGood);
            }
            var size = db.Sizes.Find(idSize);
            db.Sizes.Remove(size);
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
        public static void DeleteStyles(string idStyle)
        {
            ColltexEntities db = new ColltexEntities();
            var countGood = db.Goods.Where(x => x.IdStyle == idStyle).Count();
            for (int i = 0; i < countGood; i++)
            {
                var idGood = db.Goods.OrderBy(x => x.IdStyle == idStyle).ToList().LastOrDefault().Id;
                DeleteGoods(idGood);
            }
            var style = db.Styles.Find(idStyle);
            db.Styles.Remove(style);
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
            ColltexEntities db = new ColltexEntities();
            var countUser = db.Users.Where(x => x.IdFX == idFXconnects).Count();
            for (int i = 0; i < countUser; i++)
            {
                var idUser = db.Users.OrderBy(x => x.IdFX == idFXconnects).ToList().LastOrDefault().Id;
                DeleteUsers(idUser);
            }
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
        public static void DeleteDetailEPCs(string FX)
        {
            ColltexEntities db = new ColltexEntities();
            var countDeEpc = db.DetailEPCs.Where(x => x.IdEPC.Length>0 && x.IdFX==FX).Count();
            for (int i = 0; i < countDeEpc; i++)
            {
                var idDeEpc = db.DetailEPCs.OrderBy(x => x.IdEPC.Length > 0 && x.IdFX == FX).ToList().LastOrDefault().IdEPC;
                var DeEpc = db.DetailEPCs.Find(idDeEpc);
                db.DetailEPCs.Remove(DeEpc);
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
}