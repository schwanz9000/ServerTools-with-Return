using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ServerTools
{
    public class TeleportReturn
    {
        public static bool IsEnabled = false;
        public static int DelayBetweenUses = 60;
        private static SortedDictionary<string, string> _savedReturns = new SortedDictionary<string, string>();
        private static SortedDictionary<string, DateTime> _lastused = new SortedDictionary<string, DateTime>();
        private static string _file = "SavedReturnsData.xml";
        private static string _filepath = string.Format("{0}/{1}", Config._datapath, _file);

        public static void Init()
        {
            LoadSavedReturnsXml();
        }

        public static void LoadSavedReturnsXml()
        {
            if (!Utils.FileExists(_filepath))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(_filepath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", _file, e.Message));
                return;
            }
            XmlNode _ConfigXml = xmlDoc.DocumentElement;
            _savedReturns.Clear();
            _lastused.Clear();
            foreach (XmlNode childNode in _ConfigXml.ChildNodes)
            {
                if (childNode.Name == "Returns")
                {
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Returns' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("steamId"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Return entry because of missing 'steamId' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("pos"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Return entry because of missing 'pos' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("lastused"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Return entry because of missing 'lastused' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        _savedReturns.Add(_line.GetAttribute("steamId"), _line.GetAttribute("pos"));
                        DateTime dt;
                        if (!DateTime.TryParse(_line.GetAttribute("lastused"), out dt))
                        {
                            continue;
                        }
                        _lastused.Add(_line.GetAttribute("steamId"), dt);
                    }
                }
            }
        }

        private static void UpdateXml()
        {
            if (!Directory.Exists(Config._datapath))
            {
                Directory.CreateDirectory(Config._datapath);
            }
            using (StreamWriter sw = new StreamWriter(_filepath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<SavedReturns>");
                sw.WriteLine("    <Returns>");
                foreach (KeyValuePair<string, string> kvp in _savedReturns)
                {
                    DateTime _datetime;
                    if (_lastused.TryGetValue(kvp.Key, out _datetime))
                    {
                        sw.WriteLine(string.Format("        <Return steamId=\"{0}\" pos=\"{1}\" lastused=\"{2}\" />", kvp.Key, kvp.Value, _datetime));
                    }
                    else
                    {
                        sw.WriteLine(string.Format("        <Return steamId=\"{0}\" pos=\"{1}\" lastused=\"\" />", kvp.Key, kvp.Value));
                    }
                }
                sw.WriteLine("    </Returns>");
                sw.WriteLine("</SavedReturns>");
                sw.Flush();
                sw.Close();
            }
        }

        public static void SetReturn(ClientInfo _cInfo)
        {
            if (_savedReturns.ContainsKey(_cInfo.playerId))
            {
                string _phrase15 = "{PlayerName} you already have a Return set.";
                if (Phrases._Phrases.TryGetValue(15, out _phrase15))
                {
                    _phrase15 = _phrase15.Replace("{0}", _cInfo.playerName);
                    _phrase15 = _phrase15.Replace("{PlayerName}", _cInfo.playerName);
                }
                _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase15, CustomCommands._chatcolor), "Server"));
            }
            else
            {
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                Vector3 _position = _player.GetPosition();
                string x = _position.x.ToString();
                string y = _position.y.ToString();
                string z = _position.z.ToString();
                string _sposition = x + "," + y + "," + z;
                _savedReturns.Add(_cInfo.playerId, _sposition);
                string _phrase16 = "{PlayerName} your Return has been saved.";
                if (Phrases._Phrases.TryGetValue(16, out _phrase16))
                {
                    _phrase16 = _phrase16.Replace("{0}", _cInfo.playerName);
                    _phrase16 = _phrase16.Replace("{PlayerName}", _cInfo.playerName);
                }
                _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase16, CustomCommands._chatcolor), "Server"));
                UpdateXml();
            }
        }

        public static void DelReturn(ClientInfo _cInfo)
        {
            if (!_savedReturns.ContainsKey(_cInfo.playerId))
            {
                string _phrase17 = "{PlayerName} you do not have a Return saved.";
                if (Phrases._Phrases.TryGetValue(17, out _phrase17))
                {
                    _phrase17 = _phrase17.Replace("{0}", _cInfo.playerName);
                    _phrase17 = _phrase17.Replace("{PlayerName}", _cInfo.playerName);
                }
                _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase17, CustomCommands._chatcolor), "Server"));
            }
            else
            {
                _savedReturns.Remove(_cInfo.playerId);
                string _phrase18 = "{PlayerName} your Return has been removed.";
                if (Phrases._Phrases.TryGetValue(18, out _phrase18))
                {
                    _phrase18 = _phrase18.Replace("{0}", _cInfo.playerName);
                    _phrase18 = _phrase18.Replace("{PlayerName}", _cInfo.playerName);
                }
                _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase18, CustomCommands._chatcolor), "Server"));
                UpdateXml();
            }
        }

        public static void TeleReturn(ClientInfo _cInfo)
        {
            string _position;
            if (!_savedReturns.TryGetValue(_cInfo.playerId, out _position))
            {
                string _phrase17 = "{PlayerName} you do not have a Return saved.";
                if (Phrases._Phrases.TryGetValue(17, out _phrase17))
                {
                    _phrase17 = _phrase17.Replace("{0}", _cInfo.playerName);
                    _phrase17 = _phrase17.Replace("{PlayerName}", _cInfo.playerName);
                }
                _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase17, CustomCommands._chatcolor), "Server"));
            }
            else
            {
                float x;
                float y;
                float z;
                string[] _cords = _position.Split(',');
                float.TryParse(_cords[0], out x);
                float.TryParse(_cords[1], out y);
                float.TryParse(_cords[2], out z);
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                UnityEngine.Vector3 destPos = new UnityEngine.Vector3();
                destPos.x = x;
                destPos.y = -1;
                destPos.z = z;
                NetPackageTeleportPlayer pkg = new NetPackageTeleportPlayer(destPos);
                DateTime _datetime;
                if (DelayBetweenUses > 0 && _lastused.TryGetValue(_cInfo.playerId, out _datetime))
                {
                    int _passedtime = time.GetMinutes(_datetime);
                    if (_passedtime > DelayBetweenUses)
                    {
                        _lastused.Remove(_cInfo.playerId);
                        _cInfo.SendPackage(pkg);
                        _lastused.Add(_cInfo.playerId, DateTime.Now);
                        UpdateXml();
                    }
                    else
                    {
                        int _timeleft = DelayBetweenUses - _passedtime;
                        string _phrase19 = "{PlayerName} you can only use /return once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                        if (Phrases._Phrases.TryGetValue(19, out _phrase19))
                        {
                            _phrase19 = _phrase19.Replace("{0}", _cInfo.playerName);
                            _phrase19 = _phrase19.Replace("{1}", DelayBetweenUses.ToString());
                            _phrase19 = _phrase19.Replace("{2}", _timeleft.ToString());
                            _phrase19 = _phrase19.Replace("{PlayerName}", _cInfo.playerName);
                            _phrase19 = _phrase19.Replace("{DelayBetweenUses}", DelayBetweenUses.ToString());
                            _phrase19 = _phrase19.Replace("{TimeRemaining}", _timeleft.ToString());
                        }
                        _cInfo.SendPackage(new NetPackageGameMessage(string.Format("{1}{0}[-]", _phrase19, CustomCommands._chatcolor), "Server"));
                    }
                }
                else
                {
                    _cInfo.SendPackage(pkg);
                    if (_lastused.ContainsKey(_cInfo.playerId))
                    {
                        _lastused.Remove(_cInfo.playerId);
                    }
                    _lastused.Add(_cInfo.playerId, DateTime.Now);
                    UpdateXml();
                }
            }
        }
    }
}