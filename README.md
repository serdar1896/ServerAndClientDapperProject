# ServerAndClientDapperProject
Kullanılan Teknolojiler :
 .Net Core 
 DAPPER
 UnitOfWork
 CodeFirst
 Redis Cache
 AutoMapper
 AutoFac
 Swagger
 NLOG(log in database)
 Authentication
Proje Detayı
Customer ve Product bilgilerini işlemek için hazırlanan projede
2 birbirinden bağımsız yapı vardır.
SERVER tarafı ‘.Net Core API’ olarak yazılmış,
CLIENT ise ‘.Net Core MVC’ olarak yazılmış 

.Net Core ve Rest Api kullanılan Loglama işlemlerinin DB’de tutulduğu, 
UnitOfWork, GenericRepository ve fonksiyonel şekilde hazırlanan Dapper ile 
bir altyapı kurdum ve Swapper ile kullanıcı dostu bir API hazırladım. Front 
End kısmında .Net Core MVC kullandım. API bağlantılarını service bağlantısı 
kurarak kimi yerde AJAX çağrısı kullanarak gerçekleştirdim. .Net Core 
teknolejisine uygun Authentication işlemlerinin sağlandığı geliştirmeye 
devam ettiğim proje.
