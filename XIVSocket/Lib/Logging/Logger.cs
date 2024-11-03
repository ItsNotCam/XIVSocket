using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVSocket.App.Network;

namespace XIVSocket.App.Logging
{
    public class Logger
    {
        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            VERBOSE
        }

        public bool transmitToSocket;
        private NetworkManager networkManager;

        public Logger(NetworkManager networkManager, bool transmitToSocket=false) {
            // if the config file says to transmit logs to the socket, set the transmitToSocket variable to true
            this.transmitToSocket = transmitToSocket;
            this.networkManager = networkManager;
        }

        public void Log(string message, LogLevel logLevel)
        {
            switch(logLevel) {
                case LogLevel.DEBUG:
                    Plugin.PluginLogger.Debug(message);
                    break;
                case LogLevel.INFO:
                    Plugin.PluginLogger.Info(message);
                    break;
                case LogLevel.WARN:
                    Plugin.PluginLogger.Warning(message);
                    break;
                case LogLevel.ERROR:
                    Plugin.PluginLogger.Error(message);
                    break;
                case LogLevel.FATAL:
                    Plugin.PluginLogger.Fatal(message);
                    break;
                case LogLevel.VERBOSE:
                    Plugin.PluginLogger.Verbose(message);
                    break;
            }

            if (transmitToSocket && logLevel != LogLevel.VERBOSE) {
                networkManager.SendUdpMessage($"[{logLevel.ToString()}] {message}");
            }
        }
    }
}
