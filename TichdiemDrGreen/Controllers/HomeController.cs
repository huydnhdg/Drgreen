using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TichdiemDrGreen.Models;
using TichdiemDrGreen.Utlis;


namespace TichdiemDrGreen.Controllers
{
    public class HomeController : Controller
    {
        Entities db = new Entities();

       
        string SENDER_ID = ConfigurationManager.AppSettings["SENDER_ID"];
        public ActionResult Index(string code="")
        {
            ViewBag.Code = code;
            return View();
        }

        [HttpPost]
        public JsonResult CallKichHoat(string phone1, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(phone1))//không nhập đủ dữ liệu
                {
                    return ReturnResult("Chưa gửi được mã dự thưởng. Vui lòng kiểm tra các thông tin và thực hiện gửi lại.");
                }

                code = code.ToUpper();


                phone1 = MyControl.formatUserId(phone1, 0);
                int phone1_length = phone1.Length;
                if (phone1_length != 11)//kiem tra so dien thoai co hop le hay k 84965433459
                {
                    //checking code
   
                        return ReturnResult("Số thuê bao không đúng định dạng, Quý khách vui lòng kiểm tra lại.");
                    
                }

                //checking code
                if (!ValidateCode(code))
                {
                    return ReturnResult("Mã dự thưởng không đúng. Vui lòng nhập lại.", code);
                }

                //Kich hoat                
                Result jsonResultend = CallAPI(phone1, code, null);
                if (jsonResultend.status == "3")//request thanh cong trung thuong the cao
                {
                    return ReturnResult("3");
                }
                else//request thanh cong
                {
                    return ReturnResult(jsonResultend.message);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = "false",
                    error = "system",
                    message = e.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CallTraCuu(string phone1, string point)
        {
            try
            {
                // if (string.IsNullOrEmpty(product) || string.IsNullOrEmpty(phone1))//không nhập đủ dữ liệu
                if (string.IsNullOrEmpty(phone1))//không nhập đủ dữ liệu
                {
                    return ReturnResult("Vui lòng kiểm tra các thông tin và thực hiện tra cứu lại.");
                }

                phone1 = MyControl.formatUserId(phone1, 0);
                int phone1_length = phone1.Length;
                if (phone1_length != 11)//kiem tra so dien thoai co hop le hay k 84965433459
                {
                    return ReturnResult("Số thuê bao không đúng định dạng, Quý khách vui lòng kiểm tra lại.");
                }

                //Kich hoat                
                Result jsonResultend = CallAPI(phone1, "DIEM", null);
                if (jsonResultend.status == "3")//request thanh cong trung thuong the cao
                {
                    return ReturnResult("3");
                }
                else//request thanh cong
                {
                    //return Json(new
                    //{
                    //    success = true,
                    //    point = jsonResultend.Point,
                    //    message = jsonResultend.message,
                    //}, JsonRequestBehavior.AllowGet);

                    return ReturnResult(jsonResultend.message);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = "false",
                    error = "system",
                    message = e.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CallTraQua(string phone1,  string maqua)
        {
            try
            {
                // if (string.IsNullOrEmpty(product) || string.IsNullOrEmpty(phone1))//không nhập đủ dữ liệu
                if (string.IsNullOrEmpty(phone1))//không nhập đủ dữ liệu
                {
                    return ReturnResult("Vui lòng kiểm tra các thông tin và thực hiện tra cứu lại.");
                }

                phone1 = MyControl.formatUserId(phone1, 0);
                int phone1_length = phone1.Length;
                if (phone1_length != 11)//kiem tra so dien thoai co hop le hay k 84965433459
                {
                    return ReturnResult("Số thuê bao không đúng định dạng, Quý khách vui lòng kiểm tra lại.");
                }

                //Kich hoat                
                Result jsonResultend = CallAPI(phone1,"CQ", maqua);
                return ReturnResult(jsonResultend.message);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = "false",
                    error = "system",
                    message = e.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private Boolean ValidateCode(String code)
        {
            //checking code
            var isDigitPresent = code.Any(c => char.IsDigit(c));
            var isLetterPresent = code.Any(c => char.IsLetter(c));
            //if (code.Length != 10 || !isDigitPresent || !isLetterPresent)
            return true;
        }




        private Result CallAPI(string Phone, string Code, string Type)
        {
            string domain = Request.Url.Authority;

            // String url = "http://quatang.gpfrance.vn/api/sms/receive?Command_Code=GP&Service_ID=8077&Request_ID=WEB" + DateTime.Now.Ticks +"&User_ID=";
            String url = "https://" + domain + "/api/sms/receive?" + "Command_Code=DRG&Service_ID=8077&Request_ID=WEB" + "&User_ID=";
            if (Type == null)
            {
                WebRequest request = WebRequest.Create(
                           url + Phone + "&Message=DRG " + Code );
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Result result = JsonConvert.DeserializeObject<Result>(responseFromServer);
                reader.Close();
                response.Close();
                return result;
            }
            else
            {
                WebRequest request = WebRequest.Create(
               url + Phone + "&Message=DRG " + Code + " "+ Type);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Result result = JsonConvert.DeserializeObject<Result>(responseFromServer);
                reader.Close();
                response.Close();
                return result;
            }    

     
                
        }

        private JsonResult ReturnResult(string message, string type = "system")
         {
            return Json(new
            {
                success = true,
                type = type,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }
    
}
}