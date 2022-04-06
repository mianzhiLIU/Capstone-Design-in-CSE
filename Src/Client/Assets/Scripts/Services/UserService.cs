using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;    // 定义 登录 事件
        public UnityEngine.Events.UnityAction<Result, string> OnRegister; // 定义 注册 事件
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;  // 定义 角色创建 事件

        NetMessage pendingMessage = null; // 消息队列，记录消息，等连接到服务器时，发送至服务器

        bool connected = false;


        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;            // 游戏服务器连接监听事件
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;      // 游戏服务器断开监听事件
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);         // 添加对用户登录消息响应的监听
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister); // 添加对用户注册消息响应的监听
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter); // 添加对用户创建角色消息响应的监听
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);       // 注销对用户登录消息响应的监听
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister); // 注销对用户注册消息响应的监听
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter); // 注销对用户创建角色消息响应的监听
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);   // 本地服务器 端口
            NetClient.Instance.Connect();                 // 连接服务器
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if (this.pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result, string reason)
        {
            if (this.pendingMessage != null)
            {
                // 用户登录错误
                if (this.pendingMessage.Request.userLogin != null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                // 用户注册错误
                else if(this.pendingMessage.Request.userRegister != null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                // 用户角色创建错误
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        // 发送登录信息
        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        // Login 登录事件，反馈至 UI 层 （Login Panel)
        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {//登陆成功逻辑
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            };
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
  
            }
        }

        // 发送注册信息
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);

            // 发送自定义消息
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            // 判断连接是否连接上
            if (this.connected && NetClient.Instance.Connected)
            {
                // 连接上，发送消息至服务器
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                // 没有连接上，重新连接至服务器
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        // Register 注册事件，反馈至 UI 层 （Register Panel)
        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            // 当此事件不为 Null，则调用此事件
            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }

        // 发送角色创建信息
        public void SendCharacterCreate(string name, CharacterClass cls)
        {
            Debug.LogFormat("UserCreateCharacterRequest::name :{0} class:{1}", name, cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = cls;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        // 角色创建协议接收函数，角色创建事件，反馈至 UI 层 
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            if (this.OnCharacterCreate != null)
            {
                this.OnCharacterCreate(response.Result, response.Errormsg);

            }
        }


    }
}
