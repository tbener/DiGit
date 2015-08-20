using System.Windows;
using System.Windows.Input;
using BondTech.HotKeyManagement.WPF._4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiGit.Helpers
{
    public delegate void HotkeyChangedEventHandler(object sender, EventArgs arg);

    internal static class HotkeyHelper
    {
        public static event HotkeyChangedEventHandler OnHotkeyChanged;

        private static HotKeyManager _hotKeyManager;
        private static GlobalHotKey _hk;

        static HotkeyHelper()
        {
            _hotKeyManager = new HotKeyManager(BubblesManager.OwnerWindow);
        }

        public static void RegisterHotkey(ModifierKeys modifiersKeys, string key)
        {
            Keys k = (Keys) new KeysConverter().ConvertFromString(key);
            if (_hk == null)
            {
                _hk = new GlobalHotKey("ShowHide", modifiersKeys, k, true);
                _hotKeyManager.AddGlobalHotKey(_hk);
                _hk.HotKeyPressed += (sender, args) => BubblesManager.ToggleShowHide();
            }
            else
            {
                _hk.Modifier = modifiersKeys;
                _hk.Key = k;
                _hk.Enabled = true;
            }
            if (OnHotkeyChanged != null)
                OnHotkeyChanged(_hk, new EventArgs());
        }

        public static string HotkeyShortcut
        {
            get { return _hk == null ? "" : string.Format("{0} + {1}", new ModifierKeysConverter().ConvertToString(_hk.Modifier), _hk.Key.ToString()); }
        }
        
        public static void Dispose()
        {
            _hotKeyManager.Dispose();
            _hotKeyManager = null;
            _hk = null;
        }
    }
}
