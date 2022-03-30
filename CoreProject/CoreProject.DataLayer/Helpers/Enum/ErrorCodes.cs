namespace CoreProject.DataLayer.Helpers.Enum
{
    public class ErrorCodes
    {
        public static ErrorModel BilinmeyenHata { get { return   new ErrorModel { Code = 1, Text = "Bilinmeyen Bir Hata Oluştu." }; } }
        public static ErrorModel AyniMailUyelikMevcut { get { return new ErrorModel { Code = 2, Text = "Aynı Mail Adresi İle Mevcut Üyelik Bulunmaktadır." }; } }
        public static ErrorModel MailVeyaSifreHatali { get { return new ErrorModel { Code = 1, Text = "Mail veya Sifre Hatali." }; } }
        public static ErrorModel KayitYok { get { return new ErrorModel { Code = 1, Text = "Kayit Bulunamadi" }; } }
        public static ErrorModel WrongParameter { get { return new ErrorModel { Code = 1, Text = "Yanlıs Parametre" }; } }

    }

    public class ErrorModel
    {
        public int Code { get; set; }
        public string Text { get; set; }
    }
}
