using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace liblauncher.Login
{
    public class LoginThread
    {
        public delegate void LoginFinishEventHandler(LoginInfo loginInfo);

        public event LoginFinishEventHandler LoginFinishEvent;


        private LoginInfo _loginans;
        private readonly string _username;
        private readonly string _password;
        private readonly object _auth;


        protected void OnLoginFinishEvent(LoginInfo logininfo)
        {
            LoginFinishEventHandler handler = LoginFinishEvent;
        }

        public LoginThread(string username, string password, string auth, int selectIndex)
        {
            this._username = username;
            this._password = password;
            if (selectIndex != 0)
            {
                //this._auth = Core.Auths[auth];
            }
        }

        public void Start()
        {
            var thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            if (_auth != null)
            {
                Type T = _auth.GetType();
                MethodInfo login = T.GetMethod("Login");
                try
                {
                    object loginansobj = login.Invoke(_auth,
                        new object[] { _username, _password, System.Guid.NewGuid().ToString(), "zh-cn" });
                    Type li = loginansobj.GetType();
                    _loginans.Suc = (bool)li.GetField("Suc").GetValue(loginansobj);
                    if (_loginans.Suc)
                    {
                        _loginans.UN = li.GetField("UN").GetValue(loginansobj) as string;
                        _loginans.SID = li.GetField("SID").GetValue(loginansobj) as string;
                        _loginans.Client_identifier =
                            li.GetField("Client_identifier").GetValue(loginansobj) as string;
                        _loginans.UID = li.GetField("UID").GetValue(loginansobj) as string;
                        _loginans.OtherInfo = li.GetField("OtherInfo").GetValue(loginansobj) as string;
                        if (li.GetField("OutInfo") != null)
                        {
                            _loginans.OutInfo = li.GetField("OutInfo").GetValue(loginansobj) as string;
                        }
                        OnLoginFinishEvent(_loginans);
                    }
                    else
                    {
                        _loginans.Errinfo = li.GetField("Errinfo").GetValue(loginansobj) as string;
                        _loginans.OtherInfo = li.GetField("OtherInfo").GetValue(loginansobj) as string;
                        OnLoginFinishEvent(_loginans);
                    }
                }
                catch (Exception ex)
                {
                    _loginans.Suc = false;
                    _loginans.Errinfo = ex.Message;
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        _loginans.Errinfo += "\n" + ex.Message;
                    }
                    OnLoginFinishEvent(_loginans);
                }
            }
            else
            {
                _loginans.Suc = true;
                _loginans.SID = Core.Config.GUID;
                _loginans.UN = this._username;
                OnLoginFinishEvent(_loginans);
            }
        }

    }
}
