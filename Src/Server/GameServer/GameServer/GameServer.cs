using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using GameServer.Network;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;
namespace GameServer
{
    class GameServer
    {
        NetService network; // 네트워크 서비스 변수 network
        Thread thread;
        bool running = false;
        

        public bool Init()
        {
            // network 인스턴스화, 포트 Port 묵인 8000
            int Port = Properties.Settings.Default.ServerPort;
            network = new NetService();
            network.Init(Port);

            DBService.Instance.Init();    //  DBService   열기
			DataManager.Instance.Load();  //  DataManager 열기
            UserService.Instance.Init();  //  UserService 열기

            thread = new Thread(new ThreadStart(this.Update)); 
            
            return true;
        }

        public void Start()
        {
            network.Start(); //...
            running = true;
            thread.Start();
        }

        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop(); //...
        }

        public void Update()
        {
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
            }
        }
    }
}
