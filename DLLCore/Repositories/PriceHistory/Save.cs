using DLLCore.DBContext.Entities.StockHistoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentitySample.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DLLCore.DBContext;
using Newtonsoft.Json;
using DLLCore.Utility;
using System.Data.Entity;

namespace DLLCore.Repositories.PriceHistory
{
    public class Save
    {
        ApplicationDbContext db;
        Token tokenModel;
        public Save()
        {
            db = new ApplicationDbContext();
            tokenModel = new Token();
        }

        public string Mytoken { get; set; }
        public bool UpdateSecurityTbl()
        {
            var tokenInDb = db.TokenTbl.Select(a => a.TokenInDb).FirstOrDefault();
            Mytoken = tokenInDb;
            List<SecurityHistory> objStock = null;
            using (var clientToken = new HttpClient())
            {
                var flage = false;
                while (flage == false)
                {
                    using (var client = new HttpClient())
                    {
                        var credentials1 = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}", clientToken)));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Mytoken);
                        client.BaseAddress = new Uri("https://data3.nadpco.com/api/v3/TS/RealTimeTradesToday");
                        //call objStock get all records
                        //get async to send a Get request
                        //put async tp send put request
                        var responceTask = client.GetAsync("");
                        responceTask.Wait();
                        //To store result of web api response.   
                        var result = responceTask.Result;
                        //If success received   
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<List<SecurityHistory>>();
                            readTask.Wait();
                            objStock = readTask.Result;
                            if (objStock != null)
                            {
                                flage = true;
                            }
                            else
                            {
                                var baseUlr = new Uri("https://data3.nadpco.com/api/v2/Token");
                                string username = "farhadi2941";
                                string password = "*F@rhAdi#$";
                                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password)));
                                clientToken.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                                var responceToken = clientToken.PostAsync(baseUlr.ToString(), null);
                                if (responceToken.Result.IsSuccessStatusCode)
                                {
                                    var resultToken = responceToken.Result.Content.ReadAsStringAsync();
                                    Mytoken = JsonConvert.DeserializeObject<APCOtoken>(resultToken.Result).token;
                                    Token tokenTbl = new Token
                                    {
                                        TokenInDb = Mytoken,
                                        Date = DateTime.Now,
                                    };
                                    var token = db.TokenTbl.ToList().FirstOrDefault();
                                    token.Date = tokenTbl.Date;
                                    token.TokenInDb = tokenTbl.TokenInDb;
                                    db.Entry(token).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var baseUlr = new Uri("https://data3.nadpco.com/api/v2/Token");
                            string username = "farhadi2941";
                            string password = "*F@rhAdi#$";
                            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password)));
                            clientToken.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                            var responceToken = clientToken.PostAsync(baseUlr.ToString(), null);
                            if (responceToken.Result.IsSuccessStatusCode)
                            {
                                var resultToken = responceToken.Result.Content.ReadAsStringAsync();
                                Mytoken = JsonConvert.DeserializeObject<APCOtoken>(resultToken.Result).token;
                                Token tokenTbl = new Token
                                {
                                    TokenInDb = Mytoken,
                                };
                                var token = db.TokenTbl.ToList().FirstOrDefault();
                                token.Date = tokenTbl.Date;
                                token.TokenInDb = tokenTbl.TokenInDb;
                                db.Entry(token).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            if (objStock.Count != 0)
            {
                foreach (var item in objStock)
                {
                    long tafsilCodeInDb = db.SecurityTbl.Where(a => a.CoID == item.CoID).Select(a => a.TafsilCodeInCds).FirstOrDefault() ?? 0;
                    var record = db.SecurityTbl.Where(a => a.CoID == item.CoID).ToList().FirstOrDefault();
                    if (record != null)
                    {
                        int id = record.Id;
                        record = item;
                        record.TafsilCodeInCds = tafsilCodeInDb;
                        record.Id = id;
                        var local = db.Set<SecurityHistory>()
                       .Local
                       .FirstOrDefault(f => f.Id == record.Id);
                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return true;
        }

        public void UpdateTafsileCode()
        {
            //string symbol = "پارسان";
            //long tafsilCode = 7000160;
            //var securityRow = db.SecurityTbl.Where(a => a.BourseSymbol == symbol).FirstOrDefault();
            //securityRow.TafsilCodeInCds = tafsilCode;
            //db.Entry(securityRow).State = EntityState.Modified;
            //db.SaveChanges();
        }
    }
}