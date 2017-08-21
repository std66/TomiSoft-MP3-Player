namespace TomiSoft.MP3Player.Common.Lyrics {
    /// <summary>
    /// Stores informations about a translation.
    /// </summary>
    public class Translation {
        /// <summary>
        /// Gets the identifier of the translation.
        /// </summary>
        public string TranslationID { get; private set; }

        /// <summary>
        /// Gets the user-friendly display name of the translation.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        /// <param name="TranslationID">The identifier of the translation</param>
        /// <param name="DisplayName">The user-friendly display name of the translation</param>
        public Translation(string TranslationID, string DisplayName) {
            this.TranslationID = TranslationID;
            this.DisplayName = DisplayName;
        }

        public override bool Equals(object obj) {
            Translation o = obj as Translation;

            if (o == null)
                return false;

            return this == o;
        }

        public override int GetHashCode() {
            return 7 * this.TranslationID.GetHashCode() * this.DisplayName.GetHashCode();
        }

		public override string ToString() {
			return $"{this.DisplayName} ({this.TranslationID})";
		}

		public static bool operator ==(Translation a, Translation b) {
            return a?.TranslationID == b?.TranslationID;
        }

        public static bool operator !=(Translation a, Translation b) {
            return a?.TranslationID != b?.TranslationID;
        }
    }
}
