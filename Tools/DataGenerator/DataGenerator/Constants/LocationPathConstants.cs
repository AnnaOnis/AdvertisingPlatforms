namespace DataGenerator.Constants
{
    public static class LocationPathConstants
    {
        public const string ROOT_LOCATION = "/ru";

        public static readonly Dictionary<string, List<string>> REGIONS = new Dictionary<string, List<string>>
        {
            ["mos"] = new List<string> { "msk", "pod", "khi", "bal", "krl" },     
            ["spb"] = new List<string> { "kol", "kro", "pus", "pet" },              
            ["kda"] = new List<string> { "krr", "soc", "nvs", "arm", "ana" },  
            ["svrd"] = new List<string> { "ekb", "ntg", "kam", "per", "vpy" },    
            ["tat"] = new List<string> { "kzn", "nch", "nkm", "alm", "zel" },      
            ["niz"] = new List<string> { "goj", "dze", "arz", "sar", "bor" },     
            ["nvs"] = new List<string> { "nsk", "ber", "kuy", "isk", "ob" },      
            ["kya"] = new List<string> { "kja", "nor", "acs", "ksk", "zhe" },     
            ["kry"] = new List<string> { "sip", "svp", "yal", "ker", "evp" },        
            ["sak"] = new List<string> { "uus", "kor", "kho", "oha" }                
        };
    }
}
//mos — Московская область
//• msk — Москва
//• pod — Подольск
//• khi — Химки
//• bal — Балашиха
//• krl — Королёв

//spb — Санкт-Петербург
//• spb — Санкт-Петербург
//• kol — Колпино
//• kro — Кронштадт
//• pus — Пушкин
//• pet — Петергоф

//kda — Краснодарский край
//• krr — Краснодар
//• soc — Сочи
//• nvs — Новороссийск
//• arm — Армавир
//• ana — Анапа

//svrd — Свердловская область
//• ekb — Екатеринбург
//• ntg — Нижний Тагил
//• kam — Каменск-Уральский
//• per — Первоуральск
//• vpy — Верхняя Пышма

//tat — Республика Татарстан
//• kzn — Казань
//• nch — Набережные Челны
//• nkm — Нижнекамск
//• alm — Альметьевск
//• zel — Зеленодольск

//niz — Нижегородская область
//• goj — Нижний Новгород
//• dze — Дзержинск
//• arz — Арзамас
//• sar — Саров
//• bor — Бор

//nvs — Новосибирская область
//• nsk — Новосибирск
//• ber — Бердск
//• kuy — Куйбышев
//• isk — Искитим
//• ob — Обь

//kya — Красноярский край
//• kja — Красноярск
//• nor — Норильск
//• acs — Ачинск
//• ksk — Канск
//• zhe — Железногорск

//kry — Республика Крым
//• sip — Симферополь
//• svp — Севастополь
//• yal — Ялта
//• ker — Керчь
//• evp — Евпатория

//sak — Сахалинская область
//• uus — Южно-Сахалинск
//• kor — Корсаков
//• kho — Холмск
//• oha — Оха
