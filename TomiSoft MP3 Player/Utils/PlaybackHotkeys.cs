using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using TomiSoft.MP3Player.Utils.Windows;

namespace TomiSoft.MP3Player.Utils {
    /// <summary>
    /// Provides a simple way to handle playback media keys of the keyboard.
    /// </summary>
    public class PlaybackHotkeys : IDisposable {
        /// <summary>
        /// This stack is used to store the keys which are successfully
        /// registered.
        /// </summary>
        private Stack<KeyValuePair<int, VirtualKey>> UndoStack;

        /// <summary>
        /// The window's instance that handles the system messages
        /// sent by Windows.
        /// </summary>
        private Window HandlerWindow;

        /// <summary>
        /// This event is fired when the Play/Pause key is pressed.
        /// </summary>
        public event EventHandler PlayPause;

        /// <summary>
        /// This event is fired when the Stop key is pressed.
        /// </summary>
        public event EventHandler Stop;

        /// <summary>
        /// This event is fired when the Next Track key is pressed.
        /// </summary>
        public event EventHandler NextTrack;

        /// <summary>
        /// This event is fired when the Previous Track key is pressed.
        /// </summary>
        public event EventHandler PreviousTrack;

        /// <summary>
        /// Gets if all media keys are successfully registered.
        /// </summary>
        public bool Registered {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the PlaybackHotkeys class.
        /// </summary>
        /// <param name="HandlerWindow">The window that will handle the system messages</param>
        public PlaybackHotkeys(Window HandlerWindow) {
            #region Error checking
            if (HandlerWindow == null)
                throw new ArgumentNullException(nameof(HandlerWindow));
            #endregion

            this.UndoStack = new Stack<KeyValuePair<int, VirtualKey>>();
            this.Registered = true;
            this.HandlerWindow = HandlerWindow;

            this.RegisterAllKeys();

            if (!this.Registered) {
                this.UnregisterAllRegisteredKeys();
            }
        }

        /// <summary>
        /// Hook up this method to the window's HwndSource instance. For the documentation of the
        /// parameters, see GetMessage function's documentation on MSDN.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns>Always returns IntPtr.Zero</returns>
        public IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == Hotkeys.WM_HOTKEY) {
                switch (this.IDToVirtualKey(wParam.ToInt32())) {
                    case VirtualKey.VK_MEDIA_PLAY_PAUSE:
                        this.PlayPause?.Invoke(null, EventArgs.Empty);
                        break;

                    case VirtualKey.VK_MEDIA_STOP:
                        this.Stop?.Invoke(null, EventArgs.Empty);
                        break;

                    case VirtualKey.VK_MEDIA_PREV_TRACK:
                        this.PreviousTrack?.Invoke(null, EventArgs.Empty);
                        break;

                    case VirtualKey.VK_MEDIA_NEXT_TRACK:
                        this.NextTrack?.Invoke(null, EventArgs.Empty);
                        break;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the VirtualKey value of the corresponding hotkey registration ID.
        /// </summary>
        /// <param name="ID">The ID that is used to register the VirtualKey</param>
        /// <returns>The VirtualKey value associated to the ID</returns>
        private VirtualKey IDToVirtualKey(int ID) {
            return this.UndoStack.Where(x => x.Key == ID).FirstOrDefault().Value;
        }

        /// <summary>
        /// Registers all hotkeys.
        /// </summary>
        private void RegisterAllKeys() {
            foreach (var CurrentEntry in this.GetKeysWithID()) {
                bool Registered = Hotkeys.Register(
                    HandlerWindow:  this.HandlerWindow,
                    ID:             CurrentEntry.Key,
                    Key:            CurrentEntry.Value
                );

                if (Registered) {
                    this.UndoStack.Push(CurrentEntry);
                }
                else {
                    this.Registered = false;
                    Trace.TraceWarning($"Failed to register the hotkey: RegistrationID={CurrentEntry.Key}, Key={CurrentEntry.Value}");
                }
            }
        }

        /// <summary>
        /// Unregisters all previously successfully registered hotkeys.
        /// </summary>
        private void UnregisterAllRegisteredKeys() {
            while (this.UndoStack.Count > 0) {
                var Entry = this.UndoStack.Pop();

                bool Unregistered = Hotkeys.Unregister(
                    HandlerWindow:  this.HandlerWindow,
                    ID:             Entry.Key
                );

                if (!Unregistered) {
                    Trace.TraceWarning($"Failed to unregister the hotkey: RegistrationID={Entry.Key}, Key={Entry.Value}");
                }
            }

            this.disposedValue = true;
        }

        /// <summary>
        /// Gets all the media keys with an associated ID.
        /// </summary>
        /// <returns>A list of media keys with their corresponding ID</returns>
        private IEnumerable<KeyValuePair<int, VirtualKey>> GetKeysWithID() {
            return new List<KeyValuePair<int, VirtualKey>>() {
                new KeyValuePair<int, VirtualKey>(100, VirtualKey.VK_MEDIA_NEXT_TRACK),
                new KeyValuePair<int, VirtualKey>(101, VirtualKey.VK_MEDIA_PLAY_PAUSE),
                new KeyValuePair<int, VirtualKey>(102, VirtualKey.VK_MEDIA_PREV_TRACK),
                new KeyValuePair<int, VirtualKey>(103, VirtualKey.VK_MEDIA_STOP)
            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases all previously successfully registered hotkeys.
        /// </summary>
        /// <param name="disposing">Not used</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                this.UnregisterAllRegisteredKeys();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all previously successfully registered hotkeys.
        /// </summary>
        ~PlaybackHotkeys() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Releases all previously successfully registered hotkeys.
        /// </summary>
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
