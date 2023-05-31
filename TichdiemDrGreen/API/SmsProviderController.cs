using Newtonsoft.Json;
using NLog;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.UI.WebControls;
using TichdiemDrGreen.Models;
using TichdiemDrGreen.Utlis;

namespace TichdiemDrGreen.API
{
    [RoutePrefix("api/sms")]
    public class SmsProviderController : ApiController
    {

        private static object syncObject = new object();
        Logger log = LogManager.GetCurrentClassLogger();
        Entities db = new Entities();
        private readonly int NUMBER_OVER_QUOTA = 36;
        private readonly int NUMBER_OVER_QUOTA_IN_MONTH = 6;

        [Route("receive")]
        [HttpGet]
        public HttpResponseMessage Receive(string Command_Code, string User_ID, string Service_ID, string Request_ID, string Message, string Address="")
        {
            //https://localhost:44361/api/sms/receive?command_code=drg&user_id=6566668875&request_id=xx&service_id=xx&message=drg%202AYML
            //http://web02.topteen.vn//api/sms/receive?command_code=drg&user_id=6566668875&request_id=xx&service_id=xx&message=drg%2029ST2
            // Lưu log file

            log.Info(string.Format("[MO] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, Message));
            User_ID = MyControl.formatUserId(User_ID, 0);
            //tách tin nhắn của khách hàng nhắn lên
            string[] subs = Message.Split(' ');
            
            string mt_trakhachhang = "";
            string category = "TC";
            string chanel = "SMS";
            
            int status = 0; // 0 - thành công, 1- thất bại, 2 - tra cứu, 3 - sai cú pháp, 4 - trùng mã thẻ, 5 - Không đủ điểm ,6- đủ 5 điểm, 7 - đủ 10 điểm, 8 - đủ 15 điểm, 9- đủ 20 điểm 
            if (subs.Length < 2)
            {
                //nhắn sai cú pháp
                mt_trakhachhang = SmsTemp.SYNTAX_INVALID(chanel);
                category = "SYNTAX_INVALID";
                status = 3;
                var resulter = new Result()
                {
                    status = "0",
                    message = mt_trakhachhang,
                    phone1 = User_ID
                };
                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                return ResponseMessage(resulter);
            }
            var LogMO = new LogMO()
            {
                Command_Code = Command_Code,
                Phone = User_ID,
                Service_ID = Service_ID,
                Request_ID = Request_ID,
                Message = Message,
                Createdate = DateTime.Now
            };
            db.LogMOes.Add(LogMO);
            db.SaveChanges();
            //Lưu vào DB
            long id = LogMO.ID;
            
            // Kết thúc ghi log vào DB
            if (Request_ID.ToUpper().StartsWith("WEB"))
            {
                chanel = "WEB";
            }
            try
            {
                lock (syncObject)
                {

               
                string code = subs[1];
                var codeGP = db.CodeGPs.Where(a => a.Code == code).SingleOrDefault();
                if (subs.Length == 2)
                {
                    if (subs[1].Equals("DOIQUA"))
                    {
                        //hd doi qua
                        // it hon 5 diem báo chưa đủ điểm
                        var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                        if(customer1.Point < 5)
                        {
                            mt_trakhachhang = SmsTemp.LACK_OF_POINT(chanel);
                            category = "LACK_OF_POINT";
                            status = 5;
                            var resulter = new Result()
                            {
                                status = "5",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        }
                        //đổi quà đủ 5 điểm
                        else if(customer1.Point == 5)
                        {
                            mt_trakhachhang = SmsTemp.FIVE_POINT(chanel);
                            category = "FIVE_POINT";
                            status = 6;
                            var resulter = new Result()
                            {
                                status = "6",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);

                        }
                        //đổi quà đủ 10 điểm
                        else if (customer1.Point == 10)
                        {
                            mt_trakhachhang = SmsTemp.TEN_POINT(chanel);
                            category = "TEN_POINT";
                            status = 7;
                            var resulter = new Result()
                            {
                                status = "7",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);

                        }
                        //đổi quà đủ 15 điểm
                        else if (customer1.Point == 15)
                        {
                            mt_trakhachhang = SmsTemp.FIFTEEN_POINT(chanel);
                            category = "FIFTEEN_POINT";
                            status = 8;
                            var resulter = new Result()
                            {
                                status = "8",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);

                        }
                        //đổi quà đủ 20 điểm
                        else if (customer1.Point == 20)
                        {
                            mt_trakhachhang = SmsTemp.TWENTY_POINT(chanel);
                            category = "TWENTY_POINT";
                            status = 9;
                            var resulter = new Result()
                            {
                                status = "9",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);

                        }     
                    }
                    //tra cuu diem
                    else if (subs[1].Equals("DIEM"))
                    {
                        var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                        if(customer1 != null)
                        {
                            var point = customer1.Point;
                            mt_trakhachhang = SmsTemp.TRA_CUU(chanel, point.ToString());
                            category = "TRA_CUU";
                            status = 2;
                            var resulter = new Result()
                            {
                                status = "2",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        }
                        else
                        {
                            mt_trakhachhang = SmsTemp.ISNULLPHONE(chanel);
                            category = "ISNULLPHONE";
                            status = 1;
                            var resulter = new Result()
                            {
                                status = "1",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        }    
                     
                    }
                    else 
                    {
                        //gach ma
                      
                        var cus = db.Customers.FirstOrDefault(c=>c.Phone == User_ID);
                        
                        if (codeGP == null)
                        {
                            mt_trakhachhang = SmsTemp.CODE_NOTVALID(chanel, subs[1]);
                            status = 1;
                            category = "ERROR_CODE";
                        }
                        else if (codeGP.Status == null)
                        {
 
                       
                            //gạch mã thẻ đi
                            codeGP.Status = 1;
                            codeGP.Activedate = DateTime.Now;
                            codeGP.Phone = User_ID;
                            db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            if(cus != null)
                            {
                                mt_trakhachhang = SmsTemp.VALID_USER(chanel);
                                status = 0;
                                cus.Phone = User_ID;
                                cus.Point = cus.Point + 1;
                                db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                            }
                            else
                            {
                                mt_trakhachhang = SmsTemp.CODE_SUCCESS(chanel, subs[1]);
                                status = 0;
                                var newcustomer = new Customer();
                                newcustomer.Phone = User_ID;
                                newcustomer.Createdate = DateTime.Now;
                                newcustomer.Point = 1;
                                newcustomer.DRG = codeGP.Category;
                                db.Customers.Add(newcustomer);
                                db.SaveChanges();
                            }    

                            var customer2 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            if(customer2.Point == 5)
                            {
                                mt_trakhachhang = SmsTemp.GET_FIVE_POINT(chanel);
                                status = 0;
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }  
                            else if (customer2.Point == 10)
                            {
                                mt_trakhachhang = SmsTemp.GET_TEN_POINT(chanel);
                                status = 0;
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }   
                            else if(customer2.Point == 15)
                            {
                                mt_trakhachhang = SmsTemp.GET_FIFTEEN_POINT(chanel);
                                status = 0;
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }   
                            else if(customer2.Point == 20)
                            {
                                mt_trakhachhang = SmsTemp.GET_TWENTY_POINT(chanel);
                                status = 0;
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }    
                        }
                        else
                        {
                            string time = codeGP.Activedate.Value.ToString("HH:mm");
                            string date = codeGP.Activedate.Value.ToString("dd/MM/yyyy");
                            mt_trakhachhang = SmsTemp.CODE_USED(chanel, time, date);
                            status = 4;
                        }
                    }
                }
                else if (subs.Length == 3)
                {
                    if (subs[1].Equals("CQ"))
                    {
                        //doi qua

                    
                            if (subs[2].Equals("A1") || subs[2].Equals("A2"))
                            {  
                                var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                                var model = new GiftExchange();
                                if(customer1.Point >= 5)
                                {
                                    mt_trakhachhang = SmsTemp.DOIQUA(chanel);
                                    status = 0;
                                    customer1.Point = customer1.Point - 5;
                                    model.GiftID = subs[2];
                                    if (model.GiftID == "A1")
                                    {
                                        model.Status = 0;
                                        model.Count = 1;
                                        model.Createdate = DateTime.Now;
                                        model.Product = customer1.DRG;
                                        model.Phone = User_ID;
                                        model.Gift_Name = "XIT HONG KEO ONG";
                                    }
                                    else
                                    {
                                        model.Status = 0;
                                        model.Count = 1;
                                        model.Createdate = DateTime.Now;
                                        model.Product = customer1.DRG;
                                        model.Phone = User_ID;
                                        model.Gift_Name = "O";
                                    }
                                    db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                    db.GiftExchanges.Add(model);
                                    db.SaveChanges();
                                    var resulter = new Result()
                                    {
                                        status = "0",
                                        message = mt_trakhachhang,
                                        phone1 = User_ID
                                    };
                                    log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                    return ResponseMessage(resulter);
                                }    
                                else
                                {

                                    mt_trakhachhang = SmsTemp.LACK_OF_POINT(chanel);
                                    category = "LACK_OF_POINT";
                                    status = 3;
                                    var resulter = new Result()
                                    {
                                        status = "3",
                                        message = mt_trakhachhang,
                                        phone1 = User_ID
                                    };
                                    log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                    return ResponseMessage(resulter);
                                }    
          
                            }
                            
                       
                        else if (subs[2].Equals("A3") || subs[2].Equals("A4"))
                        {
                            var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            var model = new GiftExchange();
                            if (customer1.Point >= 10)
                            {
                                mt_trakhachhang = SmsTemp.DOIQUA(chanel);
                                status = 0;

                                customer1.Point = customer1.Point - 10;
                                model.GiftID = subs[2];
                                if (model.GiftID == "A3")
                                {

                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "AO MUA";
                                }
                                else
                                {
                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "THE HOC";
                                }
                                db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                db.GiftExchanges.Add(model);
                                db.SaveChanges();
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }
                            else
                            {

                                mt_trakhachhang = SmsTemp.LACK_OF_POINT(chanel);
                                category = "LACK_OF_POINT";
                                status = 3;
                                var resulter = new Result()
                                {
                                    status = "3",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }    
                                
                        }
                        else if (subs[2].Equals("A5") || subs[2].Equals("A6"))
                        {
                            var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            var model = new GiftExchange();
                            if (customer1.Point>=15)
                            {
                                mt_trakhachhang = SmsTemp.DOIQUA(chanel);
                                status = 0;

                                customer1.Point = customer1.Point - 15;
                                model.GiftID = subs[2];
                                if (model.GiftID == "A5")
                                {
                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "NUOC GIAT";
                                }
                                else
                                {
                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "CHAO CHONG DINH";
                                }
                                db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                db.GiftExchanges.Add(model);
                                db.SaveChanges();
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }    
                           
                             else
                            {

                                mt_trakhachhang = SmsTemp.LACK_OF_POINT(chanel);
                                category = "LACK_OF_POINT";
                                status = 3;
                                var resulter = new Result()
                                {
                                    status = "3",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            }
                        }
                        else if (subs[2].Equals("A7") || subs[2].Equals("A8"))
                        {
                            var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            var model = new GiftExchange();
                            if(customer1.Point >= 20)
                            {
                                mt_trakhachhang = SmsTemp.DOIQUA(chanel);
                                status = 0;

                                customer1.Point = customer1.Point - 20;
                                model.GiftID = subs[2];
                                if (model.GiftID == "A7")
                                {
                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "TUI DU LICH";
                                }
                                else
                                {
                                    model.Status = 0;
                                    model.Count = 1;
                                    model.Createdate = DateTime.Now;
                                    model.Product = customer1.DRG;
                                    model.Phone = User_ID;
                                    model.Gift_Name = "VALI";
                                }
                                db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                db.GiftExchanges.Add(model);
                                db.SaveChanges();
                                var resulter = new Result()
                                {
                                    status = "0",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);
                            } 
                            else
                            {
                                mt_trakhachhang = SmsTemp.LACK_OF_POINT(chanel);
                                category = "LACK_OF_POINT";
                                status = 3;
                                var resulter = new Result()
                                {
                                    status = "3",
                                    message = mt_trakhachhang,
                                    phone1 = User_ID
                                };
                                log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                                return ResponseMessage(resulter);

                            }    
                           
                        }
     

                    }
                    else
                    {
                        //tra cuu
                        string code1 = subs[2];
                        var codeGP1 = db.CodeGPs.Where(a => a.Code == code1.Trim()).SingleOrDefault();
                        if (codeGP1.Activedate != null)
                        {
                            mt_trakhachhang = SmsTemp.ISNULLMC(chanel);
                            status = 2;
                            var resulter = new Result()
                            {
                                status = "2",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        } else if(codeGP1.Createdate == null)
                        {
                            mt_trakhachhang = SmsTemp.ISNULL(chanel);
                            status = 2;
                            var resulter = new Result()
                            {
                                status = "2",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        }
                        else if (codeGP1.Activedate == null)
                        {
                            mt_trakhachhang = SmsTemp.ISNULLA(chanel);
                            status = 2;
                            var resulter = new Result()
                            {
                                status = "2",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);
                        }
                    }
                }
                else if (subs.Length == 4)
                {
                        //cập nhật thông tin khách
                    if(subs.Length == 4)
                    {
                        var check_customer = db.Customers.FirstOrDefault(a => a.Phone == User_ID);
                        //if (check_customer != null)
                        //{
                        //    mt_trakhachhang = SmsTemp.VALID_INFOR(chanel);
                        //    status = 1;
                        //    category = "ERROR_CODE";
                        //    var resulter = new Result()
                        //    {
                        //        status = "1",
                        //        message = mt_trakhachhang,
                        //        phone1 = User_ID
                        //    };
                        //    log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                        //    return ResponseMessage(resulter);
                        //}
                        //else
                        
                            mt_trakhachhang = SmsTemp.UPDATE_INFOR(chanel);
                            status = 0;
                            var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            customer1.Name = subs[1];
                            customer1.Address = subs[3];
                            db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            var resulter = new Result()
                            {
                                status = "0",
                                message = mt_trakhachhang,
                                phone1 = User_ID
                            };
                            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                            return ResponseMessage(resulter);

                        

                    }
                    else
                    {
                        mt_trakhachhang = SmsTemp.SYNTAX_INVALID_INFOR(chanel);
                        status = 1;
                        category = "SYNTAX_INVALID";
                        var resulter = new Result()
                        {
                            status = "1",
                            message = mt_trakhachhang,
                            phone1 = User_ID
                        };
                        log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
                        return ResponseMessage(resulter);
                    }    
                   
                }

                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                mt_trakhachhang = "Co loi ngoai le xay ra";
                status = 3;
            }
            var response = new HttpResponseMessage();
            // Lưu log xử lý vào bảng MT

            var LogMT = new LogMT();
        
            {
                LogMT.MO_ID = id;
                LogMT.Phone = User_ID;
                LogMT.Createdate = DateTime.Now;
                Message = mt_trakhachhang;
                LogMT.Product = category;
                LogMT.Chanel = chanel;
                LogMT.Status = status;
            };
            db.LogMTs.Add(LogMT);
            db.SaveChanges();
         
            // Kết thúc lưu log MT
            var result = new Result()
            {
                status = "0",
                message = mt_trakhachhang,
                phone1 = User_ID
            };
            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
            return ResponseMessage(result);
        }
        private HttpResponseMessage ResponseMessage(Result result)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            return response;
        }

    }

}