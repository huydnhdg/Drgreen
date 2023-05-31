using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TichdiemDrGreen.Utlis
{
    public class SmsTemp
    {
        public static string SYNTAX_INVALID(string chanel)
        {

            string mtReturn = string.Format("Tin nhan cua Quy khach sai cu phap. Vui long soan: DRG [dau cach] [Ma Cao] gui 8077. hoac lien he 0936002006 de duoc ho tro. Tran trong.");

            return mtReturn;

        }
        public static string SYNTAX_INVALID_INFOR(string chanel)
        {

            string mtReturn = string.Format("De tham gia chuong trinh tich diem cua DrGreen, QK vui long soan: DRG[cach]Ho_ten[cach]So DT[cach]Dia_chi gui 8077. Lien he: 0936002006 de duoc ho tro.");

            return mtReturn;

        }
        public static string CODE_NOTVALID(string chanel, string code)
        {

            string mtReturn = string.Format("Ma san pham : {0} khong dung. Quy khach vui long kiem tra lai ma san pham hoac lien he: 0936002006. Tran trong.", code);


            return mtReturn;

        }
        public static string CODE_USED(string chanel,  string time, string date)
        {

            string mtReturn = string.Format("Ma so cua san pham da duoc xac thuc vao ngay {0} {1}. Hotline: 0936002006. Tran trong.", time, date);

            return mtReturn;

        }
        public static string CODE_SUCCESS(string chanel, string code )
        {
            if (chanel == "SMS")
            {
                string mtReturn = string.Format("De tham gia chuong trinh tich diem cua DrGreen, QK vui long soan: DRG[cach]Ho_ten[cach]So DT[cach]Dia_chi gui 8077. Lien he: 0936002006 de duoc ho tro.", code);
                return mtReturn;
            }    
            else
            {
                string mtReturn = string.Format("SP chinh hang do Cty TNHH Dr Natural VN phan phoi, QK da tich duoc 1 diem. Hay tich them x hop nua de nhan chau gap gon vao. LH 0936002006 de duoc huong dan.", code);
                return mtReturn;
            }    



        }
        public static string VALID_USER(string chanel)
        {

            string mtReturn = string.Format("SP chinh hang do Cty TNHH Dr Natural VN phan phoi, QK da tich duoc 1 diem. Hay tich them x hop nua de nhan chau gap gon vao. LH 0936002006 de duoc huong dan.");

            return mtReturn;

        }
        public static string LACK_OF_POINT(string chanel)
        {

            string mtReturn = string.Format("Rat tiec ban chua du diem, hay tiep tuc tich diem de doi qua. Chi tiet qua tang lien he Hotline 0962728448 hoac truy cap tichdiem.binhruamui.com");

            return mtReturn;

        }
        public static string FIVE_POINT(string chanel)
        {

            string mtReturn = string.Format("Doi 1 XIT HONG KEO ONG soan DRG CQ A1; Doi 1 O soan DRG CQ A2 gui 8077. LH: 0936002006 hoac tichdiem.binhruamui.com");

            return mtReturn;

        }
        public static string TEN_POINT(string chanel)
        {

            string mtReturn = string.Format("Doi 1 AO MUA soan DRG CQ A3; Doi 1 THE HOC soan DRG CQ A4 gui 8077. LH: 0936002006 hoac tichdiem.binhruamui.com");

            return mtReturn;

        }
        public static string FIFTEEN_POINT(string chanel)
        {

            string mtReturn = string.Format("Doi 1 NUOC GIAT soan DRG CQ A5; Doi 1 CHAO CHONG DINH soan DRG CQ A6 gui 8077. LH: 0936002006 hoac tichdiem.binhruamui.com");

            return mtReturn;

        }
        public static string TWENTY_POINT(string chanel)
        {

            string mtReturn = string.Format("Doi 1 TUI DU LICH soan DRG CQ A7; Doi 1 VALI soan DRG CQ A8 gui 8077. LH: 0936002006 hoac tichdiem.binhruamui.com");

            return mtReturn;

        }
        public static string TRA_CUU(string chanel, string point)
        {

            string mtReturn = string.Format("So diem cua quy khach la {0} diem, hay tiep tuc tich diem de nhan qua khi mua DrGreenn. LH Hotline: 0936002006 khi can ho tro. Cam on QK" , point);

            return mtReturn;

        }
        public static string UPDATE_INFOR(string chanel)
        {

            string mtReturn = string.Format("SP chinh hang do Cty TNHH Dr Natural VN phan phoi, QK da tich duoc 1 diem. Hay tich them x hop nua de nhan chau gap gon va o. LH 0936002006 de duoc huong dan.");			


            return mtReturn;

        }
        public static string VALID_INFOR(string chanel)
        {

            string mtReturn = string.Format("Thong tin da ton tai");


            return mtReturn;

        }
        public static string GET_FIVE_POINT(string chanel)
        {

            string mtReturn = string.Format("Chuc mung ban co 5 diem. De doi 1 XIT HONG KEO ONG hoac 1 O soan tin: DRG DOIQUA gui 8077. Hoac tich diem them de doi qua gia tri hon.");


            return mtReturn;

        }
        public static string GET_TEN_POINT(string chanel)
        {

            string mtReturn = string.Format("Chuc mung ban co 10 diem. De doi 1 AO MUA hoac 1 THE HOC soan tin: DRG DOIQUA gui 8077. Hoac tich diem them de doi qua gia tri hon.");


            return mtReturn;

        }
        public static string GET_FIFTEEN_POINT(string chanel)
        {

            string mtReturn = string.Format("Chuc mung ban co 15 diem. De doi 1 NUOC GIAT hoac 1 CHAO CHONG DINH soan tin: DRG DOIQUA gui 8077. Hoac tich diem them de doi qua gia tri hon.");


            return mtReturn;

        }
        public static string GET_TWENTY_POINT(string chanel)
        {

            string mtReturn = string.Format("Chuc mung ban co 20 diem. De doi 1 TUI DU LICH hoac 1 VALI soan tin: DRG DOIQUA gui 8077. Hoac tich diem them de doi qua gia tri hon.");


            return mtReturn;

        }
        public static string DOIQUA(string chanel)
        {

            string mtReturn = string.Format("Chuc mung ban da doi qua thanh cong. Qua se chuyen den trong thoi gian som nhat. Xem them: tichdiem.binhruamui.com Hotline: 0936002006");


            return mtReturn;

        }
        public static string ISNULLMC (string chanel)
        {

            string mtReturn = string.Format("Ma Cao cua san pham da duoc xac thuc vao ngay [ngay_xacthuc]. LH Hotline: 0936002006 khi can ho tro. Cam on QK.");


            return mtReturn;

        }
        public static string ISNULL(string chanel)
        {

            string mtReturn = string.Format("Ma Cao san pham cua ban khong ton tai. Vui long kiem tra lai va lien he Hotline: 0936002006 Cam on QK.");


            return mtReturn;

        }
        public static string ISNULLA(string chanel)
        {

            string mtReturn = string.Format("Ma Cao san pham cua ban khong ton tai. Vui long kiem tra lai va lien he Hotline: 0936002006 Cam on QK.");


            return mtReturn;

        }
        public static string ISNULLPHONE(string chanel)
        {

            string mtReturn = string.Format("So dien thoai cua quy khach chua tham gia chuong trinh tich diem khi mua Dr Green. LH Hotline: 0936002006 khi can ho tro. Cam on QK");


            return mtReturn;

        }
    }
}