using System;

namespace BulkSms
{
    public class Constants
    {

        public static int SIGNAL_NORMAL_UPDATE = 24;
        public static int SIGNAL_SYNC_UPDATE = 25;
        public static int SIGNAL_SUCCESS_UPDATE = 26;
        public static int SIGNAL_FAILURE_UPDATE = 27;

        public static String BULK_SMS_SERVER_URL_TO_SEND_MESSAGE = "https://control.msg91.com/api/postsms.php";
        public static String BULK_SMS_AUTH_KEY = "134455AXvXXU6l58597ff2";
        public static String BULK_SMS_CAMPAIGN = "Bulk SMS";
        public static String BULK_SMS_ROUTE = "4";
        public static String BULK_SMS_COUNTRY = "0";

        public static String MYSQL_SERVER = "cdcsweb.in";
        public static int MYSQL_SERVER_PORT = 3306;
        public static String MYSQL_SERVER_WORKING_DB = "cdcsin_bulksmsdb";
        public static String MYSQL_USER_BSMSUSER = "cdcsin_bsmsuser";
        public static String MYSQL_USER_PASSWORD_BSMSUSER = "cdcs2017cdcs";

        public static String DATA_PATH = new IniFile("bulksms.ini").Read("data_location", "APP").Trim();
        public static String DBF_FILE_NAME = new IniFile("bulksms.ini").Read("tbf_file_name", "APP").Trim();
        public static String LOGIN_USERNAME = new IniFile("bulksms.ini").Read("username", "APP").Trim();
        public static String LOGIN_PASSWORD = new IniFile("bulksms.ini").Read("password", "APP").Trim();
        public static String SENDER_ID = new IniFile("bulksms.ini").Read("sender_id", "APP").Trim();
        public static String SERVER_IP = new IniFile("bulksms.ini").Read("server_ip", "APP").Trim();
        public static String SERVER_PORT = new IniFile("bulksms.ini").Read("server_port", "APP").Trim();

    }
}
