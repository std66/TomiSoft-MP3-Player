namespace TomiSoft.MP3Player.Utils.Windows {
    /// <summary>
    /// Represents the media key codes used for RegisterHotKey
    /// and UnregisterHotKey functions in Hotkeys class.
    /// </summary>
    public enum VirtualKey : uint {
        /// <summary>
        /// The "Next track" button's virtual key code.
        /// </summary>
        VK_MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// The "Previous track" button's virtual key code.
        /// </summary>
        VK_MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// The "Stop" button's virtual key code.
        /// </summary>
        VK_MEDIA_STOP = 0xB2,

        /// <summary>
        /// The "Play/Pause" button's virtual key code.
        /// </summary>
        VK_MEDIA_PLAY_PAUSE = 0xB3
    }
}
