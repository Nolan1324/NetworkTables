﻿using FRC.NetworkTables.Interop;

namespace FRC.NetworkTables
{
    public readonly struct ConnectionNotification
    {
        public readonly ConnectionListener Listener;
        public readonly bool Connected;
        public readonly ConnectionInfo Conn;
        public readonly NetworkTableInstance Instance;

        internal unsafe ConnectionNotification(NetworkTableInstance inst, NtConnectionNotification* notification)
        {
            this.Listener = notification->listener;
            this.Connected = notification->connected.Get();
            this.Conn = new ConnectionInfo(&notification->conn);
            this.Instance = inst;
        }
    }
}
