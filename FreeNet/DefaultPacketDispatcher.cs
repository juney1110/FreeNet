﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeNet
{
    /// <summary>
    /// 패킷을 처리해서 컨텐츠를 실행하는 곳이다.
    /// FreeNet을 사용할 때 LogicMessageEntry을 참고해서 IMessageDispatcher를 상속 받는 클래스를 맞게 구현하자
    /// </summary>
    public class DefaultPacketDispatcher : IPacketDispatcher
    {
        ILogicQueue MessageQueue = new DoubleBufferingQueue();
        

        public DefaultPacketDispatcher()
        {            
        }
                       

        public void IncomingPacket(bool IsSystem, Session user, ArraySegment<byte> buffer)
        {
            // 여긴 IO스레드에서 호출된다.
            // 완성된 패킷을 메시지큐에 넣어준다.
            var packet = new Packet(buffer, user);

            if(IsSystem == false && packet.PopProtocolId() <= (short)NetworkDefine.SYS_NTF_MAX)
            {
                //TODO: 로그 남기기 serilog otr nlog(여기에 serilog 사용 가능)
                // Serilogのログイベントからの情報抜き出し方法
                // https://qiita.com/skitoy4321/items/6863dd5c8e8eb7124130
                //ASP.NET Core～SerilogからSeqでロギングしてslackに通知する
                // http://ryuichi111std.hatenablog.com/entry/2016/07/20/111015

                // 시스템만 보내어야할 패킷을 상대방이 보냈음. 해킹 의심
                return;
            }

            MessageQueue.Enqueue(packet);
        }
        
        public Queue<Packet> DispatchAll()
        {
            return MessageQueue.TakeAll();
        }

              
    }
}
