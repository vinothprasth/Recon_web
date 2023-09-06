using Microsoft.AspNetCore.Mvc;
using System.Data;
using ReconWeb.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net.Mail;
using Recon_Model;
using static Recon.Controllers.LoginController;
using System.Net.Http.Headers;
using static Recon_Model.user_model;

namespace Recon.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CommonController _commonController;
        string ipAddress = "";

        public LoginController(
            CommonController commonController,
            IConfiguration configuration,
            ILogger<LoginController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _commonController = commonController;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login()
        {
            return View();
        }
        public class Login_model1
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }


        [HttpPost]
       public string Login_validation([FromBody] Login_model1 model)
        {
            string post_data = "";
            DataTable result = new DataTable();
            List<user_model> objcat_lst = new List<user_model>();
            try
            {
                string hostName = Dns.GetHostName();
                ipAddress = Dns.GetHostAddresses(hostName)[0].ToString();

                var pass = Encrypt(model.Password);

                Login_model loginmodel = new Login_model();
                loginmodel.user_id = model.UserName;
                loginmodel.password = pass;
                loginmodel.ip = ipAddress;
                loginmodel.msg = "";
                loginmodel.ip_address = "";
                loginmodel.datasource = "";
                loginmodel.user_code = "";
                loginmodel.user_name = "";
                loginmodel.oldpassword = "";
                string post_data1 = _commonController.GetApiResult(JsonConvert.SerializeObject(loginmodel), "Loginvalidation");
                string d2 = JsonConvert.DeserializeObject<string>(post_data1);
                result = JsonConvert.DeserializeObject<DataTable>(d2);
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    user_model objcat = new user_model();
                    objcat.user_gid = Convert.ToInt32(result.Rows[i]["user_gid"]);
                    objcat.user_name = result.Rows[i]["user_name"].ToString();
                    objcat.passwordexpdate = result.Rows[i]["password_expiry_date"].ToString();
                    objcat.usergroup_gid = Convert.ToInt32(result.Rows[i]["usergroup_code"]);
                    objcat.result = Convert.ToInt32(result.Rows[i]["out_result"]);
                    objcat.msg = result.Rows[i]["out_msg"].ToString();
                    objcat.oldpassworrd = Decrypt(pass);
                    objcat.user_status = result.Rows[i]["user_status"].ToString();
                    objcat_lst.Add(objcat);

                    HttpContext.Session.SetString("usercode", model.UserName);
                    HttpContext.Session.SetString("username", result.Rows[i]["user_name"].ToString());
                    HttpContext.Session.SetString("mindate", result.Rows[i]["min_tran_date"].ToString());
                    HttpContext.Session.SetString("fin_date", result.Rows[i]["fin_start_date"].ToString());
                    HttpContext.Session.SetString("user_code", result.Rows[i]["user_gid"].ToString());
                    HttpContext.Session.SetString("usergroup_code", result.Rows[i]["usergroup_code"].ToString());
                    HttpContext.Session.SetString("userrole", "ADMIN");

                }


            }
            catch (Exception ex)
            {
                string control = this.RouteData.Values["controller"].ToString();
                _logger.LogError(ex, $"An error occurred in {control}");
            }

            return JsonConvert.SerializeObject(objcat_lst);
        }


        private string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return cipherText;
        }

        //public ActionResult Forgot_pass(string UserName)
        //{
        //    string message = "";
        //    try
        //    {
        //        string hostName = Dns.GetHostName(); // Retrive the Name of HOST 
        //        ipAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
        //        List<user_model> objcat_lst = new List<user_model>();
        //        DataTable result = new DataTable();

        //        Login_model Forgot_pass = new Login_model();
        //        Forgot_pass.user_id = UserName;
        //        Forgot_pass.ip = ipAddress;

        //        using (var client = new HttpClient())
        //        {

        //            //string post_data = _commonController.getApiResult(JsonConvert.SerializeObject(Forgot_pass), "forgotpassword");
        //            //result = (DataTable)JsonConvert.DeserializeObject(post_data, result.GetType());
        //            for (int i = 0; i < result.Rows.Count; i++)
        //            {
        //                user_model objcat = new user_model();
        //                objcat.user_emailid = result.Rows[i]["user_emailid"].ToString();
        //                var password = result.Rows[i]["user_password"].ToString();
        //                objcat.oldpassworrd = Decrypt(password);
        //                objcat_lst.Add(objcat);
        //                objcat.FromEmailId = System.Configuration.ConfigurationManager.AppSettings["FromEmailId"];
        //                objcat.Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
        //                objcat.SMTPPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
        //                objcat.Host = System.Configuration.ConfigurationManager.AppSettings["Host"];

        //                if (objcat.user_emailid != "")
        //                {
        //                    var sub = "Password";
        //                    var body = objcat.oldpassworrd;
        //                    var smtp = new SmtpClient
        //                    {
        //                        Host = objcat.Host,
        //                        Port = objcat.SMTPPort,
        //                        EnableSsl = true,
        //                        DeliveryMethod = SmtpDeliveryMethod.Network,
        //                        UseDefaultCredentials = false,
        //                        Credentials = new NetworkCredential(objcat.FromEmailId, objcat.Password)
        //                    };
        //                    using (var mess = new MailMessage(objcat.FromEmailId, objcat.user_emailid)
        //                    {
        //                        Subject = sub,
        //                        Body = body
        //                    })
        //                    {
        //                        smtp.Send(mess);
        //                        message = "Mail Sended Sucessfully";
        //                    }
        //                }
        //                else
        //                {
        //                    message = "failed";
        //                }
        //            }
        //        }
        //        return new JsonResult(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        //string error = ex.ToString();
        //        //message = error;
        //        //string control = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        //_logger.LogError(ex.ToString(), control);
        //        //return View();
        //    }
        //}

        //public ActionResult session_expires()
        //{


        //    return View();
        //}
    }
}
